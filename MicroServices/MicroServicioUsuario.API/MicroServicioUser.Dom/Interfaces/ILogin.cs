namespace MicroServicioUser.Dom.Interfaces;

public interface ILogin
{
    public Task<(bool ok, string? error)> ChangePassword(int userId, string currentPassword, string newPassword);
    public Task<(bool ok, string? error)> ChangePasswordFirstLogin(int userId, string currentPassword, string newPassword);
    public Task<(bool ok, int? userId, string? role, string? error, bool isFirstLogin)> ValidateLogin(string username, string plainPassword);
}