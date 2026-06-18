using DoorsWeb.Shared.DTO;

namespace DoorsWeb.API.Services.Interfaces
{
    /// <summary>List / add / remove sites (legacy T_Sites) for the System Settings dialog.</summary>
    public interface ISiteService
    {
        Task<List<SiteDto>> GetAll();

        /// <summary>Adds a site with the next available id. Returns the created row.</summary>
        Task<SiteDto> Create(string name);

        /// <summary>Deletes a site by id. Returns false if it did not exist.</summary>
        Task<bool> Delete(int site);
    }
}
