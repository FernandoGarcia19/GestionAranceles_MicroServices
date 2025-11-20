using MicroServicioUser.Dom.Entities;
using MicroServicioUser.Dom.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MicroServicioUser.App.Services
{
    public class AuthenticationService : IAuthService
    {
        private readonly IRepositoryService _userService;
        private readonly EmailService _emailService;

        // Role mapping between application role names and DB integer codes
        private static readonly Dictionary<string, int> RoleToCode = new()
        {
            { "Admin", 1 },
            { "Contador", 2 }
        };

        private static readonly Dictionary<int, string> CodeToRole = RoleToCode.ToDictionary(kv => kv.Value, kv => kv.Key);

        public AuthService(IRepositoryServiceFactory<User> userService, EmailService emailService)
        {
            _userService = (IUserRepositoryService)userService.CreateRepositoryService();
            _emailService = emailService;
        }

        public (bool ok, int? userId, string? role, string? error, bool isFirstLogin) ValidateLogin(string username, string plainPassword)
        {
            var user = _userService.GetByUsername(username);
            if (user is null || !user.Status) return (false, null, null, "Usuario no encontrado o inactivo.", false);

            var givenHash = Md5Hex(plainPassword);
            if (!string.Equals(user.PasswordHash, givenHash, System.StringComparison.OrdinalIgnoreCase))
                return (false, null, null, "Contraseña incorrecta.", false);

            // DB may store role as numeric code; convert it to application role name if necessary
            var roleValue = user.Role;
            if (int.TryParse(roleValue, out var code) && CodeToRole.ContainsKey(code))
            {
                roleValue = CodeToRole[code];
            }
            return (true, user.Id, roleValue, null, user.FirstLogin == 0);
        }

        public (bool ok, string? generatedUsername, string? generatedPassword, string? error) RegisterUser(string firstName, string lastName, string email, string role, int createdBy)
        {
            if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName) || string.IsNullOrWhiteSpace(email))
                return (false, null, null, "Faltan datos obligatorios.");

            static string RemoveDiacritics(string text)
            {
                if (string.IsNullOrWhiteSpace(text)) return string.Empty;
                var normalized = text.Normalize(NormalizationForm.FormD);
                var sb = new StringBuilder();
                foreach (var c in normalized)
                {
                    var uc = CharUnicodeInfo.GetUnicodeCategory(c);
                    if (uc != UnicodeCategory.NonSpacingMark) sb.Append(c);
                }
                return sb.ToString().Normalize(NormalizationForm.FormC);
            }

            static string SlugifyLetters(string text)
            {
                text = RemoveDiacritics(text).ToLowerInvariant();
                var sb = new StringBuilder(text.Length);
                foreach (var ch in text)
                {
                    if (ch is >= 'a' and <= 'z') sb.Append(ch);
                }
                return sb.ToString();
            }

            var firstSlug = SlugifyLetters(firstName?.Trim() ?? "");
            var lastSlug = SlugifyLetters(lastName?.Trim() ?? "");

            string baseUser;
            if (!string.IsNullOrEmpty(firstSlug) && !string.IsNullOrEmpty(lastSlug))
            {
                baseUser = $"{firstSlug.Substring(0, 1)}_{lastSlug}";
            }
            else
            {
                var local = (email ?? "").Split('@')[0];
                baseUser = SlugifyLetters(local);
                if (string.IsNullOrEmpty(baseUser)) baseUser = "user";
            }

            var candidate = baseUser;
            int suffix = 2;
            while (_userService.GetByUsername(candidate) != null)
            {
                candidate = baseUser + suffix.ToString();
                suffix++;
            }

            var pwd = GeneratePassword(10);

            var roleCode = RoleToCode.ContainsKey(role) ? RoleToCode[role] : 0;

            var user = new User
            {
                Username = candidate,
                PasswordHash = Md5Hex(pwd),
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                Role = roleCode.ToString(),
                CreatedBy = createdBy,
                CreatedDate = System.DateTime.UtcNow,
                LastUpdate = System.DateTime.UtcNow,
                Status = true,
                FirstLogin = 0
            };

            var result = _userService.Insert(user);
            if (!result.IsSuccess) return (false, null, null, string.Join("; ", result.Errors));

            _ = Task.Run(async () =>
            {
                try
                {
                    await _emailService.SendNewUserCredentialsAsync(email, candidate, firstName, pwd);
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine($"Failed to send email: {ex.Message}");
                }
            });

            return (true, candidate, pwd, null);
        }


        private static string GeneratePassword(int length)
        {
            const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz23456789!@#$%";
            var rng = RandomNumberGenerator.Create();
            var bytes = new byte[length];
            rng.GetBytes(bytes);
            var sb = new StringBuilder();
            for (int i = 0; i < length; i++)
            {
                sb.Append(chars[bytes[i] % chars.Length]);
            }
            return sb.ToString();
        }

        public User? GetUserById(int userId)
        {
            var result = _userService.GetById(userId);
            return result.IsSuccess ? result.Value : null;
        }

        public async Task<(bool ok, string? error)> ChangePasswordFirstLogin(int userId, string currentPassword, string newPassword)
        {
            var userResult = _userService.GetById(userId);
            if (!userResult.IsSuccess || userResult.Value is null)
                return (false, "Usuario no encontrado.");

            var user = userResult.Value;

            if (user.FirstLogin != 0)
                return (false, "Este usuario ya ha cambiado su contraseña inicial.");

            var currentHash = Md5Hex(currentPassword);
            if (!string.Equals(user.PasswordHash, currentHash, StringComparison.OrdinalIgnoreCase))
                return (false, "La contraseña actual no es correcta.");

            var pwCheck = PasswordValidator.Validate(newPassword);
            if (!pwCheck.ok)
                return (false, pwCheck.error);
            var newHash = Md5Hex(newPassword);
            if (string.Equals(user.PasswordHash, newHash, StringComparison.OrdinalIgnoreCase))
                return (false, "La nueva contraseña debe ser diferente a la actual.");
            user.PasswordHash = newHash;
            user.FirstLogin = 1;
            user.LastUpdate = DateTime.UtcNow;

            var update = _userService.Update(user);
            if (!update.IsSuccess)
                return (false, string.Join("; ", update.Errors));

            return (true, null);
        }

        public async Task<(bool ok, string? error)> ChangePassword(int userId, string currentPassword, string newPassword)
        {
            var userResult = _userService.GetById(userId);
            if (!userResult.IsSuccess || userResult.Value is null)
                return (false, "Usuario no encontrado.");

            var user = userResult.Value;

            // 1) Verificar contraseña actual
            var currentHash = Md5Hex(currentPassword);
            if (!string.Equals(user.PasswordHash, currentHash, StringComparison.OrdinalIgnoreCase))
                return (false, "La contraseña actual no es correcta.");

            // 2) Reglas de complejidad
            var pwCheck = PasswordValidator.Validate(newPassword);
            if (!pwCheck.ok)
                return (false, pwCheck.error);

            // 3) Evitar misma contraseña
            var newHash = Md5Hex(newPassword);
            if (string.Equals(user.PasswordHash, newHash, StringComparison.OrdinalIgnoreCase))
                return (false, "La nueva contraseña debe ser diferente a la actual.");

            // 4) Persistir (NO tocar FirstLogin aquí)
            user.PasswordHash = newHash;
            user.LastUpdate = DateTime.UtcNow;

            var update = _userService.Update(user);
            if (!update.IsSuccess)
                return (false, string.Join("; ", update.Errors));

            return (true, null);
        }


        private static string Md5Hex(string input)
        {
            using var md5 = MD5.Create();
            var inputBytes = Encoding.ASCII.GetBytes(input);
            var hashBytes = md5.ComputeHash(inputBytes);
            return Convert.ToHexString(hashBytes);
        }
    }
}
