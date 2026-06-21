using DoorsWeb.Shared.DTO;
using DoorsWeb.Shared.Entities;

namespace DoorsWeb.API.Services.Interfaces
{
    public interface ICardholderService
    {
        Task<List<Cardholder>> GetAll();

        /// <summary>Streams every card with the names of the access levels it is tied to (and whether
        /// it has a photo), so the client can paint the first rows immediately and load the rest in the
        /// background.</summary>
        IAsyncEnumerable<CardDto> GetAllCards(CancellationToken cancellationToken = default);

        /// <summary>Builds a CardPresso-friendly CSV of every cardholder (RFC 4180, comma-delimited),
        /// including each card's photo file name so the card designer can bind images by filename.</summary>
        Task<string> BuildCardPressoCsvAsync();

        Task<Cardholder?> GetById(int id);
        Task<List<Cardholder>> Create(Cardholder entity);
        Task<List<Cardholder>?> Update(int id, Cardholder entity);
        Task<List<Cardholder>?> Delete(int id);
    }
}
