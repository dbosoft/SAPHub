using System;
using Microsoft.Extensions.Configuration;
using Rebus.Config;
using Rebus.Persistence.InMem;
using Rebus.Subscriptions;
using SAPHub.Bus;

namespace SAPHub.MessageBus;

public class SubscriptionStoreSelector : IRebusSubscriptionConfigurer
{
    private readonly InMemorySubscriberStore _store;
    private readonly string _busType;
    private readonly string _connectionString;
    public SubscriptionStoreSelector(InMemorySubscriberStore store, IConfiguration configuration)
    {
        _store = store;

        _busType = configuration["bus:type"];
        _connectionString = configuration["bus:connectionstring"]; 

        if (_busType == null)
            throw new InvalidOperationException("Missing configuration entry for bus::type. Configure a valid bus type (inmemory,rabbitmq,azurestorage,azureservicebus");

        switch (_busType)
        {
            case "azurestorage":
            case "azureservicebus":
            case "rabbitmq":
            case "inmemory": return;

        }

    }

    public void Configure(StandardConfigurer<ISubscriptionStorage> subscriberStore)
    {
        switch (_busType)
        {
            case "inmemory":
                // since rebus 8.2.2 , the inmemory subscription storage is registered with the inmemory transport
                //subscriberStore.StoreInMemory(_store);
                break;
            case "azurestorage":
                subscriberStore.StoreInAzureTables(_connectionString, automaticallyCreateTable: true);
                break;
        }
    }
}