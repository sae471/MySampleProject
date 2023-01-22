using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAE471
{
    public class KeyValueDTO<TPrimaryKey>
    {
        public TPrimaryKey Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public TPrimaryKey Data { get { return this.Id; } }
        public string Label { get { return this.Name; } }
        public object ExtraProperties { get; set; }
        public object Item
        {
            get
            {
                return new
                {
                    ExtraProperties = this.ExtraProperties
                };
            }
        }
    }
}