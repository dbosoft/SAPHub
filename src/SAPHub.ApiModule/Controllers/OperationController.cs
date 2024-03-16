using System;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using SAPHub.ApiModule.ApiModel;
using Swashbuckle.AspNetCore.Annotations;

namespace SAPHub.ApiModule.Controllers;

[ApiController]
[Route("[controller]")]
public class OperationsController(IOperationService operationService) : ControllerBase
{
    [HttpGet]
    [SwaggerOperation(OperationId = "Operation_List")]
    public Task<IEnumerable<Operation>> Get()
    {
        return operationService.GetAll();
    }

    [HttpGet("{id}")]
    [SwaggerOperation(OperationId = "Operation_Get")]
    public Task<Operation> Get(Guid id)
    {
        return operationService.Get(id);
    }


}