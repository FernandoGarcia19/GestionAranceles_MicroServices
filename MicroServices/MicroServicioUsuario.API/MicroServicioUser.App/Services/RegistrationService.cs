using MicroServicioUser.Dom.Entities;
using MicroServicioUser.Dom.Interfaces;
using MicroServicoUser.Inf.Repository;
using Mysqlx;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MicroServicioUser.App.Services
{
    public class RegistrationService
    {
        private readonly IRegistration _registration;
        private readonly EmailService _emailService;
        public RegistrationService(IRegistration registration, EmailService emailService)
        {
            _registration = registration;
            _emailService = emailService;
        }

        public async Task<(bool ok, string? generatedUsername, string? generatedPassword, string? error)> RegisterUser(string firstName, string lastName, string email, string role, int createdBy)
        {
            (bool ok, string ? generatedUsername, string ? generatedPassword, string ? error) =  await _registration.RegisterUser(firstName, lastName, email, role, createdBy);
            if (!ok)
                return (false, null, null, error);

            _ = Task.Run(async () =>
            {
                try
                {
                    await _emailService.SendNewUserCredentialsAsync(email, generatedUsername, firstName, generatedPassword);
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine($"Failed to send email: {ex.Message}");
                }
            });
            return (true, generatedUsername, generatedPassword, null);
        }
    }
}
