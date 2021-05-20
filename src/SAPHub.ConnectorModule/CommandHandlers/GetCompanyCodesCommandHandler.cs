using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dbosoft.YaNco;
using JetBrains.Annotations;
using Rebus.Bus;
using SAPHub.Messages;

namespace SAPHub.ConnectorModule.CommandHandlers
{
    [UsedImplicitly]
    public class GetCompanyCodesCommandHandler : OperationHandler<GetCompanyCodesCommand>
    {
        private readonly IRfcContext _rfcContext;

        public GetCompanyCodesCommandHandler(IBus bus, IRfcContext rfcContext) : base(bus)
        {
            _rfcContext = rfcContext;
        }

        public override async Task<IEnumerable<object>> HandleOperation(GetCompanyCodesCommand message)
        {
            return await _rfcContext.CallFunction("BAPI_COMPANYCODE_GETLIST",
                Output: f => f
                    .MapTable("COMPANYCODE_LIST", s =>
                        from code in s.GetField<string>("COMP_CODE")
                        from name in s.GetField<string>("COMP_NAME")
                        select new CompanyCodeData()
                        {
                            Code = code,
                            Name = name
                        })).IfLeft(l => throw new Exception(l.Message));

        }
    }
}