using System;
using Aranceles_UI.Domain.Dtos;
using Aranceles_UI.Security;
using Aranceles_UI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Aranceles_UI.Pages.Users
{
    public class EditModel : PageModel
    {
        private readonly IUserService _userService;
        private readonly IdProtector _idProtector;

        [BindProperty]
        public UserDto User { get; set; } = new();

        public EditModel(IUserService userService, IdProtector idProtector)
        {
            _userService = userService;
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

            var user = await _userService.GetUserByIdAsync(realId);
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

            var success = await _userService.UpdateUserAsync(User);
            if (success)
            {
                return RedirectToPage("./Index");
            }

            return Page();
        }
    }
}
