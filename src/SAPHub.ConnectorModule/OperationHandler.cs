using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Rebus.Bus;
using Rebus.Handlers;
using Rebus.Transport;
using SAPHub.Messages;

namespace SAPHub.ConnectorModule
{
    /// <summary>
    /// This generic handler will process a operation command send by Rebus.
    /// </summary>
    /// <typeparam name="T">Operation type</typeparam>
    public abstract class OperationHandler<T> : IHandleMessages<T> where T: OperationCommand
    {
        private readonly IBus _bus;

        protected OperationHandler(IBus bus)
        {
            _bus = bus;
        }

        public async Task Handle(T message)
        {
            //send status update, that we picked up message in it's own scope
            //otherwise it will be send after processing and almost at same time as processed message status
            using (var scope = new RebusTransactionScope())
            {
                await _bus.Publish(new OperationStatusEvent
                    {Id = Guid.NewGuid(), OperationId = message.Id, Status = OperationStatus.Running});
                await scope.CompleteAsync().ConfigureAwait(false);
            }

            var result = await HandleOperation(message);

            //ok, finished, now send result back to client
            var statusEvent = new OperationStatusEvent
            {
                Id = Guid.NewGuid(),
                OperationId = message.Id,
                Status = OperationStatus.Completed
            };

            //Remarks if you use that for you own data: 
            //keep in mind that data will be included in entire message. 
            //Message systems typical restrict message size, so you may have to separate result 
            //and data. This can also be achieved with Rebus but would require a additional storage. 
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

        public abstract Task<IEnumerable<object>> HandleOperation(T message);
    }
}