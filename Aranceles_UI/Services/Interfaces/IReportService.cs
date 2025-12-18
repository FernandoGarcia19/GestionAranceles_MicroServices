namespace Aranceles_UI.Services.Interfaces;

public interface IReportService
{
    Task<byte[]?> GeneratePaymentPdfAsync(int paymentId, object paymentData);
}
