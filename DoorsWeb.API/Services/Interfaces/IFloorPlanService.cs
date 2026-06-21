using DoorsWeb.Shared.DTO;

namespace DoorsWeb.API.Services.Interfaces
{
    /// <summary>
    /// Persists each site's optional floorplan layout (background image + door placements) as a
    /// JSON file on the settings volume, with the uploaded images stored alongside and served as
    /// static content from <see cref="ImageWebPath"/>.
    /// </summary>
    public interface IFloorPlanService
    {
        /// <summary>The site's layout, or an empty layout (no image, no placements) when none has been saved.</summary>
        FloorPlanLayoutDto Get(int site);

        /// <summary>Saves the full layout (image reference + placements) for a site.</summary>
        FloorPlanLayoutDto Save(FloorPlanLayoutDto layout);

        /// <summary>Stores an uploaded background image for a site and returns the updated layout.</summary>
        Task<FloorPlanLayoutDto> SaveImageAsync(int site, Stream content, string originalFileName, CancellationToken ct = default);

        /// <summary>Physical directory the images live in (so the static-file provider can point at it).</summary>
        string ImageDirectory { get; }
    }
}
