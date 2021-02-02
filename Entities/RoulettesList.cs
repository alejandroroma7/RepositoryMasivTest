using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ARoulete.Entities
{
    public class RoulettesList
    {
        public RoulettesList()
        {
            List = new List<ListEntity>();
        }
        public IList<ListEntity> List { get; set; }
        
    }
}
