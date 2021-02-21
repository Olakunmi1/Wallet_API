using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Wallet_API.WriteDTO
{
    public class TransferFundDTOW
    {
        [Range(0.0, 1000000)]
        [Required]
        public decimal Amount { get; set; }

        [Required]
        public string WalletName { get; set; }

        [Required]
        public string Narration { get; set; }
    }

}
