using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SAPHub.ApiModule.ApiModel;
using SAPHub.Messages;
using Swashbuckle.AspNetCore.Annotations;

namespace SAPHub.ApiModule.Controllers
{
    [Route("[controller]")]
    public class CompanyController : ControllerBase
    {
        private readonly IOperationService _operationService;
        
        public CompanyController(IOperationService operationService)
        {
            _operationService = operationService;
        }

        [HttpGet]
        [SwaggerOperation(OperationId = "Company_List")]
        public Task<ApiModel.Operation> Get()
        {
            return _operationService.New(new GetCompaniesCommand());
        }

        [HttpGet("{id}")]
        [SwaggerOperation(OperationId = "Company_Get")]
        public Task<ApiModel.Operation> Get(string id)
        {
            return _operationService.New(new GetCompanyCommand { Code = id });
        }


        [HttpGet("Result/{operationId}")]
        [SwaggerOperation(OperationId = "Company_GetResult")]

        public Task<OperationResult<Company>> GetResult(Guid operationId)
        {
            return _operationService.GetResult<CompanyData, Company> (operationId,
                x => new Company { Code = x.Code, Name = x.Name });
        }

    }
}