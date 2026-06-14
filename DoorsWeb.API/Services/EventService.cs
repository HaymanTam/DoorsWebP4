using DoorsWeb.API.Data;
using DoorsWeb.API.Services.Interfaces;
using DoorsWeb.Shared.Models;
using Mapster;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Reflection.PortableExecutable;

namespace DoorsWeb.API.Services
{
    public class EventService : IEventService
    {
        private readonly DataContext _context;
        private readonly IHubContext<EventHub> _hubContext;
        public EventService(DataContext context, IHubContext<EventHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }
        public async Task<List<EventListDto>> CreateEvent(EventCreateDto EvtDto)
        {
            // Go check foreign keys valid
            
            if(EvtDto.TimeStamp == DateTime.MinValue) EvtDto.TimeStamp = DateTime.Now;
            Event evt = EvtDto.Adapt<Event>();
            _context.Event.Add(evt);
            await _context.SaveChangesAsync();
            // push to wasm 
            await _hubContext.Clients.All.SendAsync("NewEvent", EvtDto);
            return await _context.Event.ProjectToType<EventListDto>().ToListAsync();
        }

        public async Task<List<EventListDto>?> DeleteEventById(Guid id)
        {
            var result = await _context.Event.FindAsync(id);
            if (result is null) return null;
            // Event Found
            _context.Event.Remove(result);
            await _context.SaveChangesAsync();
            return await _context.Event.ProjectToType<EventListDto>().ToListAsync();
        }

        public async Task<EventListDto?> GetEventById(Guid id)
        {
            var evt = await _context.Event.FindAsync(id);
            return evt.Adapt<EventListDto>();
        }

        public async Task<List<EventListDto>> GetEvents(DateTime? startDate, DateTime? endDate, int? pageSize, int? pageNumber)
        {
            var pSize = pageSize ?? 500;
            var pNum = pageNumber ?? 1;
            var query = _context.Event.AsNoTracking().AsQueryable();
            if (startDate is not null) query = query.Where(x => x.TimeStamp >= startDate);
            if (endDate is not null) query = query.Where(x => x.TimeStamp <= endDate);
            return await query.OrderBy(p => p.TimeStamp).Skip((pNum - 1) * pSize).Take(pSize).ProjectToType<EventListDto>().ToListAsync();
        }
    }
}
