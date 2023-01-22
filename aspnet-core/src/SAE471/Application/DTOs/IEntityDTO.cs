using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAE471.Application.DTOs
{
    public interface IEntityDTO<TKey>
    {
        TKey Id { get; set; }
    }
}