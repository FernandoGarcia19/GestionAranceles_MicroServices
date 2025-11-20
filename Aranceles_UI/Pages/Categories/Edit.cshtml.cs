using System;
using System.Linq;
using Aranceles_UI.Domain.Dtos;
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
        public CategoryDto CategoryForm { get; set; } = new();

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

            Category = await categoryClient.GetFromJsonAsync<CategoryDto>($"api/Category/{realId}");
            
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

            var result = await categoryClient.PutAsJsonAsync($"api/Category/{Category.Id}", Category);
            if (result.IsSuccessStatusCode)
            {
                return RedirectToPage("./Index");
            }
            
            return Page();
        }
    }
}