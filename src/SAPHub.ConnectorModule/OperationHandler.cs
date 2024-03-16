using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Rebus.Bus;
using Rebus.Handlers;
using Rebus.Transport;
using SAPHub.Messages;

namespace SAPHub.ConnectorModule;

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

        //attach result to database
        //this avoids sending the entire data over the message queue as 
        //it is typical restricted in message size. 
        //The databus is a special storage to transport the data in parallel

        if (result != null)
        {
            var resultArray = result.ToArray();
            var firstEntry = resultArray.FirstOrDefault();
            if (firstEntry != null)
            {
                using var jsonStream = new MemoryStream();

                await using var writer = new StreamWriter(jsonStream);
                using var jsonWriter = new JsonTextWriter(writer);
                var ser = new JsonSerializer();
                ser.Serialize(jsonWriter, resultArray);
                await jsonWriter.FlushAsync();
                jsonStream.Seek(0, SeekOrigin.Begin);

                statusEvent.Attachment = await _bus.Advanced.DataBus.CreateAttachment(jsonStream);
                statusEvent.ResultType = firstEntry.GetType().AssemblyQualifiedName;

            }

        }

        await _bus.Publish(statusEvent);


    }

    public abstract Task<IEnumerable<object>> HandleOperation(T message);
}