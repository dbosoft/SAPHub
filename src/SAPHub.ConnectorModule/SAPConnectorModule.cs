using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Dbosoft.Hosuto.HostedServices;
using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Rebus.Config;
using Rebus.Handlers;
using Rebus.Retry.Simple;
using Rebus.Routing.TypeBased;
using Rebus.Serialization.Json;
using SAPHub.Bus;
using SAPHub.ConnectorModule.CommandHandlers;
using SAPHub.Messages;

namespace SAPHub.ConnectorModule
{
    /// <summary>
    /// SAP Connector Module
    /// </summary>
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class SAPConnectorModule
    {

        public void ConfigureServices(IServiceProvider sp, IServiceCollection services)
        {
            //add YaNco to services
            services.AddYaNco();

            //add rebus message queue with Transport configured by App
            services.AddRebus(configurer =>
            {
                return configurer
                    .Routing(x=> x.TypeBased().Map<OperationStatusEvent>(QueueNames.Api))
                    .Transport(t =>
                        sp.GetRequiredService<IRebusTransportConfigurer>().Configure(t, QueueNames.SAPConnector))

                    .Options(x =>
                    {
                        x.RetryStrategy(maxDeliveryAttempts: 1,
                            secondLevelRetriesEnabled: true);

                        x.SetNumberOfWorkers(2); // restrict to 2 workers for each Instance
                    })
                    .DataBus(ds => sp.GetRequiredService<IRebusDataBusConfigurer>()
                        .Configure(ds)
                    )
                    .Subscriptions(s => sp.GetRequiredService<IRebusSubscriptionConfigurer>().Configure(s))
                    .Serialization(x => x.UseNewtonsoftJson(new JsonSerializerSettings
                        { TypeNameHandling = TypeNameHandling.None }))
                    .Timeouts(cfg => sp.GetRequiredService<IRebusTimeoutConfigurer>().Configure(cfg))
                    .Logging(x => x.ColoredConsole());
            });

            //this could also be automated by type lookup, but as we need only 2 handlers, keep it simple...
            services.AddRebusHandler<GetCompanyCodesCommandHandler>();
            services.AddRebusHandler<GetCompanyCodeCommandHandler>();

            //automatic defer and requeue of failed messages
            //this currently breaks on Linux, so disable it for now on this platform

            if(!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                services.AddTransient(typeof(IHandleMessages<>), typeof(FailedMessageHandler<>));

            //finally add our handler - that is just the Rebus message loop...
            services.AddHostedHandler((s, c) => Task.CompletedTask);

        }


    }
}