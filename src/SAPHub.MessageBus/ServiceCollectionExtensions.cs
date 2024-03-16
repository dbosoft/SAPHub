using Microsoft.Extensions.DependencyInjection;
using Rebus.DataBus.InMem;
using Rebus.Persistence.InMem;
using Rebus.Transport.InMem;
using SAPHub.Bus;

namespace SAPHub.MessageBus;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRebusSelectors(this IServiceCollection services)
    {
        services.AddSingleton(new InMemNetwork(true));
        services.AddSingleton(new InMemorySubscriberStore());
        services.AddSingleton<IRebusTransportConfigurer, RebusTransportSelector>();
        services.AddSingleton<IRebusSubscriptionConfigurer, SubscriptionStoreSelector>();
        services.AddSingleton<IRebusTimeoutConfigurer, RebusTimeoutSelector>();
            
        services.AddSingleton(new InMemDataStore());
        services.AddSingleton<IRebusDataBusConfigurer, RebusDataBusSelector>();
            
        return services;
    }


}