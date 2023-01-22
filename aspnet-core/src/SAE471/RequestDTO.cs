using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAE471
{
    public class RequestDTO
    {
        // public RequestDTO()
        // {
        //     MaxResultCount = int.MaxValue;
        // }

        public string? Filtering { get; set; }
        public string? Fields { get; set; }
        public string? LookupStringFormat { get; set; }
        public string? SearchedFields { get; set; }
        public string? SearchedText { get; set; }
        public string? Sorting { get; set; }
        public int SkipCount { get; set; }
        public int MaxResultCount { get; set; }
    }
}