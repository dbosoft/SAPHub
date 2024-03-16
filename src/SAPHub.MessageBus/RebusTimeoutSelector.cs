using System;
using Microsoft.Extensions.Configuration;
using Rebus.Config;
using Rebus.Persistence.InMem;
using Rebus.Timeouts;
using SAPHub.Bus;

namespace SAPHub.MessageBus;

public class RebusTimeoutSelector : IRebusTimeoutConfigurer
{
    private readonly string _busType;

    public RebusTimeoutSelector(IConfiguration configuration)
    {

        _busType = configuration["bus:type"];
            
        switch (_busType)
        {
            case "azurestorage":
            case "azureservicebus":
            case "rabbitmq":
            case "inmemory": return;

        }

        throw new InvalidOperationException($"Invalid bus type: {_busType} .Configure a valid bus type (inmemory,rabbitmq,azurestorage,azureservicebus).");

    }

         
    public void Configure(StandardConfigurer<ITimeoutManager> configurer)
    {
        switch (_busType)
        {
            case "azurestorage":
            case "azureservicebus":
            case "rabbitmq":
                return;  // these all have native timeout support
            case "inmemory": 
                configurer.StoreInMemory();
                return;
        }
    }
}