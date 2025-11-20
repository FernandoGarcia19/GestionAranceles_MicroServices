using System.Collections.Generic;
using Aranceles_UI.Domain.Dtos;
using Aranceles_UI.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Aranceles_UI.Pages.Establishments
{
    public class IndexModel : PageModel
    {
        private readonly HttpClient _establishmentClient;
        private readonly IdProtector _idProtector;

        [BindProperty(SupportsGet = true)]
        public string SearchTerm { get; set; }

        public List<EstablishmentDto> Establishments { get; set; } = new();

        public IndexModel(IHttpClientFactory factory, IdProtector idProtector)
        {
            _establishmentClient = factory.CreateClient("establishmentApi");
            _idProtector = idProtector;
        }

        public async Task OnGet()
        {
            Establishments = await _establishmentClient.GetFromJsonAsync<List<EstablishmentDto>>("api/Establishment") ?? new();
        }

        public async Task OnPostSearch()
        {
            if (!string.IsNullOrWhiteSpace(SearchTerm))
                Establishments = await _establishmentClient.GetFromJsonAsync<List<EstablishmentDto>>($"api/Establishment/search/{SearchTerm}") ?? new();
            else
                Establishments = await _establishmentClient.GetFromJsonAsync<List<EstablishmentDto>>("api/Establishment") ?? new();
        }

        public string Protect(int id) => _idProtector.ProtectInt(id);
    }
}