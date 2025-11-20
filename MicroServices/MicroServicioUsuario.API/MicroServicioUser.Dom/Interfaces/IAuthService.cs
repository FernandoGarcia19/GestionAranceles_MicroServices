using MicroServicioUser.Dom.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroServicioUser.Dom.Interfaces
{
    public interface IAuthService
    {
        (bool ok, int? userId, string? role, string? error, bool isFirstLogin) ValidateLogin(string username, string plainPassword);

        User? GetUserById(int userId);
        Task<(bool ok, string? error)> ChangePasswordFirstLogin(int userId, string currentPassword, string newPassword);
        Task<(bool ok, string? error)> ChangePassword(int userId, string currentPassword, string newPassword);


        (bool ok, string? generatedUsername, string? generatedPassword, string? error) RegisterUser(string firstName, string lastName, string email, string role, int createdBy);
    }
}
