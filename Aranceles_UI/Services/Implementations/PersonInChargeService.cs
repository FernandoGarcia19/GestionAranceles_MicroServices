using Aranceles_UI.Domain.Dtos;
using Aranceles_UI.Services.Interfaces;

namespace Aranceles_UI.Services.Implementations;

public class PersonInChargeService : IPersonInChargeService
{
    private readonly HttpClient _personClient;

    public PersonInChargeService(IHttpClientFactory factory)
    {
        _personClient = factory.CreateClient("personInChargeApi");
    }

    public async Task<List<PersonInChargeDto>> GetAllPersonsInChargeAsync()
    {
        return await _personClient.GetFromJsonAsync<List<PersonInChargeDto>>("api/PersonInCharge/") ?? new();
    }

    public async Task<List<PersonInChargeDto>> SearchPersonsInChargeAsync(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return await GetAllPersonsInChargeAsync();
        }

        return await _personClient.GetFromJsonAsync<List<PersonInChargeDto>>($"api/PersonInCharge/search/{searchTerm}") ?? new();
    }

    public async Task<PersonInChargeDto?> GetPersonInChargeByIdAsync(int id)
    {
        return await _personClient.GetFromJsonAsync<PersonInChargeDto>($"api/PersonInCharge/{id}");
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

        var result = await _personClient.PostAsJsonAsync("api/PersonInCharge/insert", person);
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

        var result = await _personClient.PutAsJsonAsync($"api/PersonInCharge/{person.Id}", person);
        return result.IsSuccessStatusCode;
    }

    public async Task<bool> DeletePersonInChargeAsync(int id)
    {
        var result = await _personClient.DeleteAsync($"api/PersonInCharge/{id}");
        return result.IsSuccessStatusCode;
    }
}

