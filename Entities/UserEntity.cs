using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ARoulete.Entities
{
    public class UserEntity
    {
        public string UserName { get; set; }
        public int? BetNumber { get; set; }
        public double CashToBet { get; set; }
        public string Colour { get; set; }
    }
}
