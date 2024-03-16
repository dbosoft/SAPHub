using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace SAPHub.StateDb;

internal class CosmosDbStateStoreContextConfigurer : IDbContextConfigurer<StateStoreContext>
{
    private readonly string _connectionstring;
    private readonly string _databaseName;

    public CosmosDbStateStoreContextConfigurer(IConfiguration configuration)
    {
        _connectionstring = configuration["cosmosdb:connectionstring"];

        if (_connectionstring == null)
            throw new InvalidOperationException("Missing configuration entry for cosmosdb::connectionstring.");

        _databaseName = configuration["cosmosdb:databaseName"];

        if (_databaseName == null)
            throw new InvalidOperationException("Missing configuration entry for cosmosdb::databaseName.");

    }

    public void Configure(DbContextOptionsBuilder options)
    {
        options.UseCosmos(_connectionstring, _databaseName);
    }
}