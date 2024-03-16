using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Rebus.Config;
using Rebus.DataBus;
using Rebus.DataBus.FileSystem;
using Rebus.DataBus.InMem;
using SAPHub.Bus;

namespace SAPHub.MessageBus;

public class RebusDataBusSelector : IRebusDataBusConfigurer
{
    private readonly string _busType;
    private readonly string _connectionString;
    private readonly string _path;
    private readonly string _containerName;
    private readonly InMemDataStore _dataStore;
    private ILogger _logger;


    public RebusDataBusSelector(IConfiguration configuration, InMemDataStore dataStore,
        ILogger<RebusDataBusSelector> logger)
    {
        _dataStore = dataStore;
        _logger = logger;

        _busType = configuration["databus:type"];
        _connectionString = configuration["databus:connectionstring"];
        _path = configuration["databus:path"];
        _containerName = configuration["databus:container"];

        if (_busType == null)
            throw new InvalidOperationException(
                "Missing configuration entry for databus::type. Configure a valid bus type (inmemory,filesystem,azureblobs");

        switch (_busType)
        {
            case "azureblobs":
                if (_connectionString == null)
                    throw new InvalidOperationException("Missing configuration entry for databus::connectionstring.");
                if (_containerName == null)
                    throw new InvalidOperationException("Missing configuration entry for databus::container.");
                return;

            case "filesystem":
                if (_path == null)
                    throw new InvalidOperationException("Missing configuration entry for databus::path.");
                return;

            case "inmemory": return;

        }

        throw new InvalidOperationException(
            $"Invalid bus type: {_busType} .Configure a valid bus type (inmemory,rabbitmq,azureservicebus).");

    }


    public void Configure(StandardConfigurer<IDataBusStorage> configurer)
    {
        switch (_busType)
        {
            case "azureblobs":
                configurer.StoreInBlobStorage(_connectionString, _containerName);
                break;
            case "filesystem":
                configurer.StoreInFileSystem(_path);
                break;
            case "inmemory":
                configurer.StoreInMemory(_dataStore);
                break;
        }
    }
}