using System;
using System.Collections.Generic;
using Aranceles_UI.Domain.Dtos;
using Microsoft.AspNetCore.Mvc.RazorPages;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Aranceles_UI.Security;

namespace Aranceles_UI.Pages.PersonInCharges
{
    public class IndexModel : PageModel
    {
        private readonly HttpClient personClient;
        private readonly IdProtector _idProtector;

        [BindProperty]
        public string SearchTerm { get; set; }
        public List<PersonInChargeDto> Persons { get; set; } = new();
        public IndexModel(IHttpClientFactory factory, IdProtector idProtector)
        {
            personClient = factory.CreateClient("personInChargeApi");
            _idProtector = idProtector;
        }

        public async Task OnGet()
        {
            Persons = await personClient.GetFromJsonAsync<List<PersonInChargeDto>>("api/PersonInCharge/");
        }

        public void OnPost()
        {
        }

        public string Protect(int id) => _idProtector.ProtectInt(id);
    }
}