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
    }
}
