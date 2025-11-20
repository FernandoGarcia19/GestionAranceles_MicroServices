using MicroServicioUser.Dom.Patterns;

namespace MicroServicioUser.Dom.Interfaces;

public interface IJwtService
{
    public Task<Result<string>> GenerateToken(int userId, string role);
}