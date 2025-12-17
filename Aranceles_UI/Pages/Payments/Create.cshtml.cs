using Aranceles_UI.Domain.Dtos;
using Aranceles_UI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;

namespace Aranceles_UI.Pages.Payments
{
    [Authorize(Roles = "Admin,Contador")]
    public class CreateModel : PageModel
    {
        private readonly IPaymentService _paymentService;
        private readonly IEstablishmentService _establishmentService;
        private readonly ICategoryService _categoryService;

        [BindProperty]
        public PaymentDto Payment { get; set; } = new();

        public List<EstablishmentDto> Establishments { get; set; } = new();
        public List<CategoryDto> Categories { get; set; } = new();

        public CreateModel(
            IPaymentService paymentService, 
            IEstablishmentService establishmentService,
            ICategoryService categoryService)
        {
            _paymentService = paymentService;
            _establishmentService = establishmentService;
            _categoryService = categoryService;
        }

        public async Task OnGet()
        {
            Establishments = await _establishmentService.GetAllEstablishmentsAsync();
            Categories = await _categoryService.GetAllCategoriesAsync();
        }

        public async Task<IActionResult> OnPost()
        {
            // Validate that at least one category item exists
            if (Payment.Items == null || !Payment.Items.Any())
            {
                ModelState.AddModelError(string.Empty, "Debe agregar al menos una categoría al pago.");
            }

            // Validate each category item
            if (Payment.Items != null)
            {
                for (int i = 0; i < Payment.Items.Count; i++)
                {
                    var item = Payment.Items[i];
                    if (item.CategoryId <= 0)
                    {
                        ModelState.AddModelError($"Payment.Items[{i}].CategoryId", "Debe seleccionar una categoría válida.");
                    }
                    if (item.Quantity <= 0 || item.Quantity > 255)
                    {
                        ModelState.AddModelError($"Payment.Items[{i}].Quantity", "La cantidad debe estar entre 1 y 255.");
                    }
                    if (item.UnitPrice <= 0 || item.UnitPrice > 16777215)
                    {
                        ModelState.AddModelError($"Payment.Items[{i}].UnitPrice", "El precio unitario debe estar entre 1 y 16777215.");
                    }
                }
            }

            if (!ModelState.IsValid)
            {
                Establishments = await _establishmentService.GetAllEstablishmentsAsync();
                Categories = await _categoryService.GetAllCategoriesAsync();
                return Page();
            }

            var success = await _paymentService.CreatePaymentAsync(Payment);
            if (success)
            {
                TempData["SuccessMessage"] = "Pago registrado exitosamente.";
                return RedirectToPage("./Index");
            }

            // If failed, reload data
            ModelState.AddModelError(string.Empty, "No se pudo registrar el pago. Intente nuevamente.");
            Establishments = await _establishmentService.GetAllEstablishmentsAsync();
            Categories = await _categoryService.GetAllCategoriesAsync();

            return Page();
        }
    }
}



