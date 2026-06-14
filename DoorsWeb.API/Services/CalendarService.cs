using DoorsWeb.API.Data;
using DoorsWeb.API.Services.Interfaces;
using DoorsWeb.Shared.DTO;
using DoorsWeb.Shared.Models;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace DoorsWeb.API.Services
{
    public class CalendarService : ICalendarService
    {
        private readonly DataContext _context;

        public CalendarService(DataContext context)
        {
            _context = context;
        }

        public async Task<List<AccessCalendarDto>> GetAllCalendars()
        {
            return await _context.AccessCalendar.ProjectToType<AccessCalendarDto>().ToListAsync();
        }

        public async Task<AccessCalendarDto?> GetCalendarById(Guid id)
        {
            var calendar = await _context.AccessCalendar.FindAsync(id);
            return calendar?.Adapt<AccessCalendarDto>();
        }

        public async Task<List<AccessCalendarDto>> CreateCalendar(AccessCalendarDto dto)
        {
            var calendar = dto.Adapt<AccessCalendar>();
            calendar.LastUpdatedAt = DateTime.UtcNow;
            _context.AccessCalendar.Add(calendar);
            await _context.SaveChangesAsync();
            return await _context.AccessCalendar.ProjectToType<AccessCalendarDto>().ToListAsync();
        }

        public async Task<List<AccessCalendarDto>?> UpdateCalendar(Guid id, AccessCalendarDto dto)
        {
            dto.Id = id;
            var result = await _context.AccessCalendar.FindAsync(id);
            if (result is null) return null;
            _context.Entry(result).CurrentValues.SetValues(dto.Adapt<AccessCalendar>());
            result.LastUpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return await _context.AccessCalendar.ProjectToType<AccessCalendarDto>().ToListAsync();
        }

        public async Task<List<AccessCalendarDto>?> DeleteCalendarById(Guid id)
        {
            var result = await _context.AccessCalendar.FindAsync(id);
            if (result is null) return null;
            _context.AccessCalendar.Remove(result);
            await _context.SaveChangesAsync();
            return await _context.AccessCalendar.ProjectToType<AccessCalendarDto>().ToListAsync();
        }
    }
}
