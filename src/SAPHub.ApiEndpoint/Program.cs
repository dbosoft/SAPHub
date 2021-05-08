using System.Threading;
using Dbosoft.Hosuto.Modules.Hosting;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SAPHub.MessageBus;
using SAPHub.StateDb;

namespace SAPHub
{
    class Program
    {
        public static void Main(string[] args)
        {
            //give started bus some time to start up
            Thread.Sleep(5000);

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            var services = new ServiceCollection()
                .AddTransportSelector()
                .AddStateDb();

            return ModulesHost.CreateDefaultBuilder(args)
                .UseServiceCollection(services)
                .UseAspNetCoreWithDefaults()
                .HostModule<ApiModule.ApiModule>()
                .ConfigureHostConfiguration(config => config
                    .AddEnvironmentVariables("SAPHUB_")
                    .AddUserSecrets<Program>()
                );
        }
    }
}
