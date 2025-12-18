using System.Collections.Generic;
using Aranceles_UI.Domain.Dtos;
using Aranceles_UI.Security;
using Aranceles_UI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Aranceles_UI.Pages.Categories
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly ICategoryService _categoryService;
        private readonly IdProtector _idProtector;
        
        public IndexModel(ICategoryService categoryService, IdProtector idProtector)
        {
            _categoryService = categoryService;
            _idProtector = idProtector;
        }
        
        [BindProperty(SupportsGet = true)]
        public string SearchTerm { get; set; }


        public List<CategoryDto> Categories { get; set; } = new();

        

        public async Task OnGet()
        {
            Categories = await _categoryService.GetAllCategoriesAsync();
        }

        public async Task OnPostSearch()
        {
            Categories = await _categoryService.SearchCategoriesAsync(SearchTerm);
        }

        public string Protect(int id) => _idProtector.ProtectInt(id);
    }
}