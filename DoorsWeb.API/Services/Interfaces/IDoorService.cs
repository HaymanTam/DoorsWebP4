namespace DoorsWeb.API.Services.Interfaces
{
    public interface IDoorService
    {
        Task<List<DoorListDto>> GetAllDoors();
        Task<DoorDetailDto?> GetDoorById(Guid id);
        Task<List<DoorListDto>> CreateDoor(DoorDetailDto dDetail);
        Task<List<DoorListDto>?> UpdateDoor(Guid id, DoorDetailDto dDetail);
        Task<List<DoorListDto>?> DeleteDoorById(Guid id);
    }
}
