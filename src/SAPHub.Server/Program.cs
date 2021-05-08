using System.Collections.Generic;
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
        const string BasePath = "http://localhost:62089/";


        public static void Main(string[] args)
        {
            RfcLibraryHelper.EnsurePathVariable();

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            var services = new ServiceCollection()
                .AddTransportSelector()
                .AddStateDb();

            var busSettings = new Dictionary<string,string>{ ["bus:type"] ="inmemory"};

            return ModulesHost.CreateDefaultBuilder(args)
                .UseServiceCollection(services)
                .UseAspNetCoreWithDefaults((module, webHostBuilder) =>
                {
                    webHostBuilder.UseStaticWebAssets();
#pragma warning disable CA1416
                    webHostBuilder.UseHttpSys(options => { options.UrlPrefixes.Add($"{BasePath}{module.Path}"); })
                        .UseUrls($"{BasePath}{module.Path}");
#pragma warning restore CA1416
                })
                .HostModule<ApiModule.ApiModule>()
                .HostModule<UI.UIModule>()
                .HostModule<SAPConnectorModule>()
                .ConfigureHostConfiguration(config => config
                    .AddInMemoryCollection(busSettings)
                    .AddEnvironmentVariables("SAPHUB_")
                    .AddUserSecrets<Program>()
                );
        }
    }
}
