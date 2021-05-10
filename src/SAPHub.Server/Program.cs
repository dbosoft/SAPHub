using System.Collections.Generic;
using System.Threading.Tasks;
using Dbosoft.Hosuto.Modules.Hosting;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SAPHub.Connector;
using SAPHub.MessageBus;
using SAPHub.StateDb;

namespace SAPHub
{
    class Program
    {
        public static Task Main(string[] args)
        {
            RfcLibraryHelper.EnsurePathVariable();

           return CreateHostBuilder(args).RunConsoleAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            var services = new ServiceCollection()
                .AddTransportSelector()
                .AddStateDb();

            var staticSettings = new Dictionary<string,string>
            {
                ["bus:type"] ="inmemory"
            };

            return ModulesHost.CreateDefaultBuilder(args)
                .UseServiceCollection(services)
                .UseAspNetCoreWithDefaults((module, webHostBuilder) =>
                {
#pragma warning disable CA1416
                    webHostBuilder.UseHttpSys(options => { options.UrlPrefixes.Add(module.Path); })
                        .UseUrls(module.Path);
#pragma warning restore CA1416
                })
                .HostModule<ApiModule.ApiModule>()
                .HostModule<UI.UIModule>()
                .AddHostAssets<UI.UIModule>()
                .HostModule<SAPConnectorModule>()
                .ConfigureAppConfiguration(config => config
                    .AddInMemoryCollection(staticSettings)
                    .AddEnvironmentVariables("SAPHUB_")
                ).ConfigureLogging(l=>
                {
                    l.SetMinimumLevel(LogLevel.Trace);
                });
        }
    }
}
