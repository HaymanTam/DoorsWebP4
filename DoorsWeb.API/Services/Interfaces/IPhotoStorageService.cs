using DoorsWeb.Shared.DTO;

namespace DoorsWeb.API.Services.Interfaces
{
    /// <summary>
    /// Stores and removes uploaded user photos on the API's storage volume.
    /// The files are also served as static content at <c>/media/user-photo</c>.
    /// </summary>
    public interface IPhotoStorageService
    {
        /// <summary>Absolute path of the directory where photos are stored.</summary>
        string PhysicalDirectory { get; }

        /// <summary>Saves an uploaded image and returns its generated file name and web path.</summary>
        Task<PhotoUploadResult> SaveAsync(Stream content, string originalFileName, CancellationToken cancellationToken = default);

        /// <summary>Deletes a stored photo by file name (or web path). Returns false if it didn't exist.</summary>
        bool Delete(string fileNameOrPath);
    }
}
