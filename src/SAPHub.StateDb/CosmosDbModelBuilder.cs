using Microsoft.EntityFrameworkCore;
using SAPHub.StateDb.Model;

namespace SAPHub.StateDb;

internal class CosmosDbModelBuilder : IModelBuilder<StateStoreContext>
{
    public void ConfigureModel(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<OperationModel>().UseETagConcurrency();

    }
}