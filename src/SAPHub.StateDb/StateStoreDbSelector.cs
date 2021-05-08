using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SAPHub.StateDb
{
    internal class StateStoreDbSelector : IDbContextConfigurer<StateStoreContext>, IModelBuilder<StateStoreContext>
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly bool _cosmosDbEnabled;

        public StateStoreDbSelector(IConfiguration configuration, IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;

            _cosmosDbEnabled = configuration["cosmosdb:connectionstring"] != null;

        }

        public void Configure(DbContextOptionsBuilder options)
        {
            if (_cosmosDbEnabled)
                _serviceProvider.GetRequiredService<CosmosDbStateStoreContextConfigurer>().Configure(options);
            else
            {
                _serviceProvider.GetRequiredService<InMemoryStateStoreContextConfigurer>().Configure(options);
            }
        }

        public void ConfigureModel(ModelBuilder modelBuilder)
        {
            if (_cosmosDbEnabled)
                _serviceProvider.GetRequiredService<CosmosDbModelBuilder>().ConfigureModel(modelBuilder);
            else
            {
                _serviceProvider.GetRequiredService<SqlModelBuilder>().ConfigureModel(modelBuilder);
            }
        }
    }

}