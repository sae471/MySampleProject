using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAE471
{
    public class ResultDTO<T>
    {
        public long TotalCount { get; set; }
        public IReadOnlyList<T> Items { get; set; }
    }
}