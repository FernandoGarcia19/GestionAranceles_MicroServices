using Aranceles_UI.Domain.Dtos;

namespace Aranceles_UI.Services.Interfaces;

public interface IPaymentService
{
    Task<List<PaymentDto>> GetAllPaymentsAsync();
    Task<List<PaymentDto>> SearchPaymentsAsync(string searchTerm);
    Task<PaymentDto?> GetPaymentByIdAsync(int id);
    Task<bool> CreatePaymentAsync(PaymentDto payment);
    Task<bool> UpdatePaymentAsync(PaymentDto payment);
    Task<bool> DeletePaymentAsync(int id);
}

