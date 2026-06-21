namespace DoorsWeb.Shared.DTO
{
    /// <summary>
    /// Global system settings, persisted as a single JSON document on the API's settings
    /// volume (Storage:SettingsDirectory). Read/updated as a whole by SystemSettingsController.
    /// "Settings are always global" — there is no per-site or per-controller scoping.
    /// </summary>
    public class SystemSettings
    {
        public ControllerCommunicationSettings ControllerCommunication { get; set; } = new();

        public LicenseSettings License { get; set; } = new();
    }

    /// <summary>
    /// The installation's license. Just the signed key string — the limits and expiry inside it are
    /// validated server-side, never trusted from here. Persisted alongside the other system settings.
    /// </summary>
    public class LicenseSettings
    {
        /// <summary>The signed license key (the string the keygen / payment site produces). Null when unlicensed.</summary>
        public string? Key { get; set; }
    }

    /// <summary>
    /// How the protocol polls the controllers. These drive the UDP protocol loop and replace
    /// the per-connector polling columns the legacy system used (PingFrequency / CommandFrequency
    /// / ForcePing) now that connectors are gone.
    /// </summary>
    public class ControllerCommunicationSettings
    {
        /// <summary>Seconds between controller pings ("Ping controllers every N seconds").</summary>
        public int PingIntervalSeconds { get; set; } = 2;

        /// <summary>Seconds between command checks ("Check for commands every N seconds").</summary>
        public int CommandCheckIntervalSeconds { get; set; } = 2;

        /// <summary>Force a ping after this many commands ("Force a ping after N commands").</summary>
        public int ForcePingAfterCommands { get; set; } = 50;
    }
}
