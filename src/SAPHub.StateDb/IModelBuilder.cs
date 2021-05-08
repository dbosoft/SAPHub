using Microsoft.EntityFrameworkCore;

namespace SAPHub.StateDb
{
    public interface IModelBuilder<T> where T: DbContext
    {
        void ConfigureModel(ModelBuilder modelBuilder);
    }
}