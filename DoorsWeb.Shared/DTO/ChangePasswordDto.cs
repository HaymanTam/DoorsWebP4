using System.ComponentModel.DataAnnotations;
using DoorsWeb.Shared.Auth;

namespace DoorsWeb.Shared.Models
{
    public class ChangePasswordDto
    {
        [Required]
        [MinLength(PasswordPolicy.MinLength, ErrorMessage = "Password must be at least 12 characters.")]
        public string NewPassword { get; set; } = "";
    }
}
