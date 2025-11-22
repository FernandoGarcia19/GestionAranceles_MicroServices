using System.Linq;
using Aranceles_UI.Domain.Dtos;
using Aranceles_UI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using Aranceles_UI.Security;



namespace Aranceles_UI.Pages.PersonInCharges
{
    public class DeleteModel : PageModel
    {
        private readonly IPersonInChargeService _personService;
        private readonly IdProtector _idProtector;

        [BindProperty]
        public PersonInChargeDto Person { get; set; } = new();

        public DeleteModel(IPersonInChargeService personService, IdProtector idProtector)
        {
            _personService = personService;
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

            var result = await _personService.GetPersonInChargeByIdAsync(realId);
            if (result == null)
            {
                return NotFound();
            }

            Person = result;
            return Page();
        }
        
        public async Task<IActionResult> OnPost()
        {
            var success = await _personService.DeletePersonInChargeAsync(Person.Id);
            if (success)
            {
                return RedirectToPage("./Index");
            }
            return Page();
        }

    }
}