using Ardalis.Specification.EntityFrameworkCore;

namespace SAPHub.StateDb;

public class StateStoreRepository<T> : RepositoryBase<T>, IStateStoreRepository<T> where T : class
{
    // ReSharper disable once SuggestBaseTypeForParameterInConstructor
    public StateStoreRepository(StateStoreContext dbContext) : base(dbContext)
    {
    }

}