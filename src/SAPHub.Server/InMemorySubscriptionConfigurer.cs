﻿using Rebus.Config;
using Rebus.Persistence.InMem;
using Rebus.Subscriptions;

namespace SAPHub
{
    public class InMemorySubscriptionConfigurer : IRebusSubscriptionConfigurer
    {

        private readonly InMemorySubscriberStore _store;

        public InMemorySubscriptionConfigurer(InMemorySubscriberStore store)
        {
            _store = store;
        }

        public void Configure(StandardConfigurer<ISubscriptionStorage> subscriberStore)
        {
            subscriberStore.StoreInMemory(_store);
        }
    }
}