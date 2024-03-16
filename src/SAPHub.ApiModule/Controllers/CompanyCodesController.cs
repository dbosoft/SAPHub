using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SAPHub.ApiModule.ApiModel;
using SAPHub.Messages;
using Swashbuckle.AspNetCore.Annotations;

namespace SAPHub.ApiModule.Controllers;

/// <summary>
/// API controller for company code queries
/// </summary>
[Route("[controller]")]
[ApiController]
public class CompanyCodesController(IOperationService operationService) : ControllerBase
{
    /// <summary>
    /// Starts a query for company codes.
    /// </summary>
    /// <returns>Operation that has been queued.</returns>
    [HttpGet]
    [SwaggerOperation(OperationId = "CompanyCode_List")]
    public Task<Operation> Get()
    {
        return operationService.New(new GetCompanyCodesCommand());
    }

    /// <summary>
    /// Starts a query for a single company codes.
    /// </summary>
    /// <param name="id">Operation that has been queued.</param>
    /// <returns></returns>
    [HttpGet("{id}")]
    [SwaggerOperation(OperationId = "CompanyCode_Get")]
    public Task<Operation> Get(string id)
    {
        return operationService.New(new GetCompanyCodeCommand { Code = id });
    }

    /// <summary>
    /// Gets result from a company code request. 
    /// </summary>
    /// <param name="operationId">id of Operation</param>
    /// <returns>Operation result. Result field will be null if command was not processed.</returns>
    [HttpGet("Result/{operationId}")]
    [SwaggerOperation(OperationId = "CompanyCode_GetResult")]

    public Task<OperationResult<Company>> GetResult(Guid operationId)
    {
        return operationService.GetResult<CompanyCodeData, Company> (operationId,
            x => new Company { Code = x.Code, Name = x.Name });
    }

}