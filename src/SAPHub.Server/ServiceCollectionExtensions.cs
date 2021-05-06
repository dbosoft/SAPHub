using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Rebus.Persistence.InMem;
using Rebus.Transport.InMem;
using SAPHub.StateDb;

namespace SAPHub
{
    internal static class ServiceCollectionExtensions
    {
        public static IServiceCollection UseInMemoryBus(this IServiceCollection services)
        {
            services.AddSingleton(new InMemNetwork(true));
            services.AddSingleton(new InMemorySubscriberStore());
            services.AddSingleton<IRebusTransportConfigurer, InMemoryTransportConfigurer>();
            services.AddSingleton(new InMemorySubscriberStore());
            services.AddSingleton<IRebusSubscriptionConfigurer, InMemorySubscriptionConfigurer>();

            return services;
        }

        public static IServiceCollection UseInMemoryDb(this IServiceCollection services)
        {
            services.AddSingleton(new InMemoryDatabaseRoot());
            services.AddSingleton<IDbContextConfigurer<StateStoreContext>, InMemoryStateStoreContextConfigurer>();
            return services;
        }

    }
}