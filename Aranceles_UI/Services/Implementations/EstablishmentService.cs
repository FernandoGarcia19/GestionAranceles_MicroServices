using Aranceles_UI.Domain.Dtos;
using Aranceles_UI.Services.Interfaces;

namespace Aranceles_UI.Services.Implementations;

public class EstablishmentService : BaseHttpService, IEstablishmentService
{
    public EstablishmentService(IHttpClientFactory factory, IHttpContextAccessor httpContextAccessor)
        : base(factory.CreateClient("establishmentApi"), httpContextAccessor)
    {
    }

    public async Task<List<EstablishmentDto>> GetAllEstablishmentsAsync()
    {
        return await GetFromJsonAuthenticatedAsync<List<EstablishmentDto>>("api/Establishment") ?? new();
    }

    public async Task<List<EstablishmentDto>> SearchEstablishmentsAsync(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return await GetAllEstablishmentsAsync();
        }

        return await GetFromJsonAuthenticatedAsync<List<EstablishmentDto>>($"api/Establishment/search/{searchTerm}") ?? new();
    }

    public async Task<EstablishmentDto?> GetEstablishmentByIdAsync(int id)
    {
        return await GetFromJsonAuthenticatedAsync<EstablishmentDto>($"api/Establishment/{id}");
    }

    public async Task<bool> CreateEstablishmentAsync(EstablishmentDto establishment)
    {
        var result = await PostAsJsonAuthenticatedAsync("api/Establishment/insert", establishment);
        return result.IsSuccessStatusCode;
    }

    public async Task<bool> UpdateEstablishmentAsync(EstablishmentDto establishment)
    {
        var result = await PutAsJsonAuthenticatedAsync($"api/Establishment/", establishment);
        return result.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteEstablishmentAsync(int id)
    {
        var result = await DeleteAuthenticatedAsync($"api/Establishment/{id}");
        return result.IsSuccessStatusCode;
    }
}

