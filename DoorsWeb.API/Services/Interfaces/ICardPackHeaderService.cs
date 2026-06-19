using DoorsWeb.Shared.Entities;

namespace DoorsWeb.API.Services.Interfaces
{
    public interface ICardPackHeaderService
    {
        Task<List<CardPack>> GetAll();
        Task<CardPack?> GetById(int id);
        Task<List<CardPack>> Create(CardPack entity);
        Task<List<CardPack>?> Update(int id, CardPack entity);
        Task<List<CardPack>?> Delete(int id);
    }
}
