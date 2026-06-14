using System.ComponentModel.DataAnnotations;

namespace DoorsWeb.Shared.Models
{
    public class AdminLoginDto
    {
        [Required]
        public string Username { get; set; } = "";
        [Required]
        public string Password { get; set; } = "";
    }
}
