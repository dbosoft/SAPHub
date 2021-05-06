using Dbosoft.Hosuto.Modules.Hosting;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SAPHub.Connector;

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
                .UseInMemoryBus()
                .UseInMemoryDb();

            return ModulesHost.CreateDefaultBuilder(args)
                .UseServiceCollection(services)
                .UseAspNetCoreWithDefaults((module, webHostBuilder) =>
                {
#pragma warning disable CA1416
                    webHostBuilder.UseHttpSys(options => { options.UrlPrefixes.Add($"{BasePath}{module.Path}"); })
                        .UseUrls($"{BasePath}{module.Path}");
#pragma warning restore CA1416
                })
                .HostModule<ApiModule.ApiModule>()
                .HostModule<SAPConnectorModule>()
                .ConfigureHostConfiguration(c => c.AddUserSecrets<Program>())
                .ConfigureLogging(l => l.AddDebug());

        }
    }
}
