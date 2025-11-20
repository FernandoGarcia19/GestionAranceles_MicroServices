using System.Collections.Generic;
using Aranceles_UI.Domain.Dtos;
using Aranceles_UI.Security;
using Microsoft.AspNetCore.Mvc.RazorPages;

using Microsoft.AspNetCore.Mvc;

namespace Aranceles_UI.Pages.Categories
{
    public class IndexModel : PageModel
    {
        private readonly HttpClient categoryClient;
        private readonly IdProtector _idProtector;
        
        public IndexModel(IHttpClientFactory factory, IdProtector idProtector)
        {
            categoryClient = factory.CreateClient("categoryApi");
            _idProtector = idProtector;
        }
        
        [BindProperty(SupportsGet = true)]
        public string SearchTerm { get; set; }


        public List<CategoryDto> Categories { get; set; } = new();

        

        public async Task OnGet()
        {
            Categories = await categoryClient.GetFromJsonAsync<List<CategoryDto>>("/api/Category");
        }

        public async Task OnPostSearch()
        {
            if(!string.IsNullOrWhiteSpace(SearchTerm))
                await categoryClient.GetFromJsonAsync<List<CategoryDto>>($"/api/Category/search/{SearchTerm}");
                
            // Categories = string.IsNullOrWhiteSpace(SearchTerm) ? 
            //     await categoryClient.GetFromJsonAsync<List<CategoryDto>>("/api/Category") : 
            //     await categoryClient.GetFromJsonAsync<List<CategoryDto>>("/api/category/search");
                
        }
        public string Protect(int id) => _idProtector.ProtectInt(id);
    }
}