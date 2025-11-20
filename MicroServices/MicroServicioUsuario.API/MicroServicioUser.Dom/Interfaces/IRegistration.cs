using MicroServicioUser.Dom.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MicroServicioUser.Dom.Interfaces
{
    public interface IRegistration
    {
        public Task<string> GeneratePassword(int lenght);
        public Task<(bool ok, string? generatedUsername, string? generatedPassword, string? error)> RegisterUser(string firstname, string lastName, string email, string role, int createdBy);
    }
}
