using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using Aranceles_UI.Domain.Dtos;


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

        public async Task<IActionResult> OnGet()
        {
            var name = User.Identity.Name;
            var token = User.FindFirst("access_token")?.Value;
            var userId = User.FindFirst(JwtRegisteredClaimNames.NameId)?.Value;
            if (userId == null) return RedirectToPage("/Login");

            var user =  await _authService.GetFromJsonAsync<UserDto>($"api/User/getById/{userId}");
            if (user == null) return RedirectToPage("/Login");
            
            
            IsFirstLogin = (user.FirstLogin == 0);

            ViewData["HideSidebar"] = IsFirstLogin;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var name = User.Identity.Name;
            var token = User.FindFirst("access_token")?.Value;
            var userId = User.FindFirst(JwtRegisteredClaimNames.NameId)?.Value;
            if (userId == null) return RedirectToPage("/Login");

            var user =  await _authService.GetFromJsonAsync<UserDto>($"api/User/getById/{userId}");
            if (user == null) return RedirectToPage("/Login");

            IsFirstLogin = (user.FirstLogin == 0);
            ViewData["HideSidebar"] = IsFirstLogin;

            
            if (!ModelState.IsValid)
            {
                return Page();
            }

            (bool ok, string? error) result;

            if (user.FirstLogin == 1)
            {
                var cpdto = new ChangePasswordDTO()
                {
                    UserId = int.Parse(userId),
                    CurrentPassword = CurrentPassword,
                    NewPassword = NewPassword
                };
                var res = await _authService.PostAsJsonAsync<ChangePasswordDTO>("api/User/change-password", cpdto);
                var q = await res.Content.ReadFromJsonAsync<ChangePasswordRespondesDTO>();
                RedirectToPage("/Index");
            }
            
            return RedirectToPage("/Index");
        }

    }
}