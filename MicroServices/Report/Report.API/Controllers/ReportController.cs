using Microsoft.AspNetCore.Mvc;
using Report.App.Service;
using Report.Dom.Model;

namespace Report.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ReportController : ControllerBase
{
    private readonly PdfGeneratorService _pdfService;

    public ReportController(PdfGeneratorService pdfService)
    {
        _pdfService = pdfService;
    }

    [HttpPost("payment-pdf")]
    public IActionResult GeneratePaymentPdf([FromBody] PaymentReport payment)
    {
        if (payment == null)
        {
            return BadRequest(new { errors = new[] { "Payment data is required" } });
        }

        try
        {
            var pdfBytes = _pdfService.GeneratePaymentPdf(payment);
            
            var fileName = $"Recibo_{payment.ReceiptNumber}_{DateTime.Now:yyyyMMddHHmmss}.pdf";
            
            return File(pdfBytes, "application/pdf", fileName);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { errors = new[] { ex.Message } });
        }
    }
}
