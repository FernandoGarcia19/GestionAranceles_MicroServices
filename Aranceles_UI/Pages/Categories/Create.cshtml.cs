using Aranceles_UI.Domain.Dtos;
using Aranceles_UI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

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
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier)
                   ?? User.FindFirstValue(JwtRegisteredClaimNames.NameId);

        Category.CreatedBy = int.Parse(userIdStr);

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