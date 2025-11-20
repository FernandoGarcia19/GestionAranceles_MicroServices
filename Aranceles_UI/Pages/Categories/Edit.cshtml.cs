using System;
using System.Linq;
using Aranceles_UI.Domain.Dtos;
using Aranceles_UI.Domain.Models;
using Aranceles_UI.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;


namespace Aranceles_UI.Pages.Categories
{
    public class EditModel : PageModel
    {
        private readonly HttpClient categoryClient;
        private readonly IdProtector _idProtector;
        public CategoryDto Category { get; set; }
        public EditModel(IHttpClientFactory factory, IdProtector idProtector)
        {
            categoryClient = factory.CreateClient("categoryApi");
            _idProtector = idProtector;
        }

        [BindProperty]
        public Category CategoryForm { get; set; } = new();

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

            var result = categoryClient.GetFromJsonAsync<CategoryDto>($"api/category/{realId}");
            
            if (Category == null)
                return RedirectToPage("./Index");

            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var result = categoryClient.PostAsJsonAsync("api/category/edit").Update(Category);
            if (result.IsSuccess)
            {
                return RedirectToPage("./Index");
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error);
            }
            return Page();
        }
    }
}