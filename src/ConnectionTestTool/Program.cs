using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dbosoft.YaNco;
using Microsoft.Extensions.Configuration;

namespace ConnectionTestTool
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .AddUserSecrets<Program>()
                .Build();

            var config = new Dictionary<string, string>(); 
            configuration.Bind("saprfc", config);


            if (config.Count == 0)
            {
                Console.WriteLine("Configuration for SAP connection is missing.");
                return -1;
            }

            var connectionFactory = new ConnectionBuilder(config).Build();

            try
            {
                using var rfcContext = new RfcContext(connectionFactory);
                await rfcContext.Ping().IfLeftAsync(l => throw new Exception(l.Message));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Connection failed. Message: {ex.Message}");
                return -1;
            }

            Console.WriteLine("Connection opened.");
            return 0;
        }
    }
}
