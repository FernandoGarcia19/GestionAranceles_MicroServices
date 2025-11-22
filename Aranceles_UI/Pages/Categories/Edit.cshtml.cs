using System;
using System.Linq;
using Aranceles_UI.Domain.Dtos;
using Aranceles_UI.Security;
using Aranceles_UI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;


namespace Aranceles_UI.Pages.Categories
{
    public class EditModel : PageModel
    {
        private readonly ICategoryService _categoryService;
        private readonly IdProtector _idProtector;
        
        [BindProperty]
        public CategoryDto Category { get; set; }
        
        public EditModel(ICategoryService categoryService, IdProtector idProtector)
        {
            _categoryService = categoryService;
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

            Category = await _categoryService.GetCategoryByIdAsync(realId);
            
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

            var success = await _categoryService.UpdateCategoryAsync(Category);
            if (success)
            {
                return RedirectToPage("./Index");
            }
            
            return Page();
        }
    }
}