using Aranceles_UI.Domain.Dtos;

namespace Aranceles_UI.Services.Interfaces;

public interface ICategoryService
{
    Task<List<CategoryDto>> GetAllCategoriesAsync();
    Task<List<CategoryDto>> SearchCategoriesAsync(string searchTerm);
    Task<CategoryDto?> GetCategoryByIdAsync(int id);
    Task<bool> CreateCategoryAsync(CategoryDto category);
    Task<bool> UpdateCategoryAsync(CategoryDto category);
    Task<bool> DeleteCategoryAsync(int id);
}

