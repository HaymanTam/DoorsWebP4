using DoorsWeb.Shared.DTO;

namespace DoorsWeb.API.Services.Interfaces
{
    public interface IDoorService
    {
        Task<List<DoorListDto>> GetAll();
        Task<DoorDetailDto?> GetById(int door);
        Task<List<DoorListDto>> Create(DoorDetailDto dto);
        Task<List<DoorListDto>?> Update(int door, DoorDetailDto dto);
        Task<List<DoorListDto>?> Delete(int door);
    }
}
