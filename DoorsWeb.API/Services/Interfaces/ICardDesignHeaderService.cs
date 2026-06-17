using DoorsWeb.Shared.Entities;

namespace DoorsWeb.API.Services.Interfaces
{
    public interface ICardDesignHeaderService
    {
        Task<List<TCardDesignHeader>> GetAll();
        Task<TCardDesignHeader?> GetById(int id);
        Task<List<TCardDesignHeader>> Create(TCardDesignHeader entity);
        Task<List<TCardDesignHeader>?> Update(int id, TCardDesignHeader entity);
        Task<List<TCardDesignHeader>?> Delete(int id);
    }
}
