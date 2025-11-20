using MicroServicioUser.Dom.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroServicioUser.App.Services
{
    public class EmailService
    {
        private IEmailService _emailService;

        public EmailService(IEmailService emailService)
        {
            _emailService = emailService;
        }

        public async Task<bool> SendEmailAsync(string toEmail, string subject, string body, bool isHtml = true)
        {
            return await _emailService.SendEmailAsync(toEmail, subject, body, isHtml);
        }

        public async Task<bool> SendNewUserCredentialsAsync(string toEmail, string userName, string firstName, string password)
        {
            var subject = "Bienvenido al Sistema de Pagos de Aranceles";

            var body = $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <style>
                        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
                        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                        .header {{ background-color: #0d6efd; color: white; padding: 20px; text-align: center; border-radius: 5px 5px 0 0; }}
                        .content {{ background-color: #f8f9fa; padding: 30px; border-radius: 0 0 5px 5px; }}
                        .credentials {{ background-color: white; padding: 20px; margin: 20px 0; border-left: 4px solid #0d6efd; }}
                        .credential-item {{ margin: 10px 0; }}
                        .credential-label {{ font-weight: bold; color: #0d6efd; }}
                        .credential-value {{ font-family: 'Courier New', monospace; background-color: #e9ecef; padding: 5px 10px; border-radius: 3px; display: inline-block; }}
                        .warning {{ background-color: #fff3cd; border-left: 4px solid #ffc107; padding: 15px; margin: 20px 0; }}
                        .footer {{ text-align: center; margin-top: 20px; color: #6c757d; font-size: 12px; }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <h1>¡Bienvenido!</h1>
                        </div>
                        <div class='content'>
                            <p>Hola <strong>{firstName}</strong>,</p>
                            
                            <p>Se ha creado una cuenta para ti en el Sistema de Pagos de Aranceles.</p>
                            
                            <div class='credentials'>
                                <div class='credential-item'>
                                    <span class='credential-label'>Usuario:</span>
                                    <span class='credential-value'>{userName}</span>
                                </div>
                                <div class='credential-item'>
                                    <span class='credential-label'>Contraseña:</span>
                                    <span class='credential-value'>{password}</span>
                                </div>
                            </div>
                            
                            <div class='warning'>
                                <strong>⚠️ Importante:</strong> 
                                <ul>
                                    <li>Guarda estas credenciales en un lugar seguro</li>
                                    <li>No compartas tu contraseña con nadie</li>
                                    <li>Se recomienda cambiar la contraseña después del primer inicio de sesión</li>
                                </ul>
                            </div>
                            
                            <p>Si tienes alguna pregunta o problema para acceder al sistema, contacta con el administrador.</p>
                            
                            <p>Saludos,<br><strong>Equipo del Sistema de Pagos de Aranceles</strong></p>
                        </div>
                        <div class='footer'>
                            <p>Este es un correo automático, por favor no respondas a este mensaje.</p>
                        </div>
                    </div>
                </body>
                </html>
            ";

            return await SendEmailAsync(toEmail, subject, body, isHtml: true);
        }
    }
}
