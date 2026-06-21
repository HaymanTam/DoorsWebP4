using DoorsWeb.Shared.DTO;

namespace DoorsWeb.API.Services.Interfaces
{
    /// <summary>
    /// Stores cardholder photos on the API's storage volume, keyed by card number so no column is
    /// needed on the (legacy) cardholder table — the photo's existence and path are derived from the
    /// card number alone. Files are served as static content at <c>/media/card-photo</c>.
    /// </summary>
    public interface ICardPhotoService
    {
        /// <summary>Absolute path of the directory where card photos are stored.</summary>
        string PhysicalDirectory { get; }

        /// <summary>Saves (replacing any existing) the photo for a card and returns its file name and web path.</summary>
        Task<PhotoUploadResult> SaveAsync(int cardNumber, Stream content, string originalFileName, CancellationToken cancellationToken = default);

        /// <summary>Web path of the card's photo, or null when it has none.</summary>
        string? GetPath(int cardNumber);

        /// <summary>
        /// Maps every card that has a photo to its file name (e.g. <c>1024 → "1024.jpg"</c>), built from
        /// a single directory scan. Used by the CSV export so it can fill the photo column without one
        /// filesystem lookup per card.
        /// </summary>
        IReadOnlyDictionary<int, string> GetAllFileNames();

        /// <summary>Deletes the card's photo. Returns false if there wasn't one.</summary>
        bool Delete(int cardNumber);
    }
}
