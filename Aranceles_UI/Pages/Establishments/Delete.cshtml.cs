using System.Linq;
using Aranceles_UI.Domain.Dtos;
using Aranceles_UI.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Aranceles_UI.Pages.Establishments
{
    public class DeleteModel : PageModel
    {
        private readonly HttpClient _establishmentClient;
        private readonly IdProtector _idProtector;

        [BindProperty]
        public EstablishmentDto Establishment { get; set; } = new();

        public DeleteModel(IHttpClientFactory factory, IdProtector idProtector)
        {
            _establishmentClient = factory.CreateClient("establishmentApi");
            _idProtector = idProtector;
        }

        public async Task<IActionResult> OnGet(string id)
        {
            int realId;
            try
            {
                realId = _idProtector.UnprotectInt(id);
            }
            catch
            {
                return RedirectToPage("./Index");
            }

            var establishment = await _establishmentClient.GetFromJsonAsync<EstablishmentDto>($"api/Establishment/{realId}");
            if (establishment == null)
                return RedirectToPage("./Index");

            Establishment = establishment;
            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            var result = await _establishmentClient.DeleteAsync($"api/Establishment/{Establishment.Id}");
            if (result.IsSuccessStatusCode)
            {
                return RedirectToPage("./Index");
            }
            return Page();
        }
    }
}