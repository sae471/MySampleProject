using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SAE471.Application.DTOs;
using SAE471.Application.Services;
using SAE471.Domain.Entities;

namespace SAE471
{
    [ApiController]
    [Route("api/[controller]")]
    public class BaseController<TEntity, TEntityDTO, TEntityUpsertDTO> : ControllerBase
    where TEntity : Entity<Guid>
        where TEntityDTO : EntityDTO<Guid>
        where TEntityUpsertDTO : EntityDTO<Guid>
    {
        private IBaseAppService<TEntity, TEntityDTO, TEntityUpsertDTO> _appService;
        public IBaseAppService<TEntity, TEntityDTO, TEntityUpsertDTO> AppService
        {
            get => _appService ?? (_appService =
                       LazyServiceProvider.LazyGetService<IBaseAppService<TEntity, TEntityDTO, TEntityUpsertDTO>>());
            set => _appService = value;
        }

        [HttpGet("{id:guid}")]
        public virtual async Task<IActionResult> Get(Guid id)
        {
            if (id == null || id == default(Guid))
            {
                return BadRequest();
            }
            var result = await AppService.GetAsync(id);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [HttpGet]
        public virtual async Task<IActionResult> GetListAsync([FromQuery] RequestDTO input)
        {
            if (input.MaxResultCount <= 1 || input.SkipCount < 0)
            {
                return BadRequest();
            }
            return Ok(await AppService.GetListAsync(input));
        }

        [HttpPost]
        public virtual async Task<IActionResult> Upsert(TEntityUpsertDTO input)
        {
            if (input.Id != default(Guid) && !AppService.GetQueryableAsync().Result.Any(it => it.Id == input.Id))
            {
                return NotFound();
            }
            return Ok(await AppService.UpsertAsync(input));
        }

        [HttpDelete("{id:guid}")]
        public virtual async Task<IActionResult> Delete(Guid id)
        {
            if (id == null || id == default(Guid))
            {
                return BadRequest();
            }
            if (!AppService.GetQueryableAsync().Result.Any(it => it.Id == id))
            {
                return NotFound();
            }
            return Ok(await AppService.DeleteAsync(id));
        }
    }
}