using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAE471.Application.DTOs
{
    public abstract class EntityDTO<TKey> : IEntityDTO<TKey>
    {
        protected EntityDTO(){}

        public TKey Id { get; set; }
    }
}