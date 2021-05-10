using System;
using System.Threading.Tasks;
using Dbosoft.Hosuto.HostedServices;
using Dbosoft.Hosuto.Modules;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Rebus.Handlers;
using Rebus.Persistence.InMem;
using Rebus.Retry.FailFast;
using Rebus.Retry.Simple;
using Rebus.Serialization.Json;
using Rebus.ServiceProvider;
using SAPHub.Bus;
using SAPHub.Connector.CommandHandlers;

namespace SAPHub.Connector
{
    public class SAPConnectorModule : IModule
    {
        public string Name => nameof(SAPConnectorModule);


        public void ConfigureServices(IServiceProvider sp, IServiceCollection services, IConfiguration configuration)
        {
            services.AddYaNco();

            services.AddRebus(configurer =>
            {
                return configurer
                    .Transport(t =>
                        sp.GetRequiredService<IRebusTransportConfigurer>().Configure(t, QueueNames.SAPConnector))
                    
                    .Options(x =>
                    {
                        x.SimpleRetryStrategy(maxDeliveryAttempts:1,
                            secondLevelRetriesEnabled: true);
                        //x.FailFastOn<InvalidOperationException>(e => true);
                        
                        x.SetNumberOfWorkers(2);
                    })
                    .Subscriptions(s => sp.GetRequiredService<IRebusSubscriptionConfigurer>().Configure(s))
                    .Serialization(x => x.UseNewtonsoftJson(new JsonSerializerSettings
                        {TypeNameHandling = TypeNameHandling.None}))
                    .Timeouts(cfg => cfg.StoreInMemory())
                    .Logging(x => x.ColoredConsole());
            });


            services.AddRebusHandler<GetCompaniesCommandHandler>();
            services.AddRebusHandler<GetCompanyCommandHandler>();

            services.AddTransient(typeof(IHandleMessages<>), typeof(FailedMessageHandler<>));


            services.AddHostedHandler((s, c) =>
            {
                s.UseRebus();
                return Task.CompletedTask;
            });

        }


    }
}