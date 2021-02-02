using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace ARoulete.Domain.DTO
{
    public class BetDTO
    {
        [Range(0, 10000)]
        public int CashToBet { get; set; }

        [Range(0, 36)]
        public int BetNumber { get; set; }

        public string Colour { get; set; }
    }
}
