using System.Linq;
using Aranceles_UI.Domain.Dtos;
using Aranceles_UI.Security;
using Aranceles_UI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Aranceles_UI.Pages.Categories
{
    public class DeleteModel : PageModel
    {
        private readonly ICategoryService _categoryService;
        private readonly IdProtector _idProtector;

        public DeleteModel(ICategoryService categoryService, IdProtector idProtector)
        {
            _categoryService = categoryService;
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

            Category = await _categoryService.GetCategoryByIdAsync(RealId);
            
            if (Category == null)
                return RedirectToPage("./Index");

            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            var success = await _categoryService.DeleteCategoryAsync(RealId);
            if (success)
            {
                return RedirectToPage("./Index");
            }
            return Page();
        }
    }
}