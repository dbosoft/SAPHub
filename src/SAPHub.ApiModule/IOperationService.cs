using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SAPHub.ApiModule.ApiModel;
using SAPHub.ApiModule.Controllers;

namespace SAPHub.ApiModule
{
    public interface IOperationService
    {
        Task<IEnumerable<ApiModel.Operation>> GetAll();
        Task<ApiModel.Operation> Get(Guid operationId);
        Task<ApiModel.Operation> New<T>(T command) where T : OperationCommand;
        Task<OperationResult<TData>> GetResult<TResult,TData>(Guid operationId, Func<TResult,TData> mapperFunc);
    }
}