using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SAE471.Domain.Entities;

namespace SAE471.Domain.Services
{
    public interface IDomainService<TEntity>  where TEntity : Entity<Guid>
    {

    }
}