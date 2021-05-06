using System;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using SAPHub.ApiModule.ApiModel;
using Swashbuckle.AspNetCore.Annotations;

namespace SAPHub.ApiModule.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OperationController : ControllerBase
    {
        private readonly IOperationService _operationService;


        public OperationController(IOperationService operationService)
        {
            _operationService = operationService;
        }

        [HttpGet]
        [SwaggerOperation(OperationId = "Operation_List")]
        public Task<IEnumerable<ApiModel.Operation>> Get()
        {
            return _operationService.GetAll();
        }

        [HttpGet("{id}")]
        [SwaggerOperation(OperationId = "Operation_Get")]
        public Task<Operation> Get(Guid id)
        {
            return _operationService.Get(id);
        }


    }
}
