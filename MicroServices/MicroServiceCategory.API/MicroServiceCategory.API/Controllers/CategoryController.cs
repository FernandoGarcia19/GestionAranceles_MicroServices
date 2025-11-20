using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MicroServiceCategory.Application.Services;
using MicroServiceCategory.Domain.Entities;
using System.Linq;
using System.Collections.Generic;

namespace MicroServiceCategory.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly CategoryService _categoryService;

        public CategoryController(CategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        private IActionResult MapFailure(IEnumerable<string> errors)
        {
            var errorList = errors.ToList();
            if (errorList.Count == 0)
                return StatusCode(500, new { errors = new[] { "UnknownError" } });

            // Validation errors start with "InvalidInput" or contain typical validation messages
            if (errorList.Any(e => e.StartsWith("InvalidInput") || e.Contains("El ") || e.Contains("must be") || e.Contains("debe")))
                return BadRequest(new { errors = errorList });

            if (errorList.Any(e => e.Equals("NotFound") || e.Contains("NoRowsAffected")))
                return NotFound(new { errors = errorList });

            // DB or other server errors
            return StatusCode(500, new { errors = errorList });
        }

        // GET: api/category

        [HttpGet]
        public async Task<IActionResult> Select()
        {
            var result = await _categoryService.Select();
            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }
            else
            {
                return MapFailure(result.Errors);
            }
        }

        // GET: api/category/search/comida
        [HttpGet("search/{property}")]
        public async Task<IActionResult> Search(string property)
        {
            var result = await _categoryService.Search(property);
            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }
            else
            {
                return MapFailure(result.Errors);
            }
        }

        // POST: api/category
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Category category)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => "InvalidInput: " + (string.IsNullOrEmpty(e.ErrorMessage) ? e.Exception?.Message : e.ErrorMessage));
                return MapFailure(errors);
            }

            var result = await _categoryService.Insert(category);

            if (result.IsSuccess)
            {
                return CreatedAtAction(
                    nameof(Select),
                    new { id = result.Value },
                    new
                    {
                        id = result.Value,
                        message = "Categoria creada exitosamente",
                        data = category
                    }
                );
            }
            else
            {
                return MapFailure(result.Errors);
            }
        }

        // PUT: api/category/5

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Category category)
        {
            category.Id = id;

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => "InvalidInput: " + (string.IsNullOrEmpty(e.ErrorMessage) ? e.Exception?.Message : e.ErrorMessage));
                return MapFailure(errors);
            }

            var result = await _categoryService.Update(category);

            if (result.IsSuccess)
            {
                return Ok(new
                {
                    message = "Categoría actualizada exitosamente",
                    data = category
                });
            }
            else
            {
                return MapFailure(result.Errors);
            }
        }

        // DELETE: api/category/5

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {

            var category = new Category { Id = id };
            var result = await _categoryService.Delete(category);

            if (result.IsSuccess)
            {
                return Ok(new
                {
                    message = $"Categoría con ID {id} eliminada exitosamente"
                });
            }
            else
            {
                return MapFailure(result.Errors);
            }
        }
    }
}
