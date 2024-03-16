using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SAPHub.ApiModule.ApiModel;
using SAPHub.Messages;

namespace SAPHub.ApiModule;

public interface IOperationService
{
    Task<IEnumerable<Operation>> GetAll();
    Task<Operation> Get(Guid operationId);
    Task<Operation> New<T>(T command) where T : OperationCommand;
    Task<OperationResult<TData>> GetResult<TResult,TData>(Guid operationId, Func<TResult,TData> mapperFunc);
}