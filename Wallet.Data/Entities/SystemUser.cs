using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Wallet.Data.Entities
{
    public class SystemUser
    {
        [Key]
        public int ID { get; set; }
        public string Username { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime Created_at { get; set; }
    }
}
