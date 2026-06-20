using System.Text.Json;
using DoorsWeb.API.Services.Interfaces;
using DoorsWeb.Shared.DTO;

namespace DoorsWeb.API.Services
{
    /// <summary>
    /// Persists the global <see cref="SystemSettings"/> as a single JSON file on the configured
    /// settings volume (Storage:SettingsDirectory). Registered as a singleton; file access is
    /// serialized by a lock since the file is the single source of truth. A missing or unreadable
    /// file yields defaults, so the app always has usable settings.
    /// </summary>
    public class SystemSettingsService : ISystemSettingsService
    {
        private const string FileName = "system-settings.json";
        private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

        private readonly string _filePath;
        private readonly object _gate = new();

        public SystemSettingsService(string settingsDirectory)
        {
            Directory.CreateDirectory(settingsDirectory);
            _filePath = Path.Combine(settingsDirectory, FileName);
        }

        public SystemSettings Get()
        {
            lock (_gate)
            {
                return Load();
            }
        }

        public SystemSettings Save(SystemSettings settings)
        {
            settings ??= new SystemSettings();
            lock (_gate)
            {
                var json = JsonSerializer.Serialize(settings, JsonOptions);
                File.WriteAllText(_filePath, json);
            }
            return settings;
        }

        private SystemSettings Load()
        {
            if (!File.Exists(_filePath)) return new SystemSettings();
            try
            {
                var json = File.ReadAllText(_filePath);
                return JsonSerializer.Deserialize<SystemSettings>(json) ?? new SystemSettings();
            }
            catch
            {
                // Corrupt/unreadable file: fall back to defaults rather than failing the app.
                return new SystemSettings();
            }
        }

        /// <summary>
        /// Resolves the settings directory (env <c>Storage__SettingsDirectory</c> / config
        /// <c>Storage:SettingsDirectory</c>), falling back to a local folder. Mirrors
        /// <see cref="PhotoStorageService.ResolveDirectory"/>.
        /// </summary>
        public static string ResolveDirectory(IConfiguration configuration)
        {
            var configured = configuration["Storage:SettingsDirectory"];
            return string.IsNullOrWhiteSpace(configured)
                ? Path.Combine(AppContext.BaseDirectory, "Settings")
                : configured;
        }
    }
}
