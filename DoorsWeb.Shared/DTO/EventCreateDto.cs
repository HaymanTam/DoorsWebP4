using Mapster;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DoorsWeb.Shared.DTO
{
    public class EventCreateDto
    {
        [Key]
        public Guid Id { get; set; }
        public int? CardNumber { get; set; } // This is the key
        public Guid? DoorId { get; set; }
        public byte? ReaderId { get; set; }
        public short EventId { get; set; }
        public DateTime TimeStamp { get; set; }

    }
}
