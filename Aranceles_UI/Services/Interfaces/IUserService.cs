using Aranceles_UI.Domain.Dtos;
using Aranceles_UI.Pages.Users;

namespace Aranceles_UI.Services.Interfaces;

public interface IUserService
{
    Task<List<UserDto>> GetAllUsersAsync();
    Task<List<UserDto>> SearchUsersAsync(string searchTerm);
    Task<UserDto?> GetUserByIdAsync(int id);
    Task<bool> CreateUserAsync(RegisterDTO registerDto, string? secondName, string? secondLastName);
    Task<bool> UpdateUserAsync(UserDto user);
    Task<bool> DeleteUserAsync(int id);
}

