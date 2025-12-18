using System;
using System.Collections.Generic;
using Aranceles_UI.Domain.Dtos;
using Aranceles_UI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;

namespace Aranceles_UI.Pages.Establishments
{
    [Authorize(Roles = "Admin,Contador")]
    public class CreateModel : PageModel
    {
        private readonly IEstablishmentService _establishmentService;
        private readonly IPersonInChargeService _personService;

        [BindProperty]
        public EstablishmentDto Establishment { get; set; } = new();

        public List<PersonInChargeDto> PersonsInCharge { get; set; } = new();

        public CreateModel(IEstablishmentService establishmentService, IPersonInChargeService personService)
        {
            _establishmentService = establishmentService;
            _personService = personService;
        }

        public async Task OnGet()
        {
            PersonsInCharge = await _personService.GetAllPersonsInChargeAsync();
        }

        public async Task<IActionResult> OnPost()
        {
            if (!ModelState.IsValid)
            {
                PersonsInCharge = await _personService.GetAllPersonsInChargeAsync();
                return Page();
            }

            var success = await _establishmentService.CreateEstablishmentAsync(Establishment);
            if (success)
            {
                return RedirectToPage("./Index");
            }

            // If failed, reload persons
            PersonsInCharge = await _personService.GetAllPersonsInChargeAsync();

            return Page();
        }
    }
}
