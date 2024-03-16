using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace SAPHub.StateDb;

internal class InMemoryStateStoreContextConfigurer : IDbContextConfigurer<StateStoreContext>
{
    private readonly InMemoryDatabaseRoot _dbRoot;

    public InMemoryStateStoreContextConfigurer(InMemoryDatabaseRoot dbRoot, IModelBuilder<StateStoreContext> modelBuilder)
    {
        _dbRoot = dbRoot;
    }

    public void Configure(DbContextOptionsBuilder options)
    {
        options.UseInMemoryDatabase("StateDb", _dbRoot);
    }
}