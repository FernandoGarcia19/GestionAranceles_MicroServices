using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroServicoUser.Inf.EmailAdapters;

public sealed record SmtpSettings
{
    public string Host { get; init; } = "";
    public int Port { get; init; } = 25;
    public string User { get; init; } = "";
    public string Password { get; init; } = "";
    public string FromEmail { get; init; } = "";
    public string FromName { get; init; } = "Sistema de Pagos";
}
