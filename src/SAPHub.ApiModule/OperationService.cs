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

namespace SAPHub.ApiModule
{
    class OperationService : IOperationService
    {
        private readonly IStateStoreRepository<OperationModel> _repository;
        private readonly IBus _bus;

        public OperationService(IStateStoreRepository<OperationModel> repository, IBus bus)
        {
            _repository = repository;
            _bus = bus;
        }

        public async Task<IEnumerable<ApiModel.Operation>> GetAll()
        {
            return (await _repository.ListAsync()).Select(x => x.ToApiModel());
        }

        public async Task<ApiModel.Operation> Get(Guid operationId)
        {
            return (await _repository.GetByIdAsync(operationId)).ToApiModel();
        }

        public async Task<ApiModel.Operation> New<T>(T command) where T : OperationCommand
        {
            command.Id = Guid.NewGuid();

            var opModel = new OperationModel {Id = command.Id, Status = OperationStatus.Queued};

            await _repository.AddAsync(opModel);
            await _bus.Send(command);

            return opModel.ToApiModel();
        }

        public async Task<OperationResult<TData>> GetResult<TResult, TData>(Guid operationId, Func<TResult, TData> mapperFunc)
        {
            var opModel = await _repository.GetByIdAsync(operationId);

            if (opModel == null)
                return null;

            var opResult = new OperationResult<TData>()
            {
                Message = opModel.StatusMessage,
                OperationId = operationId,
                OperationStatus = opModel.Status
            };
            
            if (opModel.ResultData == null) return opResult;


            var dataType = Type.GetType(opModel.ResultType);
            var enumerableDataType = typeof(IEnumerable<>).MakeGenericType(dataType);

            if (typeof(IEnumerable<TResult>).IsAssignableFrom(enumerableDataType))
            {
                opResult.Result = ((IEnumerable<TResult>) JsonConvert.DeserializeObject(opModel.ResultData, enumerableDataType)).Select(mapperFunc);
            }

            return opResult;
        }
    }
}