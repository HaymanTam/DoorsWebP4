using System;
using DoorsWeb.Shared.Enums;

namespace DoorsWeb.Shared.DTO
{
    /// <summary>
    /// One door's live state for the floorplan. Sent both in the initial snapshot
    /// (GET api/DoorControl/states) and on every real-time change pushed over the
    /// SignalR <c>EventHub</c> as the "DoorStateChanged" message.
    /// </summary>
    public class DoorStateDto
    {
        /// <summary>Door number (T_Doors.Door / PK).</summary>
        public int Door { get; set; }

        /// <summary>Door display name.</summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>Owning site (T_Doors.Site), so the view can filter to one site at a time.</summary>
        public int? Site { get; set; }

        /// <summary>Current live state (colour driver).</summary>
        public DoorLiveStatus Status { get; set; } = DoorLiveStatus.Offline;

        /// <summary>Human-readable name of the most recent event (e.g. "Card OK", "Door Forced"), or null.</summary>
        public string? LastEventName { get; set; }

        /// <summary>UTC time the most recent event/state change was observed, or null if never.</summary>
        public DateTime? LastEventUtc { get; set; }

        /// <summary>
        /// Hardware-level detail from the controller's last ping reply (relay states, alarms,
        /// firmware, diagnostics). Null until the first reply arrives → rendered as "unknown" (blue).
        /// </summary>
        public DoorHardwareStatusDto? Hardware { get; set; }
    }
}
