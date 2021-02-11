using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Wallet_API.WriteDTO
{
    public class WalletAccountDTOW
    {
        [Required]
        public string Name { get; set; }

       // public int userID { get; set; }
    }
}
