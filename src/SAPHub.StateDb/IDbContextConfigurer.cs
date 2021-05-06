using Microsoft.EntityFrameworkCore;

namespace SAPHub.StateDb
{
    public interface IDbContextConfigurer<TContext> where TContext : DbContext
    {
        void Configure(DbContextOptionsBuilder options);
    }


}