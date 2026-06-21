namespace DoorsWeb.Shared.DTO
{
    /// <summary>The control action to perform on a door (click-to-act on the floorplan).</summary>
    public enum DoorCommandAction
    {
        /// <summary>Release the door and hold it open until told otherwise ("open forever").</summary>
        Unlock = 0,

        /// <summary>Secure the door (close / lock).</summary>
        Lock = 1,

        /// <summary>Release the door for a short, timed period then auto-relock (momentary release).</summary>
        MomentaryRelease = 2
    }

    /// <summary>
    /// Body of POST api/DoorControl/{door}/command. Maps to the controller's
    /// "5,4 Trigger Channel A/B" command: channel A, mode by <see cref="Action"/>,
    /// duration by <see cref="Seconds"/> (only used for <see cref="DoorCommandAction.MomentaryRelease"/>).
    /// </summary>
    public class DoorCommandRequest
    {
        public DoorCommandAction Action { get; set; }

        /// <summary>
        /// Release duration in seconds for a momentary release (1..255). Ignored for Unlock/Lock.
        /// When null the door's configured release time is used (falling back to a small default).
        /// </summary>
        public int? Seconds { get; set; }
    }
}
