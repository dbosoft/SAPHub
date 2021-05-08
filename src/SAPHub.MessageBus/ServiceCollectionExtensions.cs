using Microsoft.Extensions.DependencyInjection;
using Rebus.Persistence.InMem;
using Rebus.Transport.InMem;
using SAPHub.Bus;

namespace SAPHub.MessageBus
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddTransportSelector(this IServiceCollection services)
        {
            services.AddSingleton(new InMemNetwork(true));
            services.AddSingleton(new InMemorySubscriberStore());
            services.AddSingleton<IRebusTransportConfigurer, RebusTransportSelector>();
            services.AddSingleton<IRebusSubscriptionConfigurer, InMemorySubscriptionConfigurer>();

            return services;
        }


    }
}