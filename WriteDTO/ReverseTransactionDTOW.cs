using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Wallet_API.WriteDTO
{
    public class ReverseTransactionDTOW
    {
        [Required]
        public string Reference { get; set; } //unique transaction Reference 
    }
}
