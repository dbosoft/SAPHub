using Microsoft.EntityFrameworkCore;

namespace SAPHub.StateDb
{
    public interface IDbContextConfigurer<TContext>
    {
        void Configure(DbContextOptionsBuilder options);
    }


}