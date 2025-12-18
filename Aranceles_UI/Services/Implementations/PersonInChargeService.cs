using Aranceles_UI.Domain.Dtos;
using Aranceles_UI.Services.Interfaces;

namespace Aranceles_UI.Services.Implementations;

public class PersonInChargeService : BaseHttpService, IPersonInChargeService
{
    public PersonInChargeService(IHttpClientFactory factory, IHttpContextAccessor httpContextAccessor)
        : base(factory.CreateClient("personInChargeApi"), httpContextAccessor)
    {
    }

    public async Task<List<PersonInChargeDto>> GetAllPersonsInChargeAsync()
    {
        return await GetFromJsonAuthenticatedAsync<List<PersonInChargeDto>>("api/PersonInCharge/") ?? new();
    }

    public async Task<List<PersonInChargeDto>> SearchPersonsInChargeAsync(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return await GetAllPersonsInChargeAsync();
        }

        return await GetFromJsonAuthenticatedAsync<List<PersonInChargeDto>>($"api/PersonInCharge/search/{searchTerm}") ?? new();
    }

    public async Task<PersonInChargeDto?> GetPersonInChargeByIdAsync(int id)
    {
        return await GetFromJsonAuthenticatedAsync<PersonInChargeDto>($"api/PersonInCharge/{id}");
    }

    public async Task<bool> CreatePersonInChargeAsync(PersonInChargeDto person, string? secondName, string? secondLastName)
    {
        var fullFirstName = person.FirstName.Trim();
        if (!string.IsNullOrWhiteSpace(secondName))
        {
            fullFirstName += " " + secondName.Trim();
        }

        var fullLastName = person.LastName.Trim();
        if (!string.IsNullOrWhiteSpace(secondLastName))
        {
            fullLastName += " " + secondLastName.Trim();
        }

        person.FirstName = fullFirstName;
        person.LastName = fullLastName;

        var result = await PostAsJsonAuthenticatedAsync("api/PersonInCharge/insert", person);
        return result.IsSuccessStatusCode;
    }

    public async Task<bool> UpdatePersonInChargeAsync(PersonInChargeDto person, string? secondName, string? secondLastName)
    {
        var fullFirstName = person.FirstName.Trim();
        if (!string.IsNullOrWhiteSpace(secondName))
        {
            fullFirstName += " " + secondName.Trim();
        }

        var fullLastName = person.LastName.Trim();
        if (!string.IsNullOrWhiteSpace(secondLastName))
        {
            fullLastName += " " + secondLastName.Trim();
        }

        person.FirstName = fullFirstName;
        person.LastName = fullLastName;

        var result = await PutAsJsonAuthenticatedAsync($"api/PersonInCharge/", person);
        return result.IsSuccessStatusCode;
    }

    public async Task<bool> DeletePersonInChargeAsync(int id)
    {
        var result = await DeleteAuthenticatedAsync($"api/PersonInCharge/{id}");
        return result.IsSuccessStatusCode;
    }
}

