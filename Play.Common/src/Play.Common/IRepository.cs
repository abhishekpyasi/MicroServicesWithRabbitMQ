using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;


namespace Play.Common
{
    public interface IRepository<T> where T : IEntity
    {
        public Task CreateAsync(T entity);
        public Task<IReadOnlyCollection<T>> GetAllAsync();
        public Task<IReadOnlyCollection<T>> GetAllAsync(Expression<Func<T, bool>> filter);
        public Task<T> GetItemAsync(Guid id);
        public Task<T> GetItemAsync(Expression<Func<T, bool>> filter);
        public Task RemoveAsync(Guid id);
        public Task UpdateAsync(T entity);
    }
}