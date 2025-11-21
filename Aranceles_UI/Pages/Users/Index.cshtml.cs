using System.Collections.Generic;
using Aranceles_UI.Domain.Dtos;
using Aranceles_UI.Security;
using Aranceles_UI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Aranceles_UI.Pages.Users
{
    public class IndexModel : PageModel
    {
        private readonly IUserService _userService;
        private readonly IdProtector _idProtector;

        [BindProperty(SupportsGet = true)]
        public string SearchTerm { get; set; }

        public List<UserDto> Users { get; set; } = new();

        public IndexModel(IUserService userService, IdProtector idProtector)
        {
            _userService = userService;
            _idProtector = idProtector;
        }

        public async Task OnGet()
        {
            Users = await _userService.GetAllUsersAsync();
        }

        public async Task OnPostSearch()
        {
            Users = await _userService.SearchUsersAsync(SearchTerm);
        }

        public string Protect(int id) => _idProtector.ProtectInt(id);
    }
}