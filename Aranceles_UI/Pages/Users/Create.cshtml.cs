using System;
using System.ComponentModel.DataAnnotations;
using Aranceles_UI.Domain.Dtos;
using Aranceles_UI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;



namespace Aranceles_UI.Pages.Users
{
    [Authorize(Roles = "Admin")]
    
    public class RegisterDTO{
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public int CreatedBy { get; set; }
    }
    
    public class CreateModel : PageModel
    {
        private readonly IUserService _userService;

        public CreateModel(IUserService userService)
        {
            _userService = userService;
        }

        public string GeneratedUsername { get; set; } = string.Empty;
        
        [BindProperty]
        public RegisterDTO User { get; set; } = new();

        [BindProperty]
        [StringLength(50, ErrorMessage = "El segundo nombre no puede exceder 50 caracteres.")]
        [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]*$", ErrorMessage = "El segundo nombre solo puede contener letras y espacios.")]
        public string? SecondName { get; set; }

        [BindProperty]
        [StringLength(50, ErrorMessage = "El segundo apellido no puede exceder 50 caracteres.")]
        [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]*$", ErrorMessage = "El segundo apellido solo puede contener letras y espacios.")]
        public string? SecondLastName { get; set; }

        public void OnGet() { }

        public async Task<IActionResult> OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var success = await _userService.CreateUserAsync(User, SecondName, SecondLastName);
            if (success)
            {
                GeneratedUsername = User.FirstName + User.LastName;
                return RedirectToPage("./Index");
            }
            return Page();
        }
    }
}
