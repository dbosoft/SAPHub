using System.Threading;
using System.Threading.Tasks;
using Dbosoft.Hosuto.Modules.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SAPHub.Connector;
using SAPHub.MessageBus;

namespace SAPHub.SAPConnector
{
    class Program
    {
        public static void Main(string[] args)
        {
            RfcLibraryHelper.EnsurePathVariable();

            //give started bus some time to start up
            Thread.Sleep(5000);

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            var services = new ServiceCollection()
                .AddTransportSelector();

            return ModulesHost.CreateDefaultBuilder(args)
                .UseServiceCollection(services)
                .HostModule<SAPConnectorModule>()
                .ConfigureHostConfiguration(config => config
                    .AddEnvironmentVariables("SAPHUB_")
                    .AddUserSecrets<Program>()
                );
        }

    }
}
