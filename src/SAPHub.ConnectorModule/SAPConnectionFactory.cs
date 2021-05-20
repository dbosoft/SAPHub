using System;
using System.Collections.Generic;
using Dbosoft.YaNco;
using Dbosoft.YaNco.TypeMapping;
using LanguageExt;
using Microsoft.Extensions.Configuration;
using ConnectionBuilder = Dbosoft.YaNco.ConnectionBuilder;

namespace SAPHub.ConnectorModule
{
    /// <summary>
    /// This factory is used to automatically create RFC connection from configuration. 
    /// </summary>
    public class SAPConnectionFactory
    {
        private readonly IConfiguration _configuration;
        private readonly Func<ILogger, IFieldMapper, IRfcRuntime> _runtimeFactory;
        public SAPConnectionFactory(IConfiguration configuration, Func<ILogger, IFieldMapper, IRfcRuntime> runtimeFactory)
        {
            _configuration = configuration;
            _runtimeFactory = runtimeFactory;
        }

        public Dictionary<string, string> CreateSettings()
        {
            var config = new Dictionary<string, string>();
            _configuration.Bind("saprfc", config);
            if(config.Count == 0)
                throw new RfcConfigMissingException("Configuration for SAP connection is missing. Add configuration with key 'saprfc'");

            return config;
        }

        public Func<EitherAsync<RfcErrorInfo, IConnection>> CreateConnectionFunc()
        {
            var builder = new ConnectionBuilder(CreateSettings())
                .ConfigureRuntime(c =>
                    c.UseFactory(_runtimeFactory));

            return builder.Build();

        }
    }
}