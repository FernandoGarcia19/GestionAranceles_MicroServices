using System.Collections.Generic;
using Aranceles_UI.Domain.Dtos;
using Aranceles_UI.Security;
using Aranceles_UI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;

namespace Aranceles_UI.Pages.Establishments
{
    [Authorize(Roles = "Admin,Contador")]
    public class IndexModel : PageModel
    {
        private readonly IEstablishmentService _establishmentService;
        private readonly IdProtector _idProtector;

        [BindProperty(SupportsGet = true)]
        public string SearchTerm { get; set; }

        public List<EstablishmentDto> Establishments { get; set; } = new();

        public IndexModel(IEstablishmentService establishmentService, IdProtector idProtector)
        {
            _establishmentService = establishmentService;
            _idProtector = idProtector;
        }

        public async Task OnGet()
        {
            Establishments = await _establishmentService.GetAllEstablishmentsAsync();
        }

        public async Task OnPostSearch()
        {
            Establishments = await _establishmentService.SearchEstablishmentsAsync(SearchTerm);
        }

        public string Protect(int id) => _idProtector.ProtectInt(id);
    }
}