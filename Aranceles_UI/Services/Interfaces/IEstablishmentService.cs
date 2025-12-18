using Aranceles_UI.Domain.Dtos;

namespace Aranceles_UI.Services.Interfaces;

public interface IEstablishmentService
{
    Task<List<EstablishmentDto>> GetAllEstablishmentsAsync();
    Task<List<EstablishmentDto>> SearchEstablishmentsAsync(string searchTerm);
    Task<EstablishmentDto?> GetEstablishmentByIdAsync(int id);
    Task<bool> CreateEstablishmentAsync(EstablishmentDto establishment);
    Task<bool> UpdateEstablishmentAsync(EstablishmentDto establishment);
    Task<bool> DeleteEstablishmentAsync(int id);
}

