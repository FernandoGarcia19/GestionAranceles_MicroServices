using Aranceles_UI.Domain.Dtos;
using Aranceles_UI.Services.Interfaces;

namespace Aranceles_UI.Services.Implementations;

public class EstablishmentService : IEstablishmentService
{
    private readonly HttpClient _establishmentClient;

    public EstablishmentService(IHttpClientFactory factory)
    {
        _establishmentClient = factory.CreateClient("establishmentApi");
    }

    public async Task<List<EstablishmentDto>> GetAllEstablishmentsAsync()
    {
        return await _establishmentClient.GetFromJsonAsync<List<EstablishmentDto>>("api/Establishment") ?? new();
    }

    public async Task<List<EstablishmentDto>> SearchEstablishmentsAsync(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return await GetAllEstablishmentsAsync();
        }

        return await _establishmentClient.GetFromJsonAsync<List<EstablishmentDto>>($"api/Establishment/search/{searchTerm}") ?? new();
    }

    public async Task<EstablishmentDto?> GetEstablishmentByIdAsync(int id)
    {
        return await _establishmentClient.GetFromJsonAsync<EstablishmentDto>($"api/Establishment/{id}");
    }

    public async Task<bool> CreateEstablishmentAsync(EstablishmentDto establishment)
    {
        var result = await _establishmentClient.PostAsJsonAsync("api/Establishment/insert", establishment);
        return result.IsSuccessStatusCode;
    }

    public async Task<bool> UpdateEstablishmentAsync(EstablishmentDto establishment)
    {
        var result = await _establishmentClient.PutAsJsonAsync($"api/Establishment/", establishment);
        return result.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteEstablishmentAsync(int id)
    {
        var result = await _establishmentClient.DeleteAsync($"api/Establishment/{id}");
        return result.IsSuccessStatusCode;
    }
}

