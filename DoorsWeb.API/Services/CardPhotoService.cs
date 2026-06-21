using DoorsWeb.API.Services.Interfaces;
using DoorsWeb.Shared.DTO;

namespace DoorsWeb.API.Services
{
    /// <summary>
    /// Stores cardholder photos on the API's storage volume. Each photo is named after its card
    /// number (e.g. <c>1024.jpg</c>), so it needs no database column: the path is derived from the
    /// card number and at most one photo exists per card. Files are served as static content at
    /// <see cref="WebPath"/> by the static-file middleware.
    /// </summary>
    public class CardPhotoService : ICardPhotoService
    {
        /// <summary>Public request path the card photos are served from.</summary>
        public const string WebPath = "/media/card-photo";

        private static readonly HashSet<string> AllowedExtensions =
            new(StringComparer.OrdinalIgnoreCase) { ".jpg", ".jpeg", ".png", ".gif", ".webp" };

        public string PhysicalDirectory { get; }

        public CardPhotoService(string physicalDirectory)
        {
            PhysicalDirectory = physicalDirectory;
            Directory.CreateDirectory(PhysicalDirectory);
        }

        public async Task<PhotoUploadResult> SaveAsync(int cardNumber, Stream content, string originalFileName, CancellationToken cancellationToken = default)
        {
            if (cardNumber <= 0)
                throw new InvalidOperationException("A valid card number is required to attach a photo.");

            var ext = Path.GetExtension(originalFileName);
            if (string.IsNullOrWhiteSpace(ext) || !AllowedExtensions.Contains(ext))
            {
                throw new InvalidOperationException(
                    $"Unsupported image type '{ext}'. Allowed: {string.Join(", ", AllowedExtensions)}.");
            }

            // One photo per card: drop any previous file (which may have a different extension).
            Delete(cardNumber);

            var fileName = $"{cardNumber}{ext.ToLowerInvariant()}";
            var fullPath = Path.Combine(PhysicalDirectory, fileName);

            await using (var fs = new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                await content.CopyToAsync(fs, cancellationToken);
            }

            return new PhotoUploadResult
            {
                FileName = fileName,
                Path = $"{WebPath}/{fileName}"
            };
        }

        public string? GetPath(int cardNumber)
        {
            var existing = Find(cardNumber);
            return existing is null ? null : $"{WebPath}/{Path.GetFileName(existing)}";
        }

        public IReadOnlyDictionary<int, string> GetAllFileNames()
        {
            var map = new Dictionary<int, string>();
            if (!Directory.Exists(PhysicalDirectory))
                return map;

            foreach (var file in Directory.EnumerateFiles(PhysicalDirectory))
            {
                if (!AllowedExtensions.Contains(Path.GetExtension(file)))
                    continue;

                // Photos are named after the card number; ignore anything else in the folder.
                if (int.TryParse(Path.GetFileNameWithoutExtension(file), out var cardNumber))
                    map[cardNumber] = Path.GetFileName(file);
            }
            return map;
        }

        public bool Delete(int cardNumber)
        {
            var deleted = false;
            foreach (var file in FindAll(cardNumber))
            {
                File.Delete(file);
                deleted = true;
            }
            return deleted;
        }

        // Locates the photo file(s) for a card (e.g. "1024.jpg"). EnumerateFiles' "1024.*" pattern
        // only matches the exact card number before the dot, so "10240.jpg" won't be picked up.
        private string? Find(int cardNumber) => FindAll(cardNumber).FirstOrDefault();

        private IEnumerable<string> FindAll(int cardNumber)
        {
            if (cardNumber <= 0 || !Directory.Exists(PhysicalDirectory))
                return Enumerable.Empty<string>();

            return Directory.EnumerateFiles(PhysicalDirectory, $"{cardNumber}.*")
                .Where(f => AllowedExtensions.Contains(Path.GetExtension(f)));
        }

        /// <summary>
        /// Resolves the configured storage directory (env <c>Storage__CardPhotoDirectory</c> /
        /// config <c>Storage:CardPhotoDirectory</c>), falling back to a local folder. Shared by DI
        /// registration and the static-file provider so both point at the same place.
        /// </summary>
        public static string ResolveDirectory(IConfiguration configuration)
        {
            var configured = configuration["Storage:CardPhotoDirectory"];
            return string.IsNullOrWhiteSpace(configured)
                ? Path.Combine(AppContext.BaseDirectory, "CardPhotos")
                : configured;
        }
    }
}
