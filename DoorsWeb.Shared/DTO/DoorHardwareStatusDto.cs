using System;
using DoorsWeb.Shared.Enums;

namespace DoorsWeb.Shared.DTO
{
    /// <summary>
    /// The hardware-level detail decoded from a door controller's ping reply (B,2): the two relay
    /// states, any active alarms, firmware version and a few diagnostics. Attached to
    /// <see cref="DoorStateDto.Hardware"/>; it stays <c>null</c> until the first reply arrives, which
    /// the UI renders as "unknown" (blue) rather than guessing a value.
    /// </summary>
    public class DoorHardwareStatusDto
    {
        /// <summary>Relay A (the lock relay) state.</summary>
        public RelayState RelayA { get; set; } = RelayState.Unknown;

        /// <summary>Relay B (the auxiliary relay) state.</summary>
        public RelayState RelayB { get; set; } = RelayState.Unknown;

        /// <summary>Any active alarm conditions (fire, intruder, tamper, …).</summary>
        public DoorAlarmFlags Alarms { get; set; } = DoorAlarmFlags.None;

        /// <summary>Controller firmware version "Major.Minor", or null if not reported.</summary>
        public string? FirmwareVersion { get; set; }

        /// <summary>Supply voltage in volts (e.g. 13.7), or null if not reported.</summary>
        public double? SupplyVoltage { get; set; }

        /// <summary>Number of unread event-log entries the controller is holding, or null.</summary>
        public int? UnreadLogCount { get; set; }

        /// <summary>Number of free card blocks the controller reports, or null.</summary>
        public int? CardBlocks { get; set; }

        /// <summary>UTC time the most recent ping reply was decoded.</summary>
        public DateTime? LastPolledUtc { get; set; }

        /// <summary>Raw Status byte 1 from the reply (relay/monitor bits), for diagnostics.</summary>
        public byte Status1 { get; set; }

        /// <summary>Raw Status byte 2 from the reply (alarm bits), for diagnostics.</summary>
        public byte Status2 { get; set; }
    }
}
