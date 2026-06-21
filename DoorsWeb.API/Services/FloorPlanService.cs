using System.Text.Json;
using DoorsWeb.API.Services.Interfaces;
using DoorsWeb.Shared.DTO;

namespace DoorsWeb.API.Services
{
    /// <summary>
    /// File-backed floorplan layout store. JSON layout files live under
    /// <c>{settings}/floorplans</c>; uploaded background images live under
    /// <c>{settings}/floorplans/images</c> and are served at <see cref="ImageWebPath"/>.
    /// Singleton; file access is serialized by a lock. Mirrors <see cref="SystemSettingsService"/>.
    /// </summary>
    public class FloorPlanService : IFloorPlanService
    {
        /// <summary>Public request path the floorplan images are served from.</summary>
        public const string ImageWebPath = "/media/floorplan";

        private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };
        private static readonly HashSet<string> AllowedExtensions =
            new(StringComparer.OrdinalIgnoreCase) { ".jpg", ".jpeg", ".png", ".gif", ".webp" };

        private readonly string _layoutDir;
        private readonly object _gate = new();

        public string ImageDirectory { get; }

        public FloorPlanService(string settingsDirectory)
        {
            _layoutDir = Path.Combine(settingsDirectory, "floorplans");
            ImageDirectory = Path.Combine(_layoutDir, "images");
            Directory.CreateDirectory(_layoutDir);
            Directory.CreateDirectory(ImageDirectory);
        }

        public FloorPlanLayoutDto Get(int site)
        {
            lock (_gate)
            {
                return Load(site);
            }
        }

        public FloorPlanLayoutDto Save(FloorPlanLayoutDto layout)
        {
            ArgumentNullException.ThrowIfNull(layout);
            lock (_gate)
            {
                Write(layout);
            }
            return layout;
        }

        public async Task<FloorPlanLayoutDto> SaveImageAsync(int site, Stream content, string originalFileName, CancellationToken ct = default)
        {
            var ext = Path.GetExtension(originalFileName);
            if (string.IsNullOrWhiteSpace(ext) || !AllowedExtensions.Contains(ext))
            {
                throw new InvalidOperationException(
                    $"Unsupported image type '{ext}'. Allowed: {string.Join(", ", AllowedExtensions)}.");
            }

            var fileName = $"site-{site}-{Guid.NewGuid():N}{ext.ToLowerInvariant()}";
            var fullPath = Path.Combine(ImageDirectory, fileName);

            await using (var fs = new FileStream(fullPath, FileMode.CreateNew, FileAccess.Write, FileShare.None))
            {
                await content.CopyToAsync(fs, ct);
            }

            lock (_gate)
            {
                var layout = Load(site);
                var previous = layout.ImageFileName;
                layout.ImageFileName = fileName;
                Write(layout);

                // Best-effort cleanup of the replaced image.
                if (!string.IsNullOrWhiteSpace(previous))
                    TryDeleteImage(previous);

                return layout;
            }
        }

        // ---- file helpers ------------------------------------------------------------

        private FloorPlanLayoutDto Load(int site)
        {
            var path = LayoutPath(site);
            if (!File.Exists(path)) return new FloorPlanLayoutDto { Site = site };
            try
            {
                var json = File.ReadAllText(path);
                var layout = JsonSerializer.Deserialize<FloorPlanLayoutDto>(json);
                if (layout is null) return new FloorPlanLayoutDto { Site = site };
                layout.Site = site; // keep authoritative
                layout.Doors ??= new();
                return layout;
            }
            catch
            {
                return new FloorPlanLayoutDto { Site = site };
            }
        }

        private void Write(FloorPlanLayoutDto layout)
        {
            var json = JsonSerializer.Serialize(layout, JsonOptions);
            File.WriteAllText(LayoutPath(layout.Site), json);
        }

        private string LayoutPath(int site) => Path.Combine(_layoutDir, $"floorplan-{site}.json");

        private void TryDeleteImage(string fileNameOrPath)
        {
            try
            {
                var fileName = Path.GetFileName(fileNameOrPath);
                if (string.IsNullOrWhiteSpace(fileName)) return;
                var full = Path.Combine(ImageDirectory, fileName);
                if (File.Exists(full)) File.Delete(full);
            }
            catch { /* non-fatal */ }
        }
    }
}
