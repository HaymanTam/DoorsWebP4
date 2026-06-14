using DoorsWeb.Shared.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DoorsWeb.Shared.DTO
{
    public class AccessLevelDto
    {
        [Key]
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public Guid? TimeZoneId { get; set; }
        public ICollection<Guid> ElementsId { get; set; } = new List<Guid>();
        public ICollection<Guid> UsersId { get; set; } = new List<Guid>();
        public ICollection<Guid> DoorsId { get; set; } = new List<Guid>();
        public DateTime LastUpdatedAt { get; set; }
    }
}
