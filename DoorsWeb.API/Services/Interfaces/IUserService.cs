namespace DoorsWeb.API.Services.Interfaces
{
    public interface IUserService
    {
        Task<List<UserDto>> GetAllUsers();
        Task<Result<UserDto>> GetUserById(Guid id);
        Task<Result<List<UserDto>>> CreateUser(UserDto userDto);
        Task<Result<List<UserDto>>> UpdateUser(Guid id, UserDto userDto);
        Task<Result<List<UserDto>>> DeleteUserById(Guid id);
    }
}
