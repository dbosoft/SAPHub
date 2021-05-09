using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Rebus.Bus;
using Rebus.Exceptions;
using Rebus.Handlers;
using Rebus.Messages;
using Rebus.Retry.Simple;
using Rebus.Transport;
using SAPHub.Messages;

namespace SAPHub.Connector
{
    [UsedImplicitly]
    public class FailedMessageHandler<T> : IHandleMessages<T> where T: IFailed<OperationCommand>
    {
        private readonly IBus _bus;

        public FailedMessageHandler(IBus bus)
        {
            _bus = bus;
        }


        public async Task Handle(T failed)
        {
            var doNotDefer = failed.Exceptions.Any(x => x is InvalidOperationException);

            const int maxDeferCount = 3;
            var deferCount = Convert.ToInt32(failed.Headers.GetValueOrDefault(Headers.DeferCount));
            if (deferCount >= maxDeferCount || doNotDefer)
            {

                await _bus.Advanced.TransportMessage.Deadletter($"Failed after {deferCount} deferrals\n\n{failed.ErrorDescription}");

                var message = failed.Exceptions.FirstOrDefault()?.Message ?? failed.ErrorDescription;

                using var scope = new RebusTransactionScope();
                await _bus.Publish(new OperationStatusEvent
                {
                    Id = Guid.NewGuid(),
                    OperationId = failed.Message.Id,
                    Status = OperationStatus.Failed,
                    StatusMessage = message
                });
                await scope.CompleteAsync().ConfigureAwait(false);
            }
            await _bus.Advanced.TransportMessage.Defer(TimeSpan.FromSeconds(30));
        }
    }
}