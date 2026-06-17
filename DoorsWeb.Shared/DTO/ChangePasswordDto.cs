using System.ComponentModel.DataAnnotations;

namespace DoorsWeb.Shared.Models
{
    public class ChangePasswordDto
    {
        [Required]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters.")]
        public string NewPassword { get; set; } = "";
    }
}
