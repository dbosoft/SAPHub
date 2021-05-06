using Ardalis.Specification;

namespace SAPHub.StateDb
{
    public interface IStateStoreRepository<T> : IRepositoryBase<T> where T: class
    {

    }
}