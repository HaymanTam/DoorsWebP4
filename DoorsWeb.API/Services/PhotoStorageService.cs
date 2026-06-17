using DoorsWeb.API.Services.Interfaces;
using DoorsWeb.Shared.DTO;

namespace DoorsWeb.API.Services
{
    /// <summary>
    /// Stores uploaded user photos on the API's storage volume. Files are written with
    /// server-generated GUID names (so client-supplied names can't traverse or collide) and
    /// served as static content at <see cref="WebPath"/> by the static-file middleware.
    /// </summary>
    public class PhotoStorageService : IPhotoStorageService
    {
        /// <summary>Public request path the photos are served from.</summary>
        public const string WebPath = "/media/user-photo";

        private static readonly HashSet<string> AllowedExtensions =
            new(StringComparer.OrdinalIgnoreCase) { ".jpg", ".jpeg", ".png", ".gif", ".webp" };

        public string PhysicalDirectory { get; }

        public PhotoStorageService(string physicalDirectory)
        {
            PhysicalDirectory = physicalDirectory;
            Directory.CreateDirectory(PhysicalDirectory);
        }

        public async Task<PhotoUploadResult> SaveAsync(Stream content, string originalFileName, CancellationToken cancellationToken = default)
        {
            var ext = Path.GetExtension(originalFileName);
            if (string.IsNullOrWhiteSpace(ext) || !AllowedExtensions.Contains(ext))
            {
                throw new InvalidOperationException(
                    $"Unsupported image type '{ext}'. Allowed: {string.Join(", ", AllowedExtensions)}.");
            }

            var fileName = $"{Guid.NewGuid():N}{ext.ToLowerInvariant()}";
            var fullPath = Path.Combine(PhysicalDirectory, fileName);

            await using (var fs = new FileStream(fullPath, FileMode.CreateNew, FileAccess.Write, FileShare.None))
            {
                await content.CopyToAsync(fs, cancellationToken);
            }

            return new PhotoUploadResult
            {
                FileName = fileName,
                Path = $"{WebPath}/{fileName}"
            };
        }

        public bool Delete(string fileNameOrPath)
        {
            // Strip any path/web prefix; only the bare file name is ever trusted.
            var fileName = Path.GetFileName(fileNameOrPath);
            if (string.IsNullOrWhiteSpace(fileName)) return false;

            var fullPath = Path.Combine(PhysicalDirectory, fileName);
            if (!File.Exists(fullPath)) return false;

            File.Delete(fullPath);
            return true;
        }

        /// <summary>
        /// Resolves the configured storage directory (env <c>Storage__UserPhotoDirectory</c> /
        /// config <c>Storage:UserPhotoDirectory</c>), falling back to a local folder. Shared by
        /// DI registration and the static-file provider so both point at the same place.
        /// </summary>
        public static string ResolveDirectory(IConfiguration configuration)
        {
            var configured = configuration["Storage:UserPhotoDirectory"];
            return string.IsNullOrWhiteSpace(configured)
                ? Path.Combine(AppContext.BaseDirectory, "UserPhotos")
                : configured;
        }
    }
}
