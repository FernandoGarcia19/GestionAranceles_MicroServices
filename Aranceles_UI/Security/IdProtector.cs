using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.WebUtilities;

namespace Aranceles_UI.Security;

public class IdProtector
{
    private readonly IDataProtector _protector;

    public IdProtector(IDataProtectionProvider provider)
    {
        _protector = provider.CreateProtector("Aranceles.IdProtector");
    }

    public string ProtectInt(int id)
    {
        var bytes = BitConverter.GetBytes(id);
        var protectedBytes = _protector.Protect(bytes);
        return WebEncoders.Base64UrlEncode(protectedBytes);
    }

    public int UnprotectInt(string protectedId)
    {
        try
        {
            var protectedBytes = WebEncoders.Base64UrlDecode(protectedId);
            var bytes = _protector.Unprotect(protectedBytes);
            return BitConverter.ToInt32(bytes, 0);
        }
        catch
        {
            throw new InvalidOperationException("Identificador inválido o manipulado.");
        }
    }
}