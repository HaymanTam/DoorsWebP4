using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DoorsWeb.Shared.Models
{
    public class Event
    {
        [Key]
        public Guid Id { get; set; }
        [ForeignKey("User")]
        public Guid? UserId { get; set; }
        public User? User { get; set; }
        public int CardNumber { get; set; } // seperate due to User might hold different card on their time span
        [ForeignKey("DoorHeader")]
        public Guid? DoorId { get; set; }
        public DoorHeader? DoorHeader { get; set; }
        public byte? ReaderId { get; set; }
        [ForeignKey("EventType")]
        public short EventTypeId { get; set; }
        public required EventType EventType { get; set; }
        public DateTime TimeStamp { get; set; }

    }
    public class EventType
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public short Id { get; set; }
        public required string Name { get; set; }

    }
    
}
