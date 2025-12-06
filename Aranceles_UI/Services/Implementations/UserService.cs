using Aranceles_UI.Domain.Dtos;
using Aranceles_UI.Pages.Users;
using Aranceles_UI.Services.Interfaces;

namespace Aranceles_UI.Services.Implementations;

public class UserService : BaseHttpService, IUserService
{
    public UserService(IHttpClientFactory factory, IHttpContextAccessor httpContextAccessor)
        : base(factory.CreateClient("userApi"), httpContextAccessor)
    {
    }

    public async Task<List<UserDto>> GetAllUsersAsync()
    {
        return await GetFromJsonAuthenticatedAsync<List<UserDto>>("api/User/select") ?? new();
    }

    public async Task<List<UserDto>> SearchUsersAsync(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return await GetFromJsonAuthenticatedAsync<List<UserDto>>("api/User") ?? new();
        }

        return await GetFromJsonAuthenticatedAsync<List<UserDto>>($"api/User/search/{searchTerm}") ?? new();
    }

    public async Task<UserDto?> GetUserByIdAsync(int id)
    {
        return await GetFromJsonAuthenticatedAsync<UserDto>($"api/User/getById/{id}");
    }

    public async Task<bool> CreateUserAsync(RegisterDTO registerDto, string? secondName, string? secondLastName)
    {
        var fullFirstName = registerDto.FirstName.Trim();
        if (!string.IsNullOrWhiteSpace(secondName))
        {
            fullFirstName += " " + secondName.Trim();
        }

        var fullLastName = registerDto.LastName.Trim();
        if (!string.IsNullOrWhiteSpace(secondLastName))
        {
            fullLastName += " " + secondLastName.Trim();
        }

        registerDto.FirstName = fullFirstName;
        registerDto.LastName = fullLastName;

        var result = await PostAsJsonAuthenticatedAsync("api/User/register", registerDto);
        return result.IsSuccessStatusCode;
    }

    public async Task<bool> UpdateUserAsync(UserDto user)
    {
        var result = await PutAsJsonAuthenticatedAsync("api/User/update", user);
        return result.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteUserAsync(int id)
    {
        var result = await DeleteAuthenticatedAsync($"api/User/delete/{id}");
        return result.IsSuccessStatusCode;
    }
}

