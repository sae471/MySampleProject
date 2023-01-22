using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SAE471.Application.DTOs;
using SAE471.Domain.Entities;
using SAE471.Domain.Repositories;

namespace SAE471.Application.Services
{
    public interface IBaseAppService<TEntity, TEntityDTO, TEntityUpsertDTO>
        where TEntity : Entity<Guid>
        where TEntityDTO : EntityDTO<Guid>
        where TEntityUpsertDTO : EntityDTO<Guid>
    {

        Task<IQueryable<TEntity>> GetQueryableAsync();
        Task<TEntityDTO> UpsertAsync(TEntityUpsertDTO input);

        Task<TEntityDTO> GetAsync(Guid id);
        Task<ResultDTO<TEntityDTO>> GetListAsync(RequestDTO input);

        Task<int> DeleteAsync(Guid id);
    }
}