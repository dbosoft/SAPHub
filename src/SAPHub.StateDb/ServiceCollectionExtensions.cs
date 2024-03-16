using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace SAPHub.StateDb;

public static class ServiceCollectionExtensions
{

    public static IServiceCollection AddStateDb(this IServiceCollection services)
    {
        services.AddSingleton(new InMemoryDatabaseRoot());
        services.AddSingleton<IDbContextConfigurer<StateStoreContext>, StateStoreDbSelector>();
        services.AddSingleton<CosmosDbStateStoreContextConfigurer>();
        services.AddSingleton<InMemoryStateStoreContextConfigurer>();
        services.AddSingleton<IModelBuilder<StateStoreContext>, StateStoreDbSelector>();
        services.AddSingleton<CosmosDbModelBuilder>();
        services.AddSingleton<SqlModelBuilder>();

        return services;
    }

}