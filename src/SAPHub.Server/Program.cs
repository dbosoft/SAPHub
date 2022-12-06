using System.Collections.Generic;
using System.Threading.Tasks;
using Dbosoft.Hosuto.Modules.Hosting;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SAPHub.ConnectorModule;
using SAPHub.MessageBus;
using SAPHub.StateDb;

namespace SAPHub
{
    class Program
    {
        public static Task Main(string[] args)
        {
            //this makes sure that UCI libraries can be found by
            //sapnwrfc. It is only required for some platforms.
            RfcLibraryHelper.EnsurePathVariable();

           return CreateHostBuilder(args).RunConsoleAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            //setup shared ServiceProvider
            var services = new ServiceCollection()
                .AddRebusSelectors()  // chooses Rebus transport by configuration
                .AddStateDb();           // required for state store in API module

            // set default Rebus transport to inmemory
            var staticSettings = new Dictionary<string,string>
            {
                ["bus:type"] ="inmemory",
                ["databus:type"] ="inmemory"
            };

            // create ModulesHosts and host SAPConnector module
            return ModulesHost.CreateDefaultBuilder(args)
                .UseServiceCollection(services)
                .UseAspNetCoreWithDefaults((module, webHostBuilder) =>
                {
                    //we will use HttpSys instead of kestrel to use port sharing
                    webHostBuilder.UseHttpSys(options => { options.UrlPrefixes.Add(module.Path); })
                        .UseUrls(module.Path);
                })
                .HostModule<ApiModule.ApiModule>()
                .HostModule<UI.UIModule>()
                .HostModule<SAPConnectorModule>()
                .ConfigureAppConfiguration(config => config
                    .AddInMemoryCollection(staticSettings)
                    .AddEnvironmentVariables("SAPHUB_"));
        }
    }
}
