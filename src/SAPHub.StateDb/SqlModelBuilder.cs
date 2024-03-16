using Microsoft.EntityFrameworkCore;
using SAPHub.StateDb.Model;

namespace SAPHub.StateDb;

public class SqlModelBuilder : IModelBuilder<StateStoreContext>
{
    public void ConfigureModel(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<OperationModel>()
            .Property(x => x.Timestamp)
            .IsRowVersion();

        modelBuilder.Entity<OperationModel>()
            .Property(x => x.Status)
            .IsConcurrencyToken();

        modelBuilder.Entity<OperationModel>();

    }
}