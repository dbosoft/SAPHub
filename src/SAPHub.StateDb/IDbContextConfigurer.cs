using Microsoft.EntityFrameworkCore;

namespace SAPHub.StateDb;

// ReSharper disable once UnusedTypeParameter
public interface IDbContextConfigurer<TContext>
{
    void Configure(DbContextOptionsBuilder options);
}