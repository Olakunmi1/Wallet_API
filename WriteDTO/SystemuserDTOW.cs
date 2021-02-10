using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Wallet_API.WriteDTO
{
    public class SystemuserDTOW
    {
        [Required]
        public string firstname { get; set; }
        [Required]
        public string lastName { get; set; }

        [Required]
        [Phone]
        public string phonenumber { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("password", ErrorMessage = "The password and confirmation password do not match.")]
        public string confirmpassword { get; set; }

        [Required]
        [RegularExpression(@"[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,4}", ErrorMessage = "Please enter Valid Email ID")]
        [EmailAddress]
        public string email { get; set; }

        [Required]
        [Display(Name = "Username")]
        public string username { get; set; }
    }
}
