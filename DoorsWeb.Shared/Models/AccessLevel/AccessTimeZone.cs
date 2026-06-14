using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DoorsWeb.Shared.Models
{
    
    // Unable to use TimeZone due to collision with system.TimeZone
    [Index(nameof(Name), IsUnique = true)]
    public class AccessTimeZone
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public required string Name { get; set; }
        [ForeignKey("AccessCalendar")]
        public Guid? CalendarId { get; set; }
        public AccessCalendar? Calendar { get; set; }
        public ICollection<AccessTimeZoneElement> Elements { get; set; } = new List<AccessTimeZoneElement>();
        public DateTime LastUpdatedAt { get; set; }
    }
    public class AccessTimeZoneElement
    {
        [Key]
        public Guid Id { get; set; }

        public TimeOnly? StartTime { get; set; }
        public TimeOnly? EndTime { get; set; }
        public bool Monday { get; set; }
        public bool Tuesday { get; set; }
        public bool Wednesday { get; set; }
        public bool Thursday { get; set; }
        public bool Friday { get; set; }
        public bool Saturday { get; set; }
        public bool Sunday { get; set; }
        [ForeignKey("AccessCalendar")]
        public Guid? CalendarId { get; set; }
        public AccessCalendar? Calendar { get; set; }

    }
}
