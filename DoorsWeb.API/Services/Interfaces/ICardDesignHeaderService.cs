using DoorsWeb.Shared.Entities;

namespace DoorsWeb.API.Services.Interfaces
{
    public interface ICardDesignHeaderService
    {
        Task<List<CardDesign>> GetAll();
        Task<CardDesign?> GetById(int id);
        Task<List<CardDesign>> Create(CardDesign entity);
        Task<List<CardDesign>?> Update(int id, CardDesign entity);
        Task<List<CardDesign>?> Delete(int id);
    }
}
