namespace DoorsWeb.Shared.DTO
{
    /// <summary>
    /// Global connector polling settings shown on the System Settings dialog.
    /// Stored as application settings (a JSON file on the API) independent of the legacy
    /// per-connector T_Connectors rows. Defaults mirror the legacy Doors client.
    /// </summary>
    public class ConnectorSettingsDto
    {
        /// <summary>Ping controllers every N seconds.</summary>
        public int PingControllersSeconds { get; set; } = 2;

        /// <summary>Check for commands every N seconds.</summary>
        public int CheckCommandsSeconds { get; set; } = 2;

        /// <summary>Force a ping after N commands.</summary>
        public int ForcePingAfterCommands { get; set; } = 50;
    }
}
