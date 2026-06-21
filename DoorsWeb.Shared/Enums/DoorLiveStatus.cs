namespace DoorsWeb.Shared.Enums
{
    /// <summary>
    /// The live, at-a-glance operational state of a door, as shown on the floorplan.
    /// Drives the colour each door tile is painted. Derived on the server from controller
    /// ping replies (B,2), event-log packets (D,3) and command acknowledgements; see
    /// <c>DoorStatusDecoder</c>.
    /// </summary>
    public enum DoorLiveStatus
    {
        /// <summary>No recent contact with the controller (grey). The default until a ping reply arrives.</summary>
        Offline = 0,

        /// <summary>Door is secured / locked (green).</summary>
        Locked = 1,

        /// <summary>Door is released / unlocked, normally and on purpose (blue).</summary>
        Unlocked = 2,

        /// <summary>Door has been left open longer than allowed (amber, pulsing) — an alarm condition.</summary>
        HeldOpen = 3,

        /// <summary>Door was forced open without a valid release (red, pulsing) — an alarm condition.</summary>
        ForcedOpen = 4
    }
}
