using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using Aranceles_UI.Domain.Dtos;
using Aranceles_UI.Security;

namespace Aranceles_UI.Pages.PersonInCharges
{
    public class CreateModel : PageModel
    {
        private readonly HttpClient personClient;

        [BindProperty]
        public PersonInChargeDto Person { get; set; } = new();

        [BindProperty]
        [StringLength(50, ErrorMessage = "El segundo nombre no puede exceder 50 caracteres.")]
        [RegularExpression(@"^[a-zA-Z������������\s]*$", ErrorMessage = "El segundo nombre solo puede contener letras y espacios.")]
        public string? SecondName { get; set; }

        [BindProperty]
        [StringLength(50, ErrorMessage = "El segundo apellido no puede exceder 50 caracteres.")]
        [RegularExpression(@"^[a-zA-Z������������\s]*$", ErrorMessage = "El segundo apellido solo puede contener letras y espacios.")]
        public string? SecondLastName { get; set; }

        public CreateModel(IHttpClientFactory factory)
        {
            personClient = factory.CreateClient("personInChargeApi");
        }

        public void OnGet() { }

        public async Task<IActionResult> OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var fullFirstName = Person.FirstName.Trim();
            if (!string.IsNullOrWhiteSpace(SecondName))
            {
                fullFirstName += " " + SecondName.Trim();
            }

            var fullLastName = Person.LastName.Trim();
            if (!string.IsNullOrWhiteSpace(SecondLastName))
            {
                fullLastName += " " + SecondLastName.Trim();
            }

            Person.FirstName = fullFirstName;
            Person.LastName = fullLastName;

            var result = await personClient.PostAsJsonAsync<PersonInChargeDto>("api/PersonInCharge/insert", Person);
            if (result.IsSuccessStatusCode)
            {
                return RedirectToPage("./Index");
            }
            return Page();
        }
    }
}
