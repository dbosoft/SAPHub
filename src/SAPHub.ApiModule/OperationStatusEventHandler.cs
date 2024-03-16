using System.IO;
using System.Threading.Tasks;
using Rebus.Handlers;
using SAPHub.Messages;
using SAPHub.StateDb;
using SAPHub.StateDb.Model;

namespace SAPHub.ApiModule;

public class OperationStatusEventHandler(IStateStoreRepository<OperationModel> repository) : IHandleMessages<OperationStatusEvent>
{
    public async Task Handle(OperationStatusEvent message)
    {
        var op = await repository.GetByIdAsync(message.OperationId);

        if (op == null)
            return;

        // skip messages if already closed
        if (op.Status == OperationStatus.Completed || op.Status == OperationStatus.Failed)
            return;
            
        op.Status = message.Status;
        op.StatusMessage = message.StatusMessage;
        op.ResultType = message.ResultType;

        if (message.Attachment != null)
        {
            await using var dataStream = await message.Attachment.OpenRead();
            using var reader = new StreamReader(dataStream);
            op.ResultData = await reader.ReadToEndAsync();
        }

        await repository.UpdateAsync(op);
    }
}