using Aranceles_UI.Domain.Dtos;

namespace Aranceles_UI.Services.Interfaces;

public interface IPersonInChargeService
{
    Task<List<PersonInChargeDto>> GetAllPersonsInChargeAsync();
    Task<List<PersonInChargeDto>> SearchPersonsInChargeAsync(string searchTerm);
    Task<PersonInChargeDto?> GetPersonInChargeByIdAsync(int id);
    Task<bool> CreatePersonInChargeAsync(PersonInChargeDto person, string? secondName, string? secondLastName);
    Task<bool> UpdatePersonInChargeAsync(PersonInChargeDto person, string? secondName, string? secondLastName);
    Task<bool> DeletePersonInChargeAsync(int id);
}

