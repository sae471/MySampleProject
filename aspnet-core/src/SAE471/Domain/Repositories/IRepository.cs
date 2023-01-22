using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SAE471.Domain.Entities;

namespace SAE471.Domain.Repositories
{
    public interface IRepository<TEntity> where TEntity : IEntity<Guid>
    {
        Task<TEntity> FindAsync(Guid id);
        Task<TEntity> InsertAsync(TEntity entity);
        Task<TEntity> UpdateAsync(TEntity entity);
        Task<int> DeleteAsync(Guid id);
        Task<IQueryable<TEntity>> GetQueryableAsync();
    }
}