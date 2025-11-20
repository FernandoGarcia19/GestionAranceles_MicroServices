using System;
using System.Collections.Generic;
using Aranceles_UI.Domain.Dtos;
using Aranceles_UI.Security;
using Aranceles_UI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Aranceles_UI.Pages.Establishments
{
    public class EditModel : PageModel
    {
        private readonly IEstablishmentService _establishmentService;
        private readonly IPersonInChargeService _personService;
        private readonly IdProtector _idProtector;

        public List<PersonInChargeDto> PersonsInCharge { get; set; } = new();

        [BindProperty]
        public EstablishmentDto Establishment { get; set; } = new();
        
        public EditModel(IEstablishmentService establishmentService, IPersonInChargeService personService, IdProtector idProtector)
        {
            _establishmentService = establishmentService;
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

            var establishment = await _establishmentService.GetEstablishmentByIdAsync(realId);
            if (establishment == null)
                return RedirectToPage("./Index");

            Establishment = establishment;
            await LoadPersonsInCharge();

            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            if (!ModelState.IsValid)
            {
                await LoadPersonsInCharge();
                return Page();
            }

            var success = await _establishmentService.UpdateEstablishmentAsync(Establishment);
            if (success)
            {
                return RedirectToPage("./Index");
            }

            await LoadPersonsInCharge();
            return Page();
        }

        private async Task LoadPersonsInCharge()
        {
            PersonsInCharge = await _personService.GetAllPersonsInChargeAsync();
        }
    }
}