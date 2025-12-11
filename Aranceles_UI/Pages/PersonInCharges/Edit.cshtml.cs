using System;
using System.Linq;
using Aranceles_UI.Domain.Dtos;
using Aranceles_UI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Aranceles_UI.Security;
using System.ComponentModel.DataAnnotations;


namespace Aranceles_UI.Pages.PersonInCharges
{
    [Authorize(Roles = "Admin")]
    public class EditModel : PageModel
    {
        private readonly IPersonInChargeService _personService;
        private readonly IdProtector _idProtector;
        
        [BindProperty]
        public PersonInChargeDto PersonDto { get; set; } = new();

        [BindProperty]
        [StringLength(2, ErrorMessage = "El complemento no puede tener más de 2 caracteres.")]
        public string? Complemento { get; set; }

        public EditModel(IPersonInChargeService personService, IdProtector idProtector)
        {
            _personService = personService;
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
                return RedirectToPage("../Error");
            }

            var result = await _personService.GetPersonInChargeByIdAsync(realId);
            if (result == null)
            {
                return RedirectToPage("Index");
            }
            
            PersonDto = result;
            Complemento = PersonDto.Ci.Split('-').Skip(1).FirstOrDefault();
            PersonDto.Ci = PersonDto.Ci.Split('-').FirstOrDefault() ?? PersonDto.Ci;
        
            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            if (Complemento != null)
            {
                PersonDto.Ci = PersonDto.Ci + "-" + Complemento.ToString();
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }
            
            var success = await _personService.UpdatePersonInChargeAsync(PersonDto, null, null);
            if (success)
            {
                return RedirectToPage("./Index");
            }
            return Page();
        }
    }
}