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
        
        Console.WriteLine($"Creating category: Name={Category.Name}, Description={Category.Description}, BaseAmount={Category.BaseAmount}, CreatedBy={Category.CreatedBy}");

        if (!ModelState.IsValid)
        {
            Console.WriteLine("ModelState is invalid:");
            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
            {
                Console.WriteLine($"  - {error.ErrorMessage}");
            }
            return Page();
        }
        
        var success = await _categoryService.CreateCategoryAsync(Category);
        Console.WriteLine($"Create category result: {success}");
        
        if (success)
        {
            return RedirectToPage("./Index");
        }

        TempData["ErrorMessage"] = "Error al crear la categoría. Verifique los datos.";
        return Page();
    }
}