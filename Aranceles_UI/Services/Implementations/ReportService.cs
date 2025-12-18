using System.Net.Http.Json;
using Aranceles_UI.Services.Interfaces;

namespace Aranceles_UI.Services.Implementations;

public class ReportService : BaseHttpService, IReportService
{
    public ReportService(IHttpClientFactory factory, IHttpContextAccessor httpContextAccessor)
        : base(factory.CreateClient("reportApi"), httpContextAccessor)
    {
    }

    public async Task<byte[]?> GeneratePaymentPdfAsync(int paymentId, object paymentData)
    {
        try
        {
            var response = await SendAuthenticatedRequestAsync(async () =>
                await _httpClient.PostAsJsonAsync("api/Report/payment-pdf", paymentData));
            
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsByteArrayAsync();
            }
            
            return null;
        }
        catch
        {
            return null;
        }
    }
}
