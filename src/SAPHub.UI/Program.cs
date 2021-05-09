using System;
using Dbosoft.Hosuto.Modules.Hosting;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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

            return ModulesHost.CreateDefaultBuilder(args)
                .UseAspNetCoreWithDefaults( (_, hb)=> hb.UseStaticWebAssets())
                .HostModule<UIModule>()
                .ConfigureHostConfiguration(config => config
                    .AddEnvironmentVariables("SAPHUB_")
                    .AddUserSecrets<Program>()
                );
        }
    }
}
