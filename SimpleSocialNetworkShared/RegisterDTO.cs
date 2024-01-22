using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSocialNetworkShared
{
    public class RegisterDTO
    {
        [Required]
        [EmailAddress]
        public string? Email { get; set; }   
        [Required]
        public string? Username { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 6)]
        public string? Password { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 6)]
        public string? ConfirmPassword { get; set; }
    }
}
