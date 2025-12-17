using Aranceles_UI.Domain.Dtos;
using Aranceles_UI.Services.Interfaces;

namespace Aranceles_UI.Services.Implementations;

public class PaymentService : BaseHttpService, IPaymentService
{
    public PaymentService(IHttpClientFactory factory, IHttpContextAccessor httpContextAccessor) 
        : base(factory.CreateClient("paymentApi"), httpContextAccessor)
    {
    }

    public async Task<List<PaymentDto>> GetAllPaymentsAsync()
    {
        return await GetFromJsonAuthenticatedAsync<List<PaymentDto>>("/api/Payment") ?? new();
    }

    public async Task<List<PaymentDto>> SearchPaymentsAsync(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return await GetAllPaymentsAsync();
        }

        var term = Uri.EscapeDataString(searchTerm.Trim());
        return await GetFromJsonAuthenticatedAsync<List<PaymentDto>>($"/api/Payment/search/{term}") ?? new();
    }

    public async Task<PaymentDto?> GetPaymentByIdAsync(int id)
    {
        return await GetFromJsonAuthenticatedAsync<PaymentDto>($"api/Payment/{id}");
    }

    public async Task<bool> CreatePaymentAsync(PaymentDto payment)
    {
        var result = await PostAsJsonAuthenticatedAsync("api/Payment/insert", payment);
        return result.IsSuccessStatusCode;
    }

    public async Task<bool> UpdatePaymentAsync(PaymentDto payment)
    {
        var result = await PutAsJsonAuthenticatedAsync($"api/Payment/", payment);
        return result.IsSuccessStatusCode;
    }

    public async Task<bool> DeletePaymentAsync(int id)
    {
        var result = await DeleteAuthenticatedAsync($"api/Payment/{id}");
        return result.IsSuccessStatusCode;
    }
}

