using System;
using System.Linq;
using Aranceles_UI.Domain.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using Aranceles_UI.Security;


namespace Aranceles_UI.Pages.PersonInCharges
{
    public class EditModel : PageModel
    {
        private readonly HttpClient personClient;
        private readonly IdProtector _idProtector;
        
        [BindProperty]
        public PersonInChargeDto PersonDto { get; set; } = new();

        public EditModel(IHttpClientFactory factory, IdProtector idProtector)
        {
            personClient = factory.CreateClient("personInChargeApi");
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

            var result = await personClient.GetFromJsonAsync<PersonInChargeDto>($"api/PersonInCharge/{realId}");
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
            
            var result = await personClient.PutAsJsonAsync($"api/PersonInCharge", PersonDto);
            if (result.IsSuccessStatusCode)
            {
                return RedirectToPage("./Index");
            }
            return Page();
        }
    }
}