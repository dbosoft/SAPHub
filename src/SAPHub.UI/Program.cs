using Dbosoft.Hosuto.Modules.Hosting;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace SAPHub.UI
{
    class Program
    {
        static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }


        public static IHostBuilder CreateHostBuilder(string[] args)
        {

            return ModulesHost.CreateDefaultBuilder(args)

                .UseAspNetCoreWithDefaults((m, hb) =>
                    hb.UseUrls(m.Path))
                .HostModule<UIModule>()
                .AddHostAssets<UIModule>()
                .ConfigureHostConfiguration(config => config
                    .AddEnvironmentVariables("SAPHUB_")
                ).ConfigureLogging(c=>c.SetMinimumLevel(LogLevel.Trace));
        }
    }
}
