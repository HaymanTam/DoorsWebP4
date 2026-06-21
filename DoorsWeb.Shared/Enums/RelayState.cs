namespace DoorsWeb.Shared.Enums
{
    /// <summary>
    /// The state of a door controller's relay, as reported in a ping reply (B,2) status byte.
    /// <see cref="Unknown"/> is the default until the first reply arrives — the UI paints it blue
    /// ("not yet known") rather than implying the relay is open or closed.
    /// </summary>
    public enum RelayState
    {
        /// <summary>No ping reply seen yet, so the relay state is not known (blue).</summary>
        Unknown = 0,

        /// <summary>Relay de-energised — door secured / locked.</summary>
        Closed = 1,

        /// <summary>Relay energised — door released / open.</summary>
        Open = 2
    }
}
