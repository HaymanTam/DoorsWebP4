using Mapster;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DoorsWeb.Shared.Models
{
    [Index(nameof(CardNumber), IsUnique = true)]
    [Index(nameof(Name), IsUnique = true)]
    public class User
    {
        [Key]
        public Guid Id { get; set; }
        public int CardNumber { get; set; }
        public string? Name { get; set; }
        public string? PhotoPath { get; set; }
        public bool IsEnabled { get; set; }
        public List<AccessLevel> AccessLevels { get; set; } = new List<AccessLevel>();
        public DateTime LastUpdatedAt { get; set; }

    }
}
