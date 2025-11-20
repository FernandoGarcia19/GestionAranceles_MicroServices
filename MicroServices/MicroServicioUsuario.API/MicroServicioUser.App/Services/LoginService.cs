using MicroServicioUser.Dom.Interfaces;

namespace MicroServicioUser.App.Services;

public class LoginService
{
    private ILogin _login;
    public LoginService(ILogin login)
    {
        _login = login;
    }

    public async Task<(bool ok, string? error)> ChangePassword(int userId, string currentPassword, string newPassword)
    {
        return await _login.ChangePassword(userId, currentPassword, newPassword);
    }

    public async Task<(bool ok, string? error)> ChangePasswordFirstLogin(int userId, string currentPassword,
        string newPassword)
    {
        return await _login.ChangePasswordFirstLogin(userId, currentPassword, newPassword);
    }

    public async Task<(bool ok, int? userId, string? role, string? error, bool isFirstLogin)> ValidateLogin(
        string username, string plainPassword)
    {
        return await _login.ValidateLogin(username, plainPassword);
    }
}