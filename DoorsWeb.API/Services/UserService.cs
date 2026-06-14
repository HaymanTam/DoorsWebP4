using DoorsWeb.API.Data;
using DoorsWeb.API.Services.Interfaces;
using DoorsWeb.Shared.Models;
using Mapster;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;

namespace DoorsWeb.API.Services
{
    public class UserService : IUserService
    {
        private readonly DataContext _context;
        private readonly IAccessLevelService _accessLevelService;

        public UserService(DataContext context, IAccessLevelService accessLevelService)
        {
            _context = context;
            _accessLevelService = accessLevelService;
        }
        public async Task<Result<List<UserDto>>> CreateUser(UserDto userDto)
        {
            User user = userDto.Adapt<User>();
            // Already Tried Mapster Async TypeConfig its too much work for little savings
            // Manually adapt List<Guid> back to List<AccessLevel>
            var accessIds = userDto.AccessLevelIds.Select(c => c.Id).ToList();
            var accessList = await _accessLevelService.GetAccessLevelListByIds(accessIds);
            // Check for duplicate names
            bool nameexists = await _context.User.AnyAsync(u => u.Name == userDto.Name);
            if (nameexists) return Result<List<UserDto>>.Failure(new Error("Name already exists! Please use another name.", ErrorType.NameAlreadyExists));
            user.AccessLevels.AddRange(accessList);
            user.LastUpdatedAt = DateTime.Now;
            _context.User.Add(user);
            await _context.SaveChangesAsync();
            return Result<List<UserDto>>.Success(await _context.User.ProjectToType<UserDto>().ToListAsync());
        }

        public async Task<Result<List<UserDto>>> DeleteUserById(Guid id)
        {
            var result = await _context.User.FindAsync(id);
            if (result is null) return Result<List<UserDto>>.Failure(new Error($"User Id <{id}> was not found.", ErrorType.NotFound));
            _context.User.Remove(result);
            await _context.SaveChangesAsync();
            return Result<List<UserDto>>.Success(await _context.User.ProjectToType<UserDto>().ToListAsync());
        }

        public async Task<List<UserDto>> GetAllUsers()
        {
            var userList = await _context.User.Include(a => a.AccessLevels).AsNoTracking().ToListAsync();
            var userDtoList = userList.Adapt<List<UserDto>>();
            foreach (var userDto in userDtoList)
            {
                var nameList = userList.Where(c => c.Id == userDto.Id).Single().AccessLevels ?? [];
                userDto.AccessLevelIds = nameList.Adapt<List<AccessLevelNameListDto>>();
            }
            return userDtoList;
        }

        public async Task<Result<UserDto>> GetUserById(Guid id)
        {
            var user = await _context.User.Include(a => a.AccessLevels).FirstOrDefaultAsync(e => e.Id == id);
            if (user is null) return Result<UserDto>.Failure(new Error($"User Id <{id}> was not found.", ErrorType.NotFound));
            var userDto = user.Adapt<UserDto>();
            userDto.AccessLevelIds = user.AccessLevels.Adapt<List<AccessLevelNameListDto>>();
            return Result<UserDto>.Success(userDto);
        }

        public async Task<Result<List<UserDto>>> UpdateUser(Guid id, UserDto userDto)
        {
            // force alignment of url and object
            userDto.Id = id;
            // Look for user  single context call single await!
            var result = await _context.User.Include(a => a.AccessLevels).FirstOrDefaultAsync(e => e.Id == userDto.Id);
            if (result is null) return Result<List<UserDto>>.Failure(new Error($"User Id <{id}> was not found.", ErrorType.NotFound));
            _context.Entry(result).CurrentValues.SetValues(userDto.Adapt<User>());
            // Already Tried Mapster Async TypeConfig its too much work for little savings
            // Manually adapt List<Guid> back to List<AccessLevel>
            var accessIds = userDto.AccessLevelIds.Select(c => c.Id).ToList();
            var accessList = await _accessLevelService.GetAccessLevelListByIds(accessIds);
            result.AccessLevels.Clear(); // Clear the list first !Important! Remember to include access level on query
            result.AccessLevels.AddRange(accessList);
            result.LastUpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();
            return Result<List<UserDto>>.Success(await _context.User.ProjectToType<UserDto>().ToListAsync());
        }
    }
}
