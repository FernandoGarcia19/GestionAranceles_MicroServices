using System;
using System.Collections.Generic;
using Aranceles_UI.Domain.Dtos;
using Aranceles_UI.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Aranceles_UI.Pages.Establishments
{
    public class EditModel : PageModel
    {
        private readonly HttpClient _establishmentClient;
        private readonly HttpClient _personClient;
        private readonly IdProtector _idProtector;

        public List<PersonInChargeDto> PersonsInCharge { get; set; } = new();

        [BindProperty]
        public EstablishmentDto Establishment { get; set; } = new();
        

        
        public EditModel(IHttpClientFactory factory, IdProtector idProtector)
        {
            _establishmentClient = factory.CreateClient("establishmentApi");
            _personClient = factory.CreateClient("personInChargeApi");
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

            var establishment = await _establishmentClient.GetFromJsonAsync<EstablishmentDto>($"api/Establishment/{realId}");
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

            var result = await _establishmentClient.PutAsJsonAsync($"api/Establishment", Establishment);
            if (result.IsSuccessStatusCode)
            {
                return RedirectToPage("./Index");
            }

            await LoadPersonsInCharge();
            return Page();
        }

        private async Task LoadPersonsInCharge()
        {
            var persons = await _personClient.GetFromJsonAsync<List<PersonInChargeDto>>("api/PersonInCharge/") ?? new();
            PersonsInCharge = persons;
        }
    }
}