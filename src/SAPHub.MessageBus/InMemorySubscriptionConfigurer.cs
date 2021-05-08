using Microsoft.Extensions.Configuration;
using Rebus.Config;
using Rebus.Persistence.InMem;
using Rebus.Subscriptions;
using SAPHub.Bus;

namespace SAPHub.MessageBus
{
    public class InMemorySubscriptionConfigurer : IRebusSubscriptionConfigurer
    {
        private readonly IConfiguration _configuration;
        private readonly InMemorySubscriberStore _store;

        public InMemorySubscriptionConfigurer(InMemorySubscriberStore store, IConfiguration configuration)
        {
            _store = store;
            _configuration = configuration;

        }

        public void Configure(StandardConfigurer<ISubscriptionStorage> subscriberStore)
        {
            var busType = _configuration["bus:type"];
            if(busType== "inmemory")
                subscriberStore.StoreInMemory(_store);
        }
    }
}