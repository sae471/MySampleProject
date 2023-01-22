
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SAE471.Domain.Entities;

namespace SAE471.Domain.Repositories
{
    public abstract class Repository<TEntity> : IRepository<TEntity>
        where TEntity : Entity<Guid>
    {
        private DbContext _context => LazyServiceProvider.LazyGetService<DbContext>();
        public virtual Task<int> DeleteAsync(Guid id)
        {
            var entity = _context.Set<TEntity>().SingleOrDefault(it => it.Id == id);
            if (entity == null)
            {
                throw new Exception("the entity not be found!!");
            }
            _context.Remove(entity);

            return Task.Run(() =>
            {
                return _context.SaveChangesAsync();
            });

            // return Task.Run(()=>{ await _context.SaveChangesAsync()})
        }

        public virtual async Task<TEntity> FindAsync(Guid id)
        {
            return await _context.Set<TEntity>().FirstOrDefaultAsync(it => it.Id == id);
        }

        public Task<IQueryable<TEntity>> GetQueryableAsync()
        {
            return Task.Run(() =>
            {
                return _context.Set<TEntity>().AsQueryable();
            });
        }

        public virtual async Task<TEntity> InsertAsync(TEntity entity)
        {
            var result = await _context.Set<TEntity>().AddAsync(entity);
            await _context.SaveChangesAsync();
            return result.Entity;
        }

        public virtual async Task<TEntity> UpdateAsync(TEntity entity)
        {
            var _entity = _context.Set<TEntity>().First(it => it.Id == entity.Id);
            _entity = entity;
            await _context.SaveChangesAsync();
            return _entity;

        }
    }
}