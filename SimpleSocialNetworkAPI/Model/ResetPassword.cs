using System.ComponentModel.DataAnnotations;

namespace SimpleSocialNetworkAPI.Model
{
    public class ResetPassword
    {
        [Required]
        public string? Token { get; set; }
        [Required]
        [EmailAddress]
        public string? Email { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 6)]
        public string? NewPassword { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 6)]
        public string? ConfirmPassword { get; set; }
    }
}
