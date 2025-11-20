using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MicroServiceCategory.Application.Services;
using MicroServiceCategory.Domain.Entities;

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

        // GET: api/category

        [HttpGet]
        public async Task<ActionResult<List<Category>>> Select()
        {
            var categories = await _categoryService.Select();
            if (categories.IsSuccess)
            {
                return Ok(categories.Value);
            }
            else
            {
                return StatusCode(500, new
                {
                    message = "Error al obtener las categorías",
                    error = categories.Errors
                });
            }
        }

        // GET: api/category/search/comida

        [HttpGet("search/{property}")]
        public async Task<ActionResult<List<Category>>> Search(string property)
        {
            var categories = await _categoryService.Search(property);
            if (categories.IsSuccess)
            {
                return Ok(categories.Value);
            }
            else
            {
                return StatusCode(500, new
                {
                    message = "Error al buscar categorías",
                    error = categories.Errors
                });
            }
        }

        // POST: api/category

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] Category category)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
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
                return StatusCode(500, new
                {
                    message = "Error al crear la categoría",
                    error = result.Errors
                });
            }
        }

        // PUT: api/category/5

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] Category category)
        {
            category.Id = id;

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
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
                if (result.Errors.Any(e => e.Contains("No se encontró")))
                {
                    return NotFound(new
                    {
                        message = $"Categoría con ID {id} no encontrada"
                    });
                }
                else
                {
                    return StatusCode(500, new
                    {
                        message = "Error al actualizar la categoría",
                        error = result.Errors
                    });
                }
            }
        }

        // DELETE: api/category/5

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
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
                if (result.Errors.Any(e => e.Contains("No se encontró")))
                {
                    return NotFound(new
                    {
                        message = $"Categoría con ID {id} no encontrada"
                    });
                }
                else
                {
                    return StatusCode(500, new
                    {
                        message = "Error al eliminar la categoría",
                        error = result.Errors
                    });
                }
            }
        }
    }
}
