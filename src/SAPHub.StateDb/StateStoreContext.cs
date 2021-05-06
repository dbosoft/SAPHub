using Microsoft.EntityFrameworkCore;
using SAPHub.StateDb.Model;

namespace SAPHub.StateDb
{
    public class StateStoreContext : DbContext
    {
        public StateStoreContext(DbContextOptions<StateStoreContext> options)
            : base(options)
        {
        }

        public DbSet<OperationModel> Operations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OperationModel>();

        }
    }
}