using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Rebus.Bus;
using Rebus.Handlers;
using SAPHub.Messages;

namespace SAPHub.Connector
{
    public abstract class OperationHandler<T> : IHandleMessages<T> where T: OperationCommand
    {
        private readonly IBus _bus;

        protected OperationHandler(IBus bus)
        {
            _bus = bus;
        }

        public async Task Handle(T message)
        {
            await _bus.Publish(new OperationStatusEvent { Id = Guid.NewGuid(), OperationId = message.Id, Status = OperationStatus.Running});

            try
            {
                var result = await HandleOperation(message);
                var statusEvent = new OperationStatusEvent
                {
                    Id = Guid.NewGuid(),
                    OperationId = message.Id,
                    Status = OperationStatus.Completed
                };

                if (result != null)
                {
                    var resultArray = result.ToArray();
                    var firstEntry = resultArray.FirstOrDefault();
                    if (firstEntry != null)
                    {
                        statusEvent.ResultData = JsonConvert.SerializeObject(resultArray, Formatting.None);
                        statusEvent.ResultType = firstEntry.GetType().AssemblyQualifiedName;

                    }

                }

                await _bus.Publish(statusEvent);

            }
            catch (Exception ex)
            {
                await _bus.Publish(new OperationStatusEvent
                {
                    Id = Guid.NewGuid(),
                    OperationId = message.Id, 
                    Status = OperationStatus.Failed,
                    StatusMessage = ex.Message
                });

            }

        }

        public abstract Task<IEnumerable<object>> HandleOperation(T message);
    }
}