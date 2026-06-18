using DoorsWeb.Shared.DTO;

namespace DoorsWeb.API.Services.Interfaces
{
    /// <summary>List / add / remove sites (legacy T_Sites) for the System Settings dialog.</summary>
    public interface ISiteService
    {
        Task<List<SiteDto>> GetAll();

        /// <summary>Adds a site with the next available id. Returns the created row.</summary>
        Task<SiteDto> Create(string name);

        /// <summary>Renames a site. Returns the updated row, or null if it did not exist.</summary>
        Task<SiteDto?> Rename(int site, string name);

        /// <summary>
        /// Deletes a site by id. Returns false if it did not exist. Throws
        /// <see cref="InvalidOperationException"/> if it is the last remaining site
        /// (at least one site must always exist).
        /// </summary>
        Task<bool> Delete(int site);
    }
}
