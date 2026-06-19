using DoorsWeb.Shared.Entities;

namespace DoorsWeb.API.Services.Interfaces
{
    public interface ICardManagerHeaderService
    {
        Task<List<CardManager>> GetAll();
        Task<CardManager?> GetById(int id);
        Task<List<CardManager>> Create(CardManager entity);
        Task<List<CardManager>?> Update(int id, CardManager entity);
        Task<List<CardManager>?> Delete(int id);
    }
}
