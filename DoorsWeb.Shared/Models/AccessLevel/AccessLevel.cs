using Mapster;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DoorsWeb.Shared.Models
{
    [Index(nameof(Name), IsUnique = true)]
    public class AccessLevel
    {
        [Key]
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public AccessTimeZone? TimeZone { get; set; }
        public ICollection<AccessLevelElement> Elements { get; set; } = new List<AccessLevelElement>();
        public ICollection<User> Users { get; set; } = new List<User>();
        public ICollection<DoorHeader> Doors { get; set; } = new List<DoorHeader>();
        public DateTime LastUpdatedAt { get; set; }
    }
    public class AccessLevelElement
    {
        [Key]
        public Guid Id { get; set; }

        // Keep Navigation both way as Doors need navigation to Access Level
        [ForeignKey("AccessLevel")]
        public Guid AccessLevelId { get; set; }
        public AccessLevel AccessLevel { get; set; } = null!;
        [ForeignKey("DoorHeader")]
        public Guid DoorId { get; set; }
        public required DoorHeader DoorHeader { get; set; }
        [ForeignKey("AccessTimeZone")]
        public AccessTimeZone? TimeZone { get; set; }
    }
}
