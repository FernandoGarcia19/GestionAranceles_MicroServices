using MicroServicioUser.Dom.Entities;
using MicroServicioUser.Dom.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MicroServicoUser.Inf.Repository
{
    public static class RegistrationHelpers
    {
        public static string Md5Hex(string input)
        {
            using var md5 = MD5.Create();
            var inputBytes = Encoding.UTF8.GetBytes(input);
            var hashBytes = md5.ComputeHash(inputBytes);
            var sb = new StringBuilder();
            foreach (var b in hashBytes)
            {
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString();
        }
    }

    public class Registration : IRegistration
    {
        private readonly IRepository _userRepository;

        public Registration(IRepository userRepository)
        {

            _userRepository = userRepository;
        }

        public async Task<string> GeneratePassword(int length)
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

        public async Task<(bool ok, string? generatedUsername, string? generatedPassword, string? error)> RegisterUser(string firstName, string lastName, string email, string role, int createdBy)
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
            while (_userRepository.GetByUsername(candidate) != null)
            {
                candidate = baseUser + suffix.ToString();
                suffix++;
            }

            var pwd = await GeneratePassword(10);

            
            var roleCode = AuthenticationRoles.RoleToCode.ContainsKey(role) ? AuthenticationRoles.RoleToCode[role] : 0;

            var user = new User
            {
                Username = candidate,
                PasswordHash = RegistrationHelpers.Md5Hex(pwd),
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

            var result = await _userRepository.Insert(user);
            if (result.IsSuccess)
                return (false, null, null,"");

            return (true, candidate, pwd, null);
        }
    }
}
