using System;
using System.ComponentModel.DataAnnotations;
using Aranceles_UI.Domain.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Aranceles_UI.Pages.Users
{
    public class CreateModel : PageModel
    {
        private readonly HttpClient _userClient;

        public CreateModel(IHttpClientFactory factory)
        {
            _userClient = factory.CreateClient("userApi");
        }

        [BindProperty]
        public UserDto User { get; set; } = new();

        [BindProperty]
        [StringLength(50, ErrorMessage = "El segundo nombre no puede exceder 50 caracteres.")]
        [RegularExpression(@"^[a-zA-Z������������\s]*$", ErrorMessage = "El segundo nombre solo puede contener letras y espacios.")]
        public string? SecondName { get; set; }

        [BindProperty]
        [StringLength(50, ErrorMessage = "El segundo apellido no puede exceder 50 caracteres.")]
        [RegularExpression(@"^[a-zA-Z������������\s]*$", ErrorMessage = "El segundo apellido solo puede contener letras y espacios.")]
        public string? SecondLastName { get; set; }

        public void OnGet() { }

        public async Task<IActionResult> OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var fullFirstName = User.FirstName.Trim();
            if (!string.IsNullOrWhiteSpace(SecondName))
            {
                fullFirstName += " " + SecondName.Trim();
            }

            var fullLastName = User.LastName.Trim();
            if (!string.IsNullOrWhiteSpace(SecondLastName))
            {
                fullLastName += " " + SecondLastName.Trim();
            }

            User.FirstName = fullFirstName;
            User.LastName = fullLastName;

            var result = await _userClient.PostAsJsonAsync("api/User", User);
            if (result.IsSuccessStatusCode)
            {
                return RedirectToPage("./Index");
            }
            return Page();
        }
    }
}
