using Microsoft.EntityFrameworkCore;
using SAPHub.StateDb.Model;

namespace SAPHub.StateDb;

public class StateStoreContext : DbContext
{
    private readonly IModelBuilder<StateStoreContext> _modelBuilder;

    public StateStoreContext(DbContextOptions<StateStoreContext> options, IModelBuilder<StateStoreContext> modelBuilder)
        : base(options)
    {
        _modelBuilder = modelBuilder;
    }

    public DbSet<OperationModel> Operations { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        _modelBuilder.ConfigureModel(modelBuilder);
    }
}