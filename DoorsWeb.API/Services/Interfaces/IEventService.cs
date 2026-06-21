using DoorsWeb.Shared.DTO;

namespace DoorsWeb.API.Services.Interfaces
{
    public interface IEventService
    {
        /// <summary>
        /// Streams the most-recent events (capped) so callers can report incremental load progress.
        /// When <paramref name="from"/>/<paramref name="to"/> are supplied, the result is restricted
        /// to that date range (still recent-first and capped).
        /// </summary>
        IAsyncEnumerable<EventDto> GetAll(DateTime? from = null, DateTime? to = null);

        /// <summary>
        /// Number of rows <see cref="GetAll"/> will yield for the same filter (capped at the same
        /// maximum), for progress display.
        /// </summary>
        Task<int> GetCount(DateTime? from = null, DateTime? to = null);

        /// <summary>
        /// Inserts one decoded controller event into T_Events and returns it shaped as an
        /// <see cref="EventDto"/> (with cardholder / door / event-type names resolved) ready to push
        /// to clients, or null if the inserted row could not be read back. <paramref name="cardNumber"/>
        /// is 0 for events that don't carry a card; <paramref name="cardId"/> is the raw card string.
        /// </summary>
        Task<EventDto?> RecordAsync(int door, DateTime when, int cardNumber, string? cardId,
            int readerId, int eventType, CancellationToken ct = default);
    }
}
