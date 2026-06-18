using DoorsWeb.Shared.DTO;
using DoorsWeb.Shared.Entities;

namespace DoorsWeb.API.Services.Interfaces
{
    public interface INameHeaderService
    {
        Task<List<TNameHeader>> GetAll();

        /// <summary>Streams every card with the names of the access levels it is tied to,
        /// so the client can paint the first rows immediately and load the rest in the background.</summary>
        IAsyncEnumerable<CardDto> GetAllCards();
        Task<TNameHeader?> GetById(int id);
        Task<List<TNameHeader>> Create(TNameHeader entity);
        Task<List<TNameHeader>?> Update(int id, TNameHeader entity);
        Task<List<TNameHeader>?> Delete(int id);
    }
}
