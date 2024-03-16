using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Rebus.Bus;
using SAPHub.ApiModule.ApiModel;
using SAPHub.Messages;
using SAPHub.StateDb;
using SAPHub.StateDb.Model;

namespace SAPHub.ApiModule;

internal class OperationService(IStateStoreRepository<OperationModel> repository, IBus bus) : IOperationService
{
    public async Task<IEnumerable<Operation>> GetAll()
    {
        return (await repository.ListAsync()).Select(x => x.ToApiModel());
    }

    public async Task<Operation> Get(Guid operationId)
    {
        return (await repository.GetByIdAsync(operationId)).ToApiModel();
    }

    public async Task<Operation> New<T>(T command) where T : OperationCommand
    {
        command.Id = Guid.NewGuid();

        var opModel = new OperationModel {Id = command.Id, Status = OperationStatus.Queued};

        await repository.AddAsync(opModel);
        await bus.Send(command);

        return opModel.ToApiModel();
    }

    public async Task<OperationResult<TData>> GetResult<TResult, TData>(Guid operationId, Func<TResult, TData> mapperFunc)
    {
        var opModel = await repository.GetByIdAsync(operationId);

        if (opModel == null)
            return null;

        var opResult = new OperationResult<TData>
        {
            Message = opModel.StatusMessage,
            OperationId = operationId,
            OperationStatus = opModel.Status
        };
            
        if (opModel.ResultData == null) return opResult;


        var dataType = Type.GetType(opModel.ResultType);
        if (dataType != null)
        {
            var enumerableDataType = typeof(IEnumerable<>).MakeGenericType(dataType);

            if (typeof(IEnumerable<TResult>).IsAssignableFrom(enumerableDataType))
            {
                opResult.Result = ((IEnumerable<TResult>) JsonConvert.DeserializeObject(opModel.ResultData, enumerableDataType) ?? Array.Empty<TResult>()).Select(mapperFunc);
            }
        }

        return opResult;
    }
}