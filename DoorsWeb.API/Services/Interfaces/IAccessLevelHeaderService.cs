using DoorsWeb.Shared.Entities;

namespace DoorsWeb.API.Services.Interfaces
{
    public interface IAccessLevelHeaderService
    {
        Task<List<TAccessLevelHeader>> GetAll();
        Task<TAccessLevelHeader?> GetById(int site, int accessLevel);
        Task<List<TAccessLevelHeader>> Create(TAccessLevelHeader entity);
        Task<List<TAccessLevelHeader>?> Update(int site, int accessLevel, TAccessLevelHeader entity);
        Task<List<TAccessLevelHeader>?> Delete(int site, int accessLevel);
    }
}
