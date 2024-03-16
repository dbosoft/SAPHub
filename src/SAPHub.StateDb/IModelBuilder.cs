using Microsoft.EntityFrameworkCore;

namespace SAPHub.StateDb;

// ReSharper disable once UnusedTypeParameter
public interface IModelBuilder<T> where T: DbContext
{
    void ConfigureModel(ModelBuilder modelBuilder);
}