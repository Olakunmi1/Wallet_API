using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wallet_API.ReadDTO
{
    public class WithdrawFundDTO 
    {
        public string Beneficiary { get; set; }
        public string narration { get; set; }
        public string TransactionType { get; set; }  
        public decimal balance_before { get; set; } 
        public decimal balance_after { get; set; }
    }
}
