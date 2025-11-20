using System;
using System.Collections.Generic;
using Aranceles_UI.Domain.Dtos;
using Aranceles_UI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc.RazorPages;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Aranceles_UI.Security;

namespace Aranceles_UI.Pages.PersonInCharges
{
    public class IndexModel : PageModel
    {
        private readonly IPersonInChargeService _personService;
        private readonly IdProtector _idProtector;

        [BindProperty]
        public string SearchTerm { get; set; }
        public List<PersonInChargeDto> Persons { get; set; } = new();
        
        public IndexModel(IPersonInChargeService personService, IdProtector idProtector)
        {
            _personService = personService;
            _idProtector = idProtector;
        }

        public async Task OnGet()
        {
            Persons = await _personService.GetAllPersonsInChargeAsync();
        }

        public async Task OnPostAsync()
        {
            Persons = await _personService.SearchPersonsInChargeAsync(SearchTerm);
        }


        public string Protect(int id) => _idProtector.ProtectInt(id);
    }
}