using System;

namespace DoorsWeb.Shared.DTO
{
    /// <summary>A single access-control event row projected from T_Events.</summary>
    public class EventDto
    {
        public int EventId { get; set; }                 // T_Events.EventId (PK)
        public DateTime EventDate { get; set; }          // Time
        public int CardNumber { get; set; }              // Card#
        public string? CardHolder { get; set; }          // Forname + Surname (CardNumberNavigation)
        public int DoorNumber { get; set; }
        public string? DoorName { get; set; }            // DoorNavigation.Name
        public int ReaderId { get; set; }
        public int EventType { get; set; }
        public string? EventName { get; set; }           // EventTypeNavigation.Description
        public string? ActualCardId { get; set; }
    }
}
