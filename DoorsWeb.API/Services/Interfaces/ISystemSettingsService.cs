using DoorsWeb.Shared.DTO;

namespace DoorsWeb.API.Services.Interfaces
{
    /// <summary>
    /// Reads/writes the global <see cref="SystemSettings"/> JSON document for the
    /// System Settings dialog. Backed by a file on the settings volume.
    /// </summary>
    public interface ISystemSettingsService
    {
        /// <summary>Current settings (defaults if no file has been written yet).</summary>
        SystemSettings Get();

        /// <summary>Persists the settings and returns the stored value.</summary>
        SystemSettings Save(SystemSettings settings);
    }
}
