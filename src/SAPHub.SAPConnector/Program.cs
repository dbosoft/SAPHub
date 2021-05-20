using Dbosoft.Hosuto.Modules.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SAPHub.ConnectorModule;
using SAPHub.MessageBus;

namespace SAPHub.SAPConnector
{
    class Program
    {
        public static void Main(string[] args)
        {
            //this makes sure that UCI libraries can be found by
            //sapnwrfc. It is only required for some platforms.
            RfcLibraryHelper.EnsurePathVariable();

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            //setup shared ServiceProvider
            var services = new ServiceCollection()
                .AddTransportSelector();    // chooses Rebus transport by configuration

            // create ModulesHosts and host SAPConnector module
            return ModulesHost.CreateDefaultBuilder(args)
                .UseServiceCollection(services)
                        .HostModule<SAPConnectorModule>()
                        .ConfigureHostConfiguration(config => config
                            .AddEnvironmentVariables("SAPHUB_")
                        );
                }

    }
}
