namespace DoorsWeb.API.Services.Interfaces
{
    public interface IAdminService
    {
        Task<List<Admin>> GetAllAdmins();
        Task<Admin?> GetAdminById(Guid id);
        Task<List<Admin>> CreateAdmin(Admin admin);
        Task<List<Admin>?> UpdateAdmin(Guid id, Admin admin);
        Task<List<Admin>?> DeleteAdminById(Guid id);
        Task<Result<Admin>> LoginAsync(AdminLoginDto dto, CancellationToken ct = default);
        Task<Admin?> GetByUsernameAsync(string username, CancellationToken ct = default);
    }
            
}
