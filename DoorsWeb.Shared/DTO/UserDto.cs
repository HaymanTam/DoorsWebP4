using DoorsWeb.Shared.Models;
using Mapster;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Text;

namespace DoorsWeb.Shared.DTO
{
    public class UserDto
    {
        [Key]
        public Guid Id { get; set; }
        public int CardNumber { get; set; }
        public string? PhotoPath { get; set; }
        public string? Name { get; set; }
        public bool IsEnabled { get; set; }
        public List<AccessLevelNameListDto> AccessLevelIds { get; set; } = new List<AccessLevelNameListDto>();
        // Added this for search purposes in tables
        public string AccessNames => string.Concat(AccessLevelIds.Select(r => r.Name));
        public DateTime LastUpdatedAt { get; set; }

    }
}
