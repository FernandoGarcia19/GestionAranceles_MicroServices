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
        private readonly IdProtector _idProtector;
        
        public IndexModel(
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
    }
}

