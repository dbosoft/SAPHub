using Dbosoft.Hosuto.Modules.Hosting;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

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
            
            // create ModulesHosts and host SAPConnector module
            return ModulesHost.CreateDefaultBuilder(args)
                .UseAspNetCoreWithDefaults((m, hb) => hb.UseUrls(m.Path))
                .HostModule<UIModule>()
                .ConfigureHostConfiguration(config => config
                    .AddEnvironmentVariables("SAPHUB_"));
        }
    }
}
