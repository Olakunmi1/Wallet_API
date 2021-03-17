using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Wallet.Data.Entities
{
   public class WalletAccount
    {

        [Key]
        public int ID { get; set; }
        public string Name { get; set; }
        public SystemUser user { get; set; } //foreingkey to SystemUser
        //public int userId { get; set; } 
        public decimal Balance { get; set; }
        public DateTime Created_at { get; set; }

    }
}
