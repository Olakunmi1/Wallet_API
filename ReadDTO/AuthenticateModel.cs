using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Wallet_API.ReadDTO
{
    public class AuthenticateModel
    {
        [Required]
        [RegularExpression(@"[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,4}", ErrorMessage = "Please enter Valid Email ID")]
        [EmailAddress]
        public string email { get; set; }
        
        [Required]
        public string Password { get; set; }
    }
}
