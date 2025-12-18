using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using Aranceles_UI.Domain.Dtos;
using Aranceles_UI.Security;
using Aranceles_UI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace Aranceles_UI.Pages.PersonInCharges
{
    [Authorize(Roles = "Admin")]
    public class CreateModel : PageModel
    {
        private readonly IPersonInChargeService _personService;

        [BindProperty]
        public PersonInChargeDto Person { get; set; } = new();

        [BindProperty]
        [StringLength(50, ErrorMessage = "El segundo nombre no puede exceder 50 caracteres.")]
        [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]*$", ErrorMessage = "El segundo nombre solo puede contener letras y espacios.")]
        public string? SecondName { get; set; }

        [BindProperty]
        [StringLength(50, ErrorMessage = "El segundo apellido no puede exceder 50 caracteres.")]
        [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]*$", ErrorMessage = "El segundo apellido solo puede contener letras y espacios.")]
        public string? SecondLastName { get; set; }
        
        [BindProperty]
        [StringLength(3, ErrorMessage = "El complemento solo puede tener 2 caracteres")]
        public string? Complemento { get; set; }

        public CreateModel(IPersonInChargeService personService)
        {
            _personService = personService;
        }

        public void OnGet() { }

        public async Task<IActionResult> OnPost()
        {
            if (Complemento != null)
            {
                Person.Ci = Person.Ci + "-" + Complemento.ToString();
            }
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var success = await _personService.CreatePersonInChargeAsync(Person, SecondName, SecondLastName);
            if (success)
            {
                return RedirectToPage("./Index");
            }
            return Page();
        }
    }
}
