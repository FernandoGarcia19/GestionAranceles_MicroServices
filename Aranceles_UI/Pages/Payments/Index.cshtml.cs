using Aranceles_UI.Domain.Dtos;
using Aranceles_UI.Security;
using Aranceles_UI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Aranceles_UI.Pages.Payments
{
    [Authorize(Roles = "Admin,Contador")]
    public class IndexModel : PageModel
    {
        private readonly IPaymentService _paymentService;
        private readonly IEstablishmentService _establishmentService;
        private readonly ICategoryService _categoryService;
        private readonly IReportService _reportService;
        private readonly IUserService _userService;
        private readonly IdProtector _idProtector;
        
        public IndexModel(
            IPaymentService paymentService, 
            IEstablishmentService establishmentService, 
            ICategoryService categoryService,
            IReportService reportService,
            IUserService userService,
            IdProtector idProtector)
        {
            _paymentService = paymentService;
            _establishmentService = establishmentService;
            _categoryService = categoryService;
            _reportService = reportService;
            _userService = userService;
            _idProtector = idProtector;
        }
        
        [BindProperty(SupportsGet = true)]
        public string SearchTerm { get; set; }

        public List<PaymentDto> Payments { get; set; } = new();

        public async Task OnGet()
        {
            Payments = await _paymentService.GetAllPaymentsAsync();
            await PopulateNames();
        }

        public async Task OnPostSearch()
        {
            Payments = await _paymentService.SearchPaymentsAsync(SearchTerm);
            await PopulateNames();
        }

        public async Task<IActionResult> OnPostDelete(string id)
        {
            var paymentId = _idProtector.UnprotectInt(id);
            
            if (paymentId <= 0)
            {
                TempData["ErrorMessage"] = "ID de pago inválido.";
                return RedirectToPage();
            }

            var success = await _paymentService.DeletePaymentAsync(paymentId);
            
            if (success)
            {
                TempData["SuccessMessage"] = "El pago ha sido eliminado exitosamente.";
            }
            else
            {
                TempData["ErrorMessage"] = "Ocurrió un error al eliminar el pago.";
            }

            return RedirectToPage();
        }

        private async Task PopulateNames()
        {
            if (!Payments.Any()) return;

            var establishments = await _establishmentService.GetAllEstablishmentsAsync();
            var categories = await _categoryService.GetAllCategoriesAsync();

            foreach (var payment in Payments)
            {
                // Populate establishment name
                var establishment = establishments.FirstOrDefault(e => e.Id == payment.EstablishmentId);
                if (establishment != null)
                {
                    payment.EstablishmentName = establishment.Name;
                }

                // Populate category names in items
                if (payment.Items != null)
                {
                    foreach (var item in payment.Items)
                    {
                        var category = categories.FirstOrDefault(c => c.Id == item.CategoryId);
                        if (category != null)
                        {
                            item.CategoryName = category.Name;
                        }
                    }
                }
            }
        }

        public string Protect(int id) => _idProtector.ProtectInt(id);

        public async Task<IActionResult> OnGetGeneratePdfAsync(string id)
        {
            try
            {
                Console.WriteLine($"OnGetGeneratePdfAsync called with id: '{id}'");
                
                if (string.IsNullOrEmpty(id))
                {
                    Console.WriteLine("ID is null or empty");
                    return BadRequest("ID de pago no proporcionado");
                }
                
                var paymentId = _idProtector.UnprotectInt(id);
                Console.WriteLine($"Unprotected payment ID: {paymentId}");
                
                if (paymentId <= 0)
                {
                    Console.WriteLine($"Invalid payment ID after unprotection: {paymentId}");
                    return BadRequest("ID de pago inválido");
                }
                
                // Get payment details from the payments list already loaded in OnGet
                var payment = Payments?.FirstOrDefault(p => p.Id == paymentId);
                if (payment == null)
                {
                    Console.WriteLine($"Payment not found in current list, fetching from API...");
                    // If not in the list, try to fetch it
                    payment = await _paymentService.GetPaymentByIdAsync(paymentId);
                    if (payment == null)
                    {
                        Console.WriteLine($"Payment {paymentId} not found in API");
                        return NotFound();
                    }
                    Console.WriteLine($"Payment {paymentId} fetched from API successfully");
                    
                    // Populate establishment and category names since they won't be set from API
                    var establishments = await _establishmentService.GetAllEstablishmentsAsync();
                    var categories = await _categoryService.GetAllCategoriesAsync();
                    
                    var establishment = establishments.FirstOrDefault(e => e.Id == payment.EstablishmentId);
                    if (establishment != null)
                    {
                        payment.EstablishmentName = establishment.Name;
                    }
                    
                    if (payment.Items != null)
                    {
                        foreach (var item in payment.Items)
                        {
                            var category = categories.FirstOrDefault(c => c.Id == item.CategoryId);
                            if (category != null)
                            {
                                item.CategoryName = category.Name;
                            }
                        }
                    }
                }
                else
                {
                    Console.WriteLine($"Payment {paymentId} found in current list");
                }

                // Calculate total from items or use AmountPaid
                var totalAmount = payment.Items?.Sum(i => i.Quantity * i.UnitPrice) ?? payment.AmountPaid;

                // Prepare items list
                var items = payment.Items?.Select(item => new
                {
                    CategoryName = item.CategoryName ?? "N/A",
                    Quantity = (int)item.Quantity,
                    UnitPrice = (decimal)item.UnitPrice,
                    Subtotal = (decimal)(item.Quantity * item.UnitPrice)
                }).ToList();

                // Get current user name from user service
                var currentUserName = "Usuario Desconocido";
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value 
                               ?? User.FindFirst("nameid")?.Value;
                
                Console.WriteLine($"User ID from claim: {userIdClaim}");
                
                if (!string.IsNullOrEmpty(userIdClaim) && int.TryParse(userIdClaim, out int userId))
                {
                    Console.WriteLine($"Fetching user details for ID: {userId}");
                    var currentUser = await _userService.GetUserByIdAsync(userId);
                    if (currentUser != null)
                    {
                        currentUserName = $"{currentUser.FirstName} {currentUser.LastName}".Trim();
                        Console.WriteLine($"User found: {currentUserName}");
                    }
                    else
                    {
                        Console.WriteLine($"User not found for ID: {userId}");
                    }
                }
                else
                {
                    Console.WriteLine("Could not parse user ID from claims");
                }
                
                Console.WriteLine($"Final user name: {currentUserName}");

                // Prepare report data matching PaymentReport model
                var reportData = new
                {
                    PaymentId = payment.Id,
                    EstablishmentName = payment.EstablishmentName ?? "N/A",
                    PaymentDate = payment.PaymentDate, // Send as DateTime, not string
                    PaymentMethod = payment.PaymentMethod,
                    ReceiptNumber = payment.ReceiptNumber,
                    AmountPaid = totalAmount, // Use AmountPaid instead of TotalAmount
                    Items = items,
                    GeneratedBy = currentUserName,
                    GeneratedDate = DateTime.Now
                };

                Console.WriteLine($"Sending report data: PaymentId={reportData.PaymentId}, ReceiptNumber={reportData.ReceiptNumber}, AmountPaid={reportData.AmountPaid}, Items count={items?.Count ?? 0}");

                // Generate PDF
                var pdfBytes = await _reportService.GeneratePaymentPdfAsync(paymentId, reportData);
                
                if (pdfBytes == null || pdfBytes.Length == 0)
                {
                    return BadRequest("Error al generar el PDF");
                }

                // Return PDF file
                var fileName = $"Recibo_{payment.ReceiptNumber}_{DateTime.Now:yyyyMMddHHmmss}.pdf";
                return File(pdfBytes, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }
    }
}

