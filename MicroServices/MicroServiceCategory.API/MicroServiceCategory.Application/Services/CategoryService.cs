using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MicroServiceCategory.Domain.Entities;
using MicroServiceCategory.Domain.Interfaces;
using MicroServiceCategory.Domain.Patterns;

namespace MicroServiceCategory.Application.Services
{
    public class CategoryService
    {
        private readonly IRepository<Category> _categoryRepository;

        public CategoryService(IRepository<Category> categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<Result<int>> Insert(Category c)
        {
            c.CreatedDate = DateTime.Now;
            c.LastUpdate = DateTime.Now;
            c.Status = 1;

            try
            {
                var id = await _categoryRepository.Insert(c);
                return Result<int>.Success(id);
            }
            catch (Exception ex)
            {
                // Return a server error with the exception message for diagnostics
                return Result<int>.Failure("ServerError", ex.Message);
            }
        }

        public async Task<Result<int>> Update(Category c)
        {
            c.LastUpdate = DateTime.Now;

            try
            {
                var result = await _categoryRepository.Update(c);
                if (result == 0)
                    return Result<int>.Failure("NotFound");

                return Result<int>.Success(result);
            }
            catch (Exception ex)
            {
                return Result<int>.Failure("ServerError", ex.Message);
            }
        }

        public async Task<Result<int>> Delete(Category c)
        {
            try
            {
                var result = await _categoryRepository.Delete(c);
                if (result == 0)
                    return Result<int>.Failure("NotFound");

                return Result<int>.Success(result);
            }
            catch (Exception ex)
            {
                return Result<int>.Failure("ServerError", ex.Message);
            }
        }

        public async Task<Result<List<Category>>> Select()
        {
            try
            {
                var categories = await _categoryRepository.Select();
                return Result<List<Category>>.Success(categories);
            }
            catch (Exception ex)
            {
                return Result<List<Category>>.Failure("ServerError", ex.Message);
            }
        }

        public async Task<Result<List<Category>>> Search(string property)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(property))
                {
                    return Result<List<Category>>.Failure("InvalidInput: El término de búsqueda no puede estar vacío");
                }

                var categories = await _categoryRepository.Search(property);
                return Result<List<Category>>.Success(categories);
            }
            catch (Exception ex)
            {
                return Result<List<Category>>.Failure("ServerError", ex.Message);
            }
        }
        
        public async Task<Result<Category>> SelectById(int id)
        {
            try
            {
                var category = await _categoryRepository.SelectById(id);
                if (category == null)
                    return Result<Category>.Failure("NotFound");

                return Result<Category>.Success(category);
            }
            catch (Exception ex)
            {
                return Result<Category>.Failure("ServerError", ex.Message);
            }
        }

    }
}
