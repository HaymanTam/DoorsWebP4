namespace DoorsWeb.API.Services.Interfaces
{
    public interface IAccessLevelService
    {
        Task<List<AccessLevelDto>> GetAllAccessLevel();
        Task<AccessLevelDto?> GetAccessLevelById(Guid id);
        Task<List<AccessLevel>> GetAccessLevelListByIds(List<Guid> idList);
        Task<List<AccessLevelDto>> CreateAccessLevel(AccessLevelDto accessLevelDto);
        Task<List<AccessLevelDto>?> UpdateAccessLevel(Guid id, AccessLevelDto accessLevelDto);
        Task<List<AccessLevelDto>?> DeleteAccessLevelById(Guid id);
    }
}
