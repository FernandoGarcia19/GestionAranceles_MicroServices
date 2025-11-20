using System.Linq;
using Aranceles_UI.Domain.Dtos;
using Aranceles_UI.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Aranceles_UI.Pages.Categories
{
    public class DeleteModel : PageModel
    {
        private readonly HttpClient categoryClient;
        private readonly IdProtector _idProtector;

        public DeleteModel(IHttpClientFactory factory, IdProtector idProtector)
        {
            categoryClient = factory.CreateClient("categoryApi");
            _idProtector = idProtector;
        }

        [BindProperty]
        public CategoryDto Category { get; set; } = new();
        
        [BindProperty]
        public int RealId { get; set; }

        public async Task<IActionResult> OnGet(string id)
        {
            try
            {
                RealId = _idProtector.UnprotectInt(id);
            }
            catch
            {
                return RedirectToPage("./Index");
            }

            Category = await categoryClient.GetFromJsonAsync<CategoryDto>($"api/Category/{RealId}");
            
            if (Category == null)
                return RedirectToPage("./Index");

            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            var result = await categoryClient.DeleteAsync($"api/Category/{RealId}");
            if (result.IsSuccessStatusCode)
            {
                return RedirectToPage("./Index");
            }
            return Page();
        }
    }
}