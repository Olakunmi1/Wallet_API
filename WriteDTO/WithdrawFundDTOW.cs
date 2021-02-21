using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Wallet_API.WriteDTO
{
    public class WithdrawFundDTOW 
    {
        [Range(0.0, 1000000)]
        [Required]
        public decimal Amount { get; set; }

    }
}
