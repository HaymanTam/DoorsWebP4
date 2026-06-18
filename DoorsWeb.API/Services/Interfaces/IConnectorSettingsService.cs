using DoorsWeb.Shared.DTO;

namespace DoorsWeb.API.Services.Interfaces
{
    /// <summary>
    /// Reads and persists the global connector polling settings shown on the System
    /// Settings dialog. Backed by a JSON file (application settings), not the legacy DB.
    /// </summary>
    public interface IConnectorSettingsService
    {
        Task<ConnectorSettingsDto> GetAsync();
        Task<ConnectorSettingsDto> SaveAsync(ConnectorSettingsDto settings);
    }
}
