using System.Text.Json;
using DoorsWeb.API.Services.Interfaces;
using DoorsWeb.Shared.DTO;

namespace DoorsWeb.API.Services
{
    /// <summary>
    /// Persists the global connector polling settings to a JSON file on the API's storage
    /// volume. Independent of the legacy per-connector T_Connectors rows (the user opted to
    /// keep these as application settings). Reads fall back to defaults if the file is absent.
    /// </summary>
    public class ConnectorSettingsService : IConnectorSettingsService
    {
        private const string FileName = "connector-settings.json";

        private static readonly SemaphoreSlim Gate = new(1, 1);
        private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

        private readonly string _filePath;

        public ConnectorSettingsService(IConfiguration configuration)
        {
            var dir = ResolveDirectory(configuration);
            Directory.CreateDirectory(dir);
            _filePath = Path.Combine(dir, FileName);
        }

        public async Task<ConnectorSettingsDto> GetAsync()
        {
            await Gate.WaitAsync();
            try
            {
                if (!File.Exists(_filePath)) return new ConnectorSettingsDto();
                var json = await File.ReadAllTextAsync(_filePath);
                return JsonSerializer.Deserialize<ConnectorSettingsDto>(json) ?? new ConnectorSettingsDto();
            }
            catch
            {
                // Corrupt/unreadable file: fall back to defaults rather than failing the dialog.
                return new ConnectorSettingsDto();
            }
            finally
            {
                Gate.Release();
            }
        }

        public async Task<ConnectorSettingsDto> SaveAsync(ConnectorSettingsDto settings)
        {
            // Clamp to sane bounds so a bad request can't persist zero/negative intervals.
            settings.PingControllersSeconds = Math.Clamp(settings.PingControllersSeconds, 1, 3600);
            settings.CheckCommandsSeconds = Math.Clamp(settings.CheckCommandsSeconds, 1, 3600);
            settings.ForcePingAfterCommands = Math.Clamp(settings.ForcePingAfterCommands, 1, 100000);

            await Gate.WaitAsync();
            try
            {
                var json = JsonSerializer.Serialize(settings, JsonOptions);
                await File.WriteAllTextAsync(_filePath, json);
                return settings;
            }
            finally
            {
                Gate.Release();
            }
        }

        /// <summary>
        /// Resolves the settings directory (env <c>Storage__SettingsDirectory</c> /
        /// config <c>Storage:SettingsDirectory</c>), falling back to a local folder.
        /// </summary>
        public static string ResolveDirectory(IConfiguration configuration)
        {
            var configured = configuration["Storage:SettingsDirectory"];
            return string.IsNullOrWhiteSpace(configured)
                ? Path.Combine(AppContext.BaseDirectory, "AppSettings")
                : configured;
        }
    }
}
