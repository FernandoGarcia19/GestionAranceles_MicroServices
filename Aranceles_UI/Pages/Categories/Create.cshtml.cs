using Aranceles_UI.Domain.Dtos;
using Aranceles_UI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;

namespace Aranceles_UI.Pages.Categories;

[Authorize(Roles = "Admin")]
public class CreateModel : PageModel
{
    private readonly ICategoryService _categoryService;

    public CreateModel(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }
        
    [BindProperty]
    public CategoryDto Category { get; set; } = new();

    public void OnGet() { }

    public async Task<IActionResult> OnPost()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }
        
        var success = await _categoryService.CreateCategoryAsync(Category);
        if (success)
        {
            return RedirectToPage("./Index");
        }

        return Page();
    }
}