using System;
using Aranceles_UI.Domain.Dtos;
using Aranceles_UI.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Aranceles_UI.Pages.Users
{
    public class EditModel : PageModel
    {
        private readonly HttpClient _userClient;
        private readonly IdProtector _idProtector;

        [BindProperty]
        public UserDto User { get; set; } = new();

        public EditModel(IHttpClientFactory factory, IdProtector idProtector)
        {
            _userClient = factory.CreateClient("userApi");
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
                return RedirectToPage("./Index");
            }

            var user = await _userClient.GetFromJsonAsync<UserDto>($"api/User/{realId}");
            if (user == null)
                return RedirectToPage("./Index");

            User = user;
            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var result = await _userClient.PutAsJsonAsync($"api/User/update", User);
            if (result.IsSuccessStatusCode)
            {
                return RedirectToPage("./Index");
            }

            return Page();
        }
    }
}
