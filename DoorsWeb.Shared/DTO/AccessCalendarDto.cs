using System.ComponentModel.DataAnnotations;

namespace DoorsWeb.Shared.DTO
{
    public class AccessCalendarDto
    {
        [Key]
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public DateTime LastUpdatedAt { get; set; }
    }
}
