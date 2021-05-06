using Dbosoft.Hosuto.Modules.Hosting;
using Microsoft.Extensions.Hosting;

namespace SAPHub
{
    class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            ModulesHost.CreateDefaultBuilder(args)
                .UseAspNetCoreWithDefaults((module, webHostBuilder) =>
                {
                })
                .HostModule<ApiModule.ApiModule>();

    }
}
