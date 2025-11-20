using System;
using System.Collections.Generic;
using Aranceles_UI.Domain.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Aranceles_UI.Pages.Establishments
{
    public class CreateModel : PageModel
    {
        private readonly HttpClient _establishmentClient;
        private readonly HttpClient _personClient;

        [BindProperty]
        public EstablishmentDto Establishment { get; set; } = new();

        public List<PersonInChargeDto> PersonsInCharge { get; set; } = new();

        public CreateModel(IHttpClientFactory factory)
        {
            _establishmentClient = factory.CreateClient("establishmentApi");
            _personClient = factory.CreateClient("personInChargeApi");
        }

        public async Task OnGet()
        {
            var persons = await _personClient.GetFromJsonAsync<List<PersonInChargeDto>>("api/PersonInCharge/") ?? new();
            PersonsInCharge = persons;
        }

        public async Task<IActionResult> OnPost()
        {
            if (!ModelState.IsValid)
            {
                var persons = await _personClient.GetFromJsonAsync<List<PersonInChargeDto>>("api/PersonInCharge/") ?? new();
                PersonsInCharge = persons;
                return Page();
            }

            var result = await _establishmentClient.PostAsJsonAsync("api/Establishment/insert", Establishment);
            Console.WriteLine(result);
            if (result.IsSuccessStatusCode)
            {
                return RedirectToPage("./Index");
            }

            // If failed, reload persons
            var personsRetry = await _personClient.GetFromJsonAsync<List<PersonInChargeDto>>("api/PersonInCharge/") ?? new();
            PersonsInCharge = personsRetry;

            return Page();
        }
    }
}
