using DoorsWeb.Shared.Entities;

namespace DoorsWeb.API.Services.Interfaces
{
    public interface IAccessLevelHeaderService
    {
        Task<List<TAccessLevelHeader>> GetAll();
        Task<TAccessLevelHeader?> GetById(int accessLevel, int site);
        Task<List<TAccessLevelHeader>> Create(TAccessLevelHeader entity);
        Task<List<TAccessLevelHeader>?> Update(int accessLevel, int site, TAccessLevelHeader entity);
        Task<List<TAccessLevelHeader>?> Delete(int accessLevel, int site);
    }
}
