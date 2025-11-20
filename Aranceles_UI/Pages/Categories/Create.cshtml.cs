using Aranceles_UI.Domain.Dtos;
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
    public CategoryDto Category { get; set; } = new();

    public void OnGet() { }

    public async Task<IActionResult> OnPost()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }
        var result = await categoryClient.PostAsJsonAsync("api/Category", Category);
        if (result.IsSuccessStatusCode)
        {
            return RedirectToPage("./Index");
        }

        return Page();
    }
}