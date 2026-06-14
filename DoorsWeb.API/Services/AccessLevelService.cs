using DoorsWeb.API.Data;
using DoorsWeb.API.Services.Interfaces;
using DoorsWeb.Shared.DTO;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace DoorsWeb.API.Services
{
    public class AccessLevelService : IAccessLevelService
    {
        private readonly DataContext _context;

        public AccessLevelService(DataContext context)
        {
            _context = context;
        }

        public async Task<List<AccessLevelDto>> CreateAccessLevel(AccessLevelDto accessLevelDto)
        {
            AccessLevel accessLevel = accessLevelDto.Adapt<AccessLevel>();
            accessLevel.LastUpdatedAt = DateTime.Now;
            _context.AccessLevel.Add(accessLevel);
            await _context.SaveChangesAsync();
            return await _context.AccessLevel.ProjectToType<AccessLevelDto>().ToListAsync();
        }

        public async Task<List<AccessLevelDto>?> DeleteAccessLevelById(Guid id)
        {
            var result = await _context.AccessLevel.FindAsync(id);
            if (result is null) return null;
            // Access Level Found
            _context.AccessLevel.Remove(result);
            await _context.SaveChangesAsync();
            return await _context.AccessLevel.ProjectToType<AccessLevelDto>().ToListAsync();
        }

        public async Task<AccessLevelDto?> GetAccessLevelById(Guid id)
        {
            var accessLevel = await _context.AccessLevel.FindAsync(id);
            return accessLevel.Adapt<AccessLevelDto>();
        }

        public async Task<List<AccessLevel>> GetAccessLevelListByIds(List<Guid> idList) 
        {
            return await _context.AccessLevel.Where(e => idList.Contains(e.Id)).ToListAsync();
        }
        public async Task<List<AccessLevelDto>> GetAllAccessLevel()
        {
            return await _context.AccessLevel.ProjectToType<AccessLevelDto>().ToListAsync();
        }

        public async Task<List<AccessLevelDto>?> UpdateAccessLevel(Guid id, AccessLevelDto accessLevelDto)
        {
            // force alignment of url and object
            accessLevelDto.Id = id;
            // Look for auser
            var result = await _context.AccessLevel.FindAsync(accessLevelDto.Id);
            if (result == null) return null;
            _context.Entry(result).CurrentValues.SetValues(accessLevelDto.Adapt<AccessLevel>());
            result.LastUpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();
            return await _context.AccessLevel.ProjectToType<AccessLevelDto>().ToListAsync();
        }
    }
}
