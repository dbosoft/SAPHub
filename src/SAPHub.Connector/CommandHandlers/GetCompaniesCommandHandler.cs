using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dbosoft.YaNco;
using JetBrains.Annotations;
using Rebus.Bus;

namespace SAPHub.Connector.CommandHandlers
{
    [UsedImplicitly]
    public class GetCompaniesCommandHandler : OperationHandler<GetCompaniesCommand>
    {
        private readonly IRfcContext _rfcContext;

        public GetCompaniesCommandHandler(IBus bus, IRfcContext rfcContext) : base(bus)
        {
            _rfcContext = rfcContext;
        }

        public override async Task<IEnumerable<object>> HandleOperation(GetCompaniesCommand message)
        {
            return await _rfcContext.CallFunction("BAPI_COMPANYCODE_GETLIST",
                Output: f => f
                    .MapTable("COMPANYCODE_LIST", s =>
                        from code in s.GetField<string>("COMP_CODE")
                        from name in s.GetField<string>("COMP_NAME")
                        select new CompanyData()
                        {
                            Code = code,
                            Name = name
                        })).IfLeft(l => throw new Exception(l.Message));

        }
    }
}