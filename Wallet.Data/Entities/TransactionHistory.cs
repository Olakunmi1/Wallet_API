using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Wallet.Data.Entities
{
   public class TransactionHistory
    {
        [Key]
        public int Id { get; set; }
        public string Txn_type { get; set; }  //debit or credit

        //refactor this later to Enum 
        public string Purpose { get; set; } //Trasaction purpsoe (deposit, transfer, Reversal)
        public WalletAccount wallet { get; set; }  //foreingkey to WalletAccount
        public decimal Amount { get; set; } 
        public string reference { get; set; } //guid types

        public decimal balance_before { get; set; }
        public decimal balance_after { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }

    }
}
