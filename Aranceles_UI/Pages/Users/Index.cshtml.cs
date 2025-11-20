using System.Collections.Generic;
using Aranceles_UI.Domain.Dtos;
using Aranceles_UI.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Aranceles_UI.Pages.Users
{
    public class IndexModel : PageModel
    {
        private readonly HttpClient _userClient;
        private readonly IdProtector _idProtector;

        [BindProperty(SupportsGet = true)]
        public string SearchTerm { get; set; }

        public List<UserDto> Users { get; set; } = new();

        public IndexModel(IHttpClientFactory factory, IdProtector idProtector)
        {
            _userClient = factory.CreateClient("userApi");
            _idProtector = idProtector;
        }

        public async Task OnGet()
        {
            Users = await _userClient.GetFromJsonAsync<List<UserDto>>("api/User/select") ?? new();
        }

        public async Task OnPostSearch()
        {
            if (!string.IsNullOrWhiteSpace(SearchTerm))
                Users = await _userClient.GetFromJsonAsync<List<UserDto>>($"api/User/search/{SearchTerm}") ?? new();
            else
                Users = await _userClient.GetFromJsonAsync<List<UserDto>>("api/User") ?? new();
        }

        public string Protect(int id) => _idProtector.ProtectInt(id);
    }
}