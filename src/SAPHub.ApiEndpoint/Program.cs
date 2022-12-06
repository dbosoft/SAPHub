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
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            //setup shared ServiceProvider
            var services = new ServiceCollection()
                .AddRebusSelectors()  // chooses Rebus transport by configuration
                .AddStateDb();           // required for state store in API module 

            // create ModulesHosts with ASPNetCore enabled and host API module
            return ModulesHost.CreateDefaultBuilder(args)
                .UseServiceCollection(services)
                .UseAspNetCoreWithDefaults((m, hb) => hb.UseUrls(m.Path))
                .HostModule<ApiModule.ApiModule>()
                .ConfigureAppConfiguration(config => config
                    .AddEnvironmentVariables("SAPHUB_"));

        }
    }
}
