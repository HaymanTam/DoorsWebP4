using DoorsWeb.API.Data;
using DoorsWeb.API.Services.Interfaces;
using DoorsWeb.Shared.DTO;
using DoorsWeb.Shared.Models;
using Mapster;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Reflection.Metadata.Ecma335;

namespace DoorsWeb.API.Services
{
    public class DoorService : IDoorService
    {
        private readonly DataContext _context;

        public DoorService(DataContext context)
        {
            _context = context;
        }

        public async Task<List<DoorListDto>> CreateDoor(DoorDetailDto dDetail)
        {
            DoorHeader dheader = dDetail.Adapt<DoorHeader>();
            DoorSettings dsettings = dDetail.Adapt<DoorSettings>();
            _context.DoorHeader.Add(dheader);
            dheader.LastUpdatedAt = DateTime.Now;
            dsettings.Header = dheader;
            dsettings.LastUpdatedAt = DateTime.Now;
            _context.DoorSettings.Add(dsettings);
            await _context.SaveChangesAsync();
            return await _context.DoorHeader.ProjectToType<DoorListDto>().ToListAsync();
        }

        public async Task<List<DoorListDto>> GetAllDoors()
        {
            return await _context.DoorHeader.ProjectToType<DoorListDto>().ToListAsync();
        }

        public async Task<DoorDetailDto?> GetDoorById(Guid id)
        {
            var dheader = await _context.DoorHeader.FindAsync(id);
            var dsettings = await _context.DoorSettings.FindAsync(id);

            var result = dheader.Adapt<DoorDetailDto>();
            dsettings.Adapt(result);
            return result;
        }
        public async Task<List<DoorListDto>?> UpdateDoor(Guid id, DoorDetailDto dDetail)
        {
            // force alignment of url and object
            dDetail.Id = id;

            // look for entry within database
            var dheader = await _context.DoorHeader.FindAsync(dDetail.Id);
            var dsettings = await _context.DoorSettings.FindAsync(dDetail.Id);
            if (dheader is null) return null;
            _context.Entry(dheader).CurrentValues.SetValues(dDetail.Adapt<DoorHeader>());
            dheader.LastUpdatedAt = DateTime.Now;

            // in case dheader exist but not dsettings
            if (dsettings is null)
            {
                dsettings = dDetail.Adapt<DoorSettings>();
                dsettings.Header = dheader;
                dsettings.LastUpdatedAt = DateTime.Now;
                _context.DoorSettings.Add(dsettings);
            }
            else { 
                _context.Entry(dsettings).CurrentValues.SetValues(dDetail.Adapt<DoorSettings>());
                dsettings.LastUpdatedAt = DateTime.Now;
            }
            await _context.SaveChangesAsync();
            return await _context.DoorHeader.ProjectToType<DoorListDto>().ToListAsync();
        }
        public async Task<List<DoorListDto>?> DeleteDoorById(Guid id)
        {
            var dheader = await _context.DoorHeader.FindAsync(id);
            var dsettings = await _context.DoorSettings.FindAsync(id);
            if(dheader is null && dsettings is null) return null;
            if (dsettings is not null) _context.DoorSettings.Remove(dsettings);
            if (dheader is not null) _context.DoorHeader.Remove(dheader);
            await _context.SaveChangesAsync();
            return await _context.DoorHeader.ProjectToType<DoorListDto>().ToListAsync();
        }
    }
}
