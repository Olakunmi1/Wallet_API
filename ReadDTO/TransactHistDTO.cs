using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wallet_API.ReadDTO
{
    public class TransactHistDTO
    {
        public string Txn_type { get; set; }  //debit or credit
        public string Purpose { get; set; } //Trasaction purpsoe (deposit, transfer, Reversal
        public decimal balance_before { get; set; }
        public decimal balance_after { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
    }
}
