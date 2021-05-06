using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SAPHub.StateDb.Model;

namespace SAPHub.StateDb
{
    public class StateStoreRepository<T> : IStateStoreRepository<T> where T : class
    {
        private readonly StateStoreContext _dbContext;
        private readonly ISpecificationEvaluator<T> _specificationEvaluator;

        public StateStoreRepository(StateStoreContext dbContext)
        {
            _dbContext = dbContext;
            _specificationEvaluator = new SpecificationEvaluator<T>();
        }

        public async Task<T> AddAsync(T entity)
        {
            await _dbContext.Set<T>().AddAsync(entity);

            await SaveChangesAsync();

            return entity;
        }

        public async Task UpdateAsync(T entity)
        {
            _dbContext.Entry(entity).State = EntityState.Modified;

            await SaveChangesAsync();
        }

        public async Task DeleteAsync(T entity)
        {
            _dbContext.Set<T>().Remove(entity);

            await SaveChangesAsync();
        }

        public async Task DeleteRangeAsync(IEnumerable<T> entities)
        {
            _dbContext.Set<T>().RemoveRange(entities);

            await SaveChangesAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _dbContext.SaveChangesAsync();
        }

        public async Task<T> GetByIdAsync(int id)
        {
            return await _dbContext.Set<T>().FindAsync(id);
        }

        public async Task<T> GetByIdAsync<TId>(TId id)
        {
            return await _dbContext.Set<T>().FindAsync(id);

        }

        public async Task<T> GetBySpecAsync(ISpecification<T> specification)
        {
            return (await ListAsync(specification)).FirstOrDefault();
        }

        public async Task<TResult> GetBySpecAsync<TResult>(ISpecification<T, TResult> specification)
        {
            return (await ListAsync(specification)).FirstOrDefault();
        }

        public async Task<List<T>> ListAsync()
        {
            return await _dbContext.Set<T>().ToListAsync();
        }

        public async Task<List<T>> ListAsync(ISpecification<T> specification)
        {
            return await ApplySpecification(specification).ToListAsync();
        }

        public async Task<List<TResult>> ListAsync<TResult>(ISpecification<T, TResult> specification)
        {
            return await ApplySpecification(specification).ToListAsync();
        }

        public async Task<int> CountAsync(ISpecification<T> specification)
        {
            return await ApplySpecification(specification).CountAsync();
        }


        private IQueryable<T> ApplySpecification(ISpecification<T> specification)
        {
            return _specificationEvaluator.GetQuery(_dbContext.Set<T>().AsQueryable().AsNoTracking(), specification);
        }

        private IQueryable<TResult> ApplySpecification<TResult>(ISpecification<T, TResult> specification)
        {
            if (specification is null) throw new ArgumentNullException(nameof(specification));
            if (specification.Selector is null) throw new SelectorNotFoundException();

            return _specificationEvaluator.GetQuery(_dbContext.Set<T>().AsQueryable().AsNoTracking(), specification);
        }
    }
}