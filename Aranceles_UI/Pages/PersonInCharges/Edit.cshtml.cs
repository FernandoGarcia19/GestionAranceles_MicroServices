using System;
using System.Linq;
using Aranceles_UI.Domain.Dtos;
using Aranceles_UI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Aranceles_UI.Security;


namespace Aranceles_UI.Pages.PersonInCharges
{
    [Authorize(Roles = "Admin")]
    public class EditModel : PageModel
    {
        private readonly IPersonInChargeService _personService;
        private readonly IdProtector _idProtector;
        
        [BindProperty]
        public PersonInChargeDto PersonDto { get; set; } = new();

        public EditModel(IPersonInChargeService personService, IdProtector idProtector)
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
                return RedirectToPage("../Error");
            }

            var result = await _personService.GetPersonInChargeByIdAsync(realId);
            if (result == null)
            {
                return RedirectToPage("Index");
            }
            
            PersonDto = result;
            return Page();
        }

        public async Task<IActionResult> OnPost()
        {

            if (!ModelState.IsValid)
            {
                return Page();
            }
            
            var success = await _personService.UpdatePersonInChargeAsync(PersonDto, null, null);
            if (success)
            {
                return RedirectToPage("./Index");
            }
            return Page();
        }
    }
}