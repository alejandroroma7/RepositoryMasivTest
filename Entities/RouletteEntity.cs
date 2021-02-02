using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ARoulete.Entities;

namespace ARoulete.Entities
{
    public class RouletteEntity
    {
        public RouletteEntity()
        {
            this.Users = new List<UserEntity>();
        }

        public IList<UserEntity> Users { get; set; }
        public bool State { get; set; }
    }
}
