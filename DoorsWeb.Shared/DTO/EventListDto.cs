using DoorsWeb.Shared.Models;
using Mapster;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DoorsWeb.Shared.DTO
{
    public class EventListDto
    {
        [Key]
        public Guid Id { get; set; }
        public Guid? UserId { get; set; } // !!!populated from CardNumber!!!
        public string? UserName { get; set; }
        public int CardNumber { get; set; } // This is the key
        public Guid? DoorId { get; set; }
        [AdaptMember("DoorHeaderName")] //Required for Mapster to Adapt
        public string? DoorName { get; set; }
        public string? ReaderName { get; set; }
        [AdaptMember("EventTypeId")]
        public short EventId { get; set; }
        [AdaptMember("EventTypeName")]
        public string? EventName { get; set; }
        public DateTime TimeStamp { get; set; }

    }
}
