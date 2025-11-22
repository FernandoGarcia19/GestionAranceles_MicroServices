using Aranceles_UI.Domain.Dtos;
using Aranceles_UI.Services.Interfaces;

namespace Aranceles_UI.Services.Implementations;

public class CategoryService : ICategoryService
{
    private readonly HttpClient _categoryClient;

    public CategoryService(IHttpClientFactory factory)
    {
        _categoryClient = factory.CreateClient("categoryApi");
    }

    public async Task<List<CategoryDto>> GetAllCategoriesAsync()
    {
        return await _categoryClient.GetFromJsonAsync<List<CategoryDto>>("/api/Category") ?? new();
    }

    public async Task<List<CategoryDto>> SearchCategoriesAsync(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return await GetAllCategoriesAsync();
        }

        var term = Uri.EscapeDataString(searchTerm.Trim());
        return await _categoryClient.GetFromJsonAsync<List<CategoryDto>>($"/api/Category/search/{term}") ?? new();
    }

    public async Task<CategoryDto?> GetCategoryByIdAsync(int id)
    {
        return await _categoryClient.GetFromJsonAsync<CategoryDto>($"api/Category/{id}");
    }

    public async Task<bool> CreateCategoryAsync(CategoryDto category)
    {
        var result = await _categoryClient.PostAsJsonAsync("api/Category", category);
        return result.IsSuccessStatusCode;
    }

    public async Task<bool> UpdateCategoryAsync(CategoryDto category)
    {
        var result = await _categoryClient.PutAsJsonAsync($"api/Category/{category.Id}", category);
        return result.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteCategoryAsync(int id)
    {
        var result = await _categoryClient.DeleteAsync($"api/Category/{id}");
        return result.IsSuccessStatusCode;
    }
}

