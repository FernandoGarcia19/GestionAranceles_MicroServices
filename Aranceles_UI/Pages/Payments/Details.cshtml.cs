using Aranceles_UI.Domain.Dtos;
using Aranceles_UI.Security;
using Aranceles_UI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Aranceles_UI.Pages.Payments
{
    [Authorize(Roles = "Admin,Contador")]
    public class DetailsModel : PageModel
    {
        private readonly IPaymentService _paymentService;
        private readonly IEstablishmentService _establishmentService;
        private readonly ICategoryService _categoryService;
        private readonly IdProtector _idProtector;

        public DetailsModel(
            IPaymentService paymentService, 
            IEstablishmentService establishmentService,
            ICategoryService categoryService,
            IdProtector idProtector)
        {
            _paymentService = paymentService;
            _establishmentService = establishmentService;
            _categoryService = categoryService;
            _idProtector = idProtector;
        }

        public PaymentDto? Payment { get; set; }

        public async Task<IActionResult> OnGet(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return RedirectToPage("./Index");
            }

            var paymentId = _idProtector.UnprotectInt(id);
            if (paymentId <= 0)
            {
                return RedirectToPage("./Index");
            }

            Payment = await _paymentService.GetPaymentByIdAsync(paymentId);

            if (Payment == null)
            {
                return RedirectToPage("./Index");
            }

            // Populate establishment name
            var establishments = await _establishmentService.GetAllEstablishmentsAsync();
            var establishment = establishments.FirstOrDefault(e => e.Id == Payment.EstablishmentId);
            if (establishment != null)
            {
                Payment.EstablishmentName = establishment.Name;
            }

            // Populate category names
            if (Payment.Items != null && Payment.Items.Any())
            {
                var categories = await _categoryService.GetAllCategoriesAsync();
                foreach (var item in Payment.Items)
                {
                    var category = categories.FirstOrDefault(c => c.Id == item.CategoryId);
                    if (category != null)
                    {
                        item.CategoryName = category.Name;
                    }
                }
            }

            return Page();
        }
    }
}
