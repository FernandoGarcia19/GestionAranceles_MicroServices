using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;

namespace Aranceles_UI.Pages;

[Authorize]
public class MyProfile : PageModel
{
    public void OnGet()
    {
        
    }
}