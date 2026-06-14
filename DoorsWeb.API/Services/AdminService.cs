using DoorsWeb.API.Data;
using DoorsWeb.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DoorsWeb.API.Services
{
    public class AdminService : IAdminService
    {
        private readonly DataContext _context;
        private readonly Guid SuperGuid;

        public AdminService(DataContext context, IConfiguration configuration)
        {
            _context = context;
            var superstring = configuration["SuperGuid"] ??
                throw new InvalidOperationException("string 'SuperGuid' not found.");
            SuperGuid = new Guid(superstring);
        }
        public async Task<List<Admin>> CreateAdmin(Admin admin)
        {
            admin.LastUpdatedAt = DateTime.Now;
            _context.Admin.Add(admin);
            await _context.SaveChangesAsync();
            return await _context.Admin.ToListAsync();
        }

        public async Task<List<Admin>> GetAllAdmins()
        {
            return await _context.Admin.ToListAsync();
        }


        public async Task<Admin?> GetAdminById(Guid id)
        {
            return await _context.Admin.FindAsync(id);
        }

        public async Task<List<Admin>?> UpdateAdmin(Guid id, Admin admin)
        {
            // force alignment of url and object
            admin.Id = id;
            // Look for admin
            var result = await _context.Admin.FindAsync(admin.Id);
            if (result == null) return null;
            // Make sure SuperAdmin remains super
            if (admin.Id == SuperGuid)
            {
                admin.Username = "super";
                admin.CanEditUsers = true;
                admin.CanEditDoors = true;
                admin.CanEditAdmins = true;
            }
            _context.Entry(result).CurrentValues.SetValues(admin);
            admin.LastUpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();
            return await _context.Admin.ToListAsync();
        }

        public async Task<List<Admin>?> DeleteAdminById(Guid id)
        {
            // Avoid Deleting SuperAdmin
            if (id == SuperGuid) return null;
            var result = await _context.Admin.FindAsync(id);
            if (result is null) return null;
            // Admin Found
            _context.Admin.Remove(result);
            await _context.SaveChangesAsync();
            return await _context.Admin.ToListAsync();
        }

        public async Task<Result<Admin>> LoginAsync(AdminLoginDto dto, CancellationToken ct = default)
        {
            var admin = await _context.Admin.FirstOrDefaultAsync(a => a.Username == dto.Username, ct);
            if (admin is null)
                return Result<Admin>.Failure(new Error("Invalid username or password.", ErrorType.Unauthorized));
            var pwHash = new PwHashService();
            if (!pwHash.Verify(dto.Password, admin.PasswordHash))
                return Result<Admin>.Failure(new Error("Invalid username or password.", ErrorType.Unauthorized));
            return Result<Admin>.Success(admin);
        }

        public async Task<Admin?> GetByUsernameAsync(string username, CancellationToken ct = default)
        {
            return await _context.Admin.FirstOrDefaultAsync(a => a.Username == username, ct);
        }
    }
}
