using System;
using System.Text.Json.Serialization;

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
        public string? ReaderName { get; set; }          // Door's configured reader name (1 = Reader A, 2 = Reader B); null when unmapped
        public int EventType { get; set; }
        public string? EventName { get; set; }           // EventTypeNavigation.Description
        public string? ActualCardId { get; set; }

        /// <summary>What the Events grid shows in its "Reader" column: the door's configured reader
        /// name when known, otherwise a "Reader {id}" fallback. Computed client-side, so it isn't serialized.</summary>
        [JsonIgnore]
        public string ReaderLabel =>
            string.IsNullOrWhiteSpace(ReaderName) ? $"Reader {ReaderId}" : ReaderName!;
    }
}
