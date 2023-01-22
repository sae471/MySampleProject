using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SAE471.Domain.Entities;
using SAE471.Domain.Repositories;

namespace SAE471.Domain.Services
{
    public abstract class DomainService<TEntity> : IDomainService<TEntity> where TEntity : Entity<Guid>
    {
        public IRepository<TEntity> Repository => LazyServiceProvider.LazyGetService<IRepository<TEntity>>();
    }
}