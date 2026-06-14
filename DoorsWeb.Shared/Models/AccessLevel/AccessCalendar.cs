using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DoorsWeb.Shared.Models
{
    [Index(nameof(Name), IsUnique = true)]
    public class AccessCalendar
    {
        [Key]
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public ICollection<AccessCalendarElement> Elements { get; set; } = new List<AccessCalendarElement>();
        public DateTime LastUpdatedAt { get; set; }
    }
    public class AccessCalendarElement
    {
        [Key]
        public Guid Id { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
    }
}
