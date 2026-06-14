using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DoorsWeb.Shared.Models
{
    [Index(nameof(Username), IsUnique = true)]
    public class Admin
    {
        [Key]
        public Guid Id { get; set; }
        [Required(ErrorMessage = "Username is required.")]
        [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Username must not contain symbols.")]
        [MinLength(4, ErrorMessage = "Username must have 4-16 characters.")]
        [MaxLength(16, ErrorMessage = "Username must have 4-16 characters.")]
        public required string Username { get; set; }
        [Required(ErrorMessage = "Password is required.")]
        public required string PasswordHash { get; set; }
        public required bool CanEditUsers { get; set; }
        public required bool CanEditDoors { get; set; }
        public required bool CanEditAdmins { get; set; }
        public DateTime LastUpdatedAt { get; set; }
    }
}
