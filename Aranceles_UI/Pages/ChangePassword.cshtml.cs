using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using Aranceles_UI.Domain.Dtos;
using Microsoft.AspNetCore.Authorization;


namespace Aranceles_UI.Pages
{
    public class ChangePasswordRespondesDTO
    {
        public bool Ok { get; set; }
        public string Error { get; set; }
    }
    public class ChangePasswordDTO
    {
        public int UserId { get; set; }
        public string CurrentPassword { get; set;  }
        public string NewPassword{ get; set; }
    }
    
    [Authorize]
    public class ChangePasswordModel : PageModel
    {
        private readonly HttpClient _authService;

        public ChangePasswordModel(IHttpClientFactory clientFactory)
        {
            _authService = clientFactory.CreateClient("userApi");
        }

        [BindProperty]
        [Required(ErrorMessage = "La contraseña actual es requerida")]
        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; } = string.Empty;

        [BindProperty]
        [Required(ErrorMessage = "La nueva contraseña es requerida")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; } = string.Empty;

        [BindProperty]
        [Required(ErrorMessage = "Debe confirmar la contraseña")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Las contraseñas no coinciden")]
        public string ConfirmPassword { get; set; } = string.Empty;
        public bool IsFirstLogin { get; private set; }

        [BindProperty(SupportsGet = true)]
        public bool? ForceFirstLogin { get; set; }

        public async Task<IActionResult> OnGet()
        {
            var userId = User.FindFirst(JwtRegisteredClaimNames.NameId)?.Value;
            if (userId == null) return RedirectToPage("/Login");

            var user = await _authService.GetFromJsonAsync<UserDto>($"api/User/getById/{userId}");
            if (user == null) return RedirectToPage("/Login");
            
            IsFirstLogin = ForceFirstLogin ?? (user.FirstLogin == 1);
            
            ViewData["HideSidebar"] = IsFirstLogin;

            

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var userId = User.FindFirst(JwtRegisteredClaimNames.NameId)?.Value;
            if (userId == null) return RedirectToPage("/Login");

            var user = await _authService.GetFromJsonAsync<UserDto>($"api/User/getById/{userId}");
            if (user == null) return RedirectToPage("/Login");

            IsFirstLogin = ForceFirstLogin ?? (user.FirstLogin == 1);
            
            ViewData["HideSidebar"] = IsFirstLogin;

            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                var changePasswordDto = new ChangePasswordDTO()
                {
                    UserId = int.Parse(userId),
                    CurrentPassword = CurrentPassword,
                    NewPassword = NewPassword
                };
                
                var response = await _authService.PostAsJsonAsync("api/User/change-password", changePasswordDto);
                var result = await response.Content.ReadFromJsonAsync<ChangePasswordRespondesDTO>();
                
                if (result?.Ok == true)
                {
                    TempData["SuccessMessage"] = "Contraseña cambiada exitosamente.";
                    return RedirectToPage("/Index");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, result?.Error ?? "Error al cambiar la contraseña.");
                    return Page();
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Error al procesar la solicitud.");
                return Page();
            }
        }

    }
}