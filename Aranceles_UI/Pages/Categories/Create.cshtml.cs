using Aranceles_UI.Domain.Dtos;
using Aranceles_UI.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Aranceles_UI.Pages.Categories;

public class CreateModel : PageModel
{
    private readonly HttpClient categoryClient;

    public CreateModel(IHttpClientFactory factory)
    {
        categoryClient = factory.CreateClient("categoryApi");
    }
        
    [BindProperty]
    public Category Category { get; set; } = new();

    public void OnGet() { }

    public async Task<IActionResult> OnPost()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }
        var result = categoryClient.PostAsJsonAsync("api/category/insert", Category);
        return RedirectToPage("./Index");
    }
}