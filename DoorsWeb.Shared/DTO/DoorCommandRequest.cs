namespace DoorsWeb.Shared.DTO
{
    /// <summary>The control action to perform on a door (click-to-act on the floorplan).</summary>
    public enum DoorCommandAction
    {
        /// <summary>Release the relay and hold it open until told otherwise ("override open / forever").</summary>
        Unlock = 0,

        /// <summary>Secure the relay (close / lock).</summary>
        Lock = 1,

        /// <summary>Release the relay for a short, timed period then auto-relock (momentary release).</summary>
        MomentaryRelease = 2
    }

    /// <summary>
    /// Which relay to drive. The values are the controller's channel codes for the
    /// "5,4 Trigger Channel A/B" command (Data 0): 0x01 = Lock A, 0x02 = Lock B.
    /// </summary>
    public enum DoorRelay
    {
        /// <summary>Relay A / Relay 1 — the door lock relay (the floorplan's "release door").</summary>
        RelayA = 0x01,

        /// <summary>Relay B / Relay 2 — the auxiliary relay.</summary>
        RelayB = 0x02
    }

    /// <summary>
    /// Body of POST api/DoorControl/{door}/command. Maps to the controller's "5,4 Trigger Channel
    /// A/B" command: channel from <see cref="Relay"/> (Data 0), mode from <see cref="Action"/>
    /// (Data 1), duration from <see cref="Seconds"/> (Data 2, only for a momentary release).
    /// </summary>
    public class DoorCommandRequest
    {
        public DoorCommandAction Action { get; set; }

        /// <summary>Which relay to drive. Defaults to Relay A — the door lock the floorplan releases.</summary>
        public DoorRelay Relay { get; set; } = DoorRelay.RelayA;

        /// <summary>
        /// Release duration in seconds for a momentary release (1..255). Ignored for Unlock/Lock.
        /// When null the door's configured release time is used (falling back to a small default).
        /// </summary>
        public int? Seconds { get; set; }
    }
}
