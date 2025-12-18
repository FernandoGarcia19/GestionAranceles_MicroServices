using Aranceles_UI.Domain.Dtos;
using Aranceles_UI.Services.Interfaces;

namespace Aranceles_UI.Services.Implementations;

public class CategoryService : BaseHttpService, ICategoryService
{
    public CategoryService(IHttpClientFactory factory, IHttpContextAccessor httpContextAccessor) 
        : base(factory.CreateClient("categoryApi"), httpContextAccessor)
    {
    }

    public async Task<List<CategoryDto>> GetAllCategoriesAsync()
    {
        return await GetFromJsonAuthenticatedAsync<List<CategoryDto>>("/api/Category") ?? new();
    }

    public async Task<List<CategoryDto>> SearchCategoriesAsync(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return await GetAllCategoriesAsync();
        }

        var term = Uri.EscapeDataString(searchTerm.Trim());
        return await GetFromJsonAuthenticatedAsync<List<CategoryDto>>($"/api/Category/search/{term}") ?? new();
    }

    public async Task<CategoryDto?> GetCategoryByIdAsync(int id)
    {
        return await GetFromJsonAuthenticatedAsync<CategoryDto>($"api/Category/{id}");
    }

    public async Task<bool> CreateCategoryAsync(CategoryDto category)
    {
        var result = await PostAsJsonAuthenticatedAsync("api/Category", category);
        
        if (!result.IsSuccessStatusCode)
        {
            var errorContent = await result.Content.ReadAsStringAsync();
            Console.WriteLine($"Category creation failed. Status: {result.StatusCode}");
            Console.WriteLine($"Error response: {errorContent}");
        }
        
        return result.IsSuccessStatusCode;
    }

    public async Task<bool> UpdateCategoryAsync(CategoryDto category)
    {
        var result = await PutAsJsonAuthenticatedAsync($"api/Category/{category.Id}", category);
        Console.WriteLine(result.Content);
        return result.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteCategoryAsync(int id)
    {
        var result = await DeleteAuthenticatedAsync($"api/Category/{id}");
        return result.IsSuccessStatusCode;
    }
}

