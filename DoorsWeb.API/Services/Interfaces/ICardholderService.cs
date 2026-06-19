using DoorsWeb.Shared.DTO;
using DoorsWeb.Shared.Entities;

namespace DoorsWeb.API.Services.Interfaces
{
    public interface ICardholderService
    {
        Task<List<Cardholder>> GetAll();

        /// <summary>Streams every card with the names of the access levels it is tied to,
        /// so the client can paint the first rows immediately and load the rest in the background.</summary>
        IAsyncEnumerable<CardDto> GetAllCards();
        Task<Cardholder?> GetById(int id);
        Task<List<Cardholder>> Create(Cardholder entity);
        Task<List<Cardholder>?> Update(int id, Cardholder entity);
        Task<List<Cardholder>?> Delete(int id);
    }
}
