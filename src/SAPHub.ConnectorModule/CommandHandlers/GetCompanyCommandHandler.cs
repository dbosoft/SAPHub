using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dbosoft.YaNco;
using JetBrains.Annotations;
using Rebus.Bus;
using SAPHub.Messages;

namespace SAPHub.Connector.CommandHandlers
{
    [UsedImplicitly]
    public class GetCompanyCommandHandler : OperationHandler<GetCompanyCommand>
    {
        private readonly IRfcContext _rfcContext;

        public GetCompanyCommandHandler(IBus bus, IRfcContext rfcContext) : base(bus)
        {
            _rfcContext = rfcContext;
        }

        public override async Task<IEnumerable<object>> HandleOperation(GetCompanyCommand message)
        {
            return new[]
            {
                await _rfcContext.CallFunction("BAPI_COMPANYCODE_GETDETAIL",
                    Input: f => f
                        .SetField("COMPANYCODEID", message.Code),
                    Output: f => f
                        .MapStructure("COMPANYCODE_DETAIL", s =>
                            from code in s.GetField<string>("COMP_CODE")
                            from name in s.GetField<string>("COMP_NAME")
                            select new CompanyData()
                            {
                                Code = code,
                                Name = name
                            })).IfLeft(l => throw new Exception(l.Message))
            };

        }
    }
}