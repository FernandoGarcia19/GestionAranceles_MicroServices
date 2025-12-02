using System.Linq;
using Aranceles_UI.Domain.Dtos;
using Aranceles_UI.Security;
using Aranceles_UI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;

namespace Aranceles_UI.Pages.Establishments
{
    [Authorize(Roles = "Admin,Contador")]
    public class DeleteModel : PageModel
    {
        private readonly IEstablishmentService _establishmentService;
        private readonly IdProtector _idProtector;

        [BindProperty]
        public EstablishmentDto Establishment { get; set; } = new();

        public DeleteModel(IEstablishmentService establishmentService, IdProtector idProtector)
        {
            _establishmentService = establishmentService;
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

            var establishment = await _establishmentService.GetEstablishmentByIdAsync(realId);
            if (establishment == null)
                return RedirectToPage("./Index");

            Establishment = establishment;
            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            var success = await _establishmentService.DeleteEstablishmentAsync(Establishment.Id);
            if (success)
            {
                return RedirectToPage("./Index");
            }
            return Page();
        }
    }
}