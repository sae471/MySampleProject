using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SAE471.Application.DTOs;
using SAE471.Domain.Entities;
using SAE471.Domain.Repositories;

namespace SAE471.Application.Services
{
    public abstract class BaseAppService<TEntity, TEntityDTO, TEntityUpsertDTO> : IBaseAppService<TEntity, TEntityDTO, TEntityUpsertDTO>
        where TEntity : Entity<Guid>
        where TEntityDTO : EntityDTO<Guid>
        where TEntityUpsertDTO : EntityDTO<Guid>
    {
        public IRepository<TEntity> Repository => LazyServiceProvider.LazyGetService<IRepository<TEntity>>();
        public IMapper Mapper => LazyServiceProvider.LazyGetService<IMapper>();

        public virtual async Task<int> DeleteAsync(Guid id)
        {
            return await Repository.DeleteAsync(id);
        }

        public virtual async Task<TEntityDTO> GetAsync(Guid id)
        {
            var entity = await Repository.FindAsync(id);
            return Mapper.Map<TEntity, TEntityDTO>(entity);
        }

        public virtual Task<ResultDTO<TEntityDTO>> GetListAsync(RequestDTO input)
        {
            return Repository.GetQueryableAsync().Result.ToResultDTO<TEntity, TEntityDTO>(input);
        }

        public virtual async Task<TEntityDTO> UpsertAsync(TEntityUpsertDTO input)
        {
            var entity = await Repository.FindAsync(input.Id);
            if (entity == null)
            {
                entity = Mapper.Map<TEntityUpsertDTO, TEntity>(input);
                return Mapper.Map<TEntity, TEntityDTO>(await Repository.InsertAsync(entity));
            }

            entity = Mapper.Map<TEntityUpsertDTO, TEntity>(input);
            return Mapper.Map<TEntity, TEntityDTO>(await Repository.UpdateAsync(entity));
        }

        public Task<IQueryable<TEntity>> GetQueryableAsync()
        {
            return Repository.GetQueryableAsync();
        }
    }
}