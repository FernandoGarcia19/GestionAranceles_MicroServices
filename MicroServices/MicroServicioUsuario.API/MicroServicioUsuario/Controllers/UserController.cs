using MicroServicioUser.App.Services;
using MicroServicioUser.Dom.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MicroServicioUsuario.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService service;
        public UserController(UserService service)
        {
            this.service = service;
        }

        [HttpGet("select")]
        public async Task<ActionResult<List<User>>> Select()
        {
            var categories = await service.GetAll();
            if (categories.IsSuccess)
            {
                return Ok(categories.Value);
            }
            else
            {
                return StatusCode(500, new
                {
                    message = "Error al obtener los usuarios",
                    error = categories.Errors
                });
            }
        }

        // GET: api/User/search/comida

        [HttpGet("search/{property}")]
        public async Task<ActionResult<List<User>>> Search(string property)
        {
            var categories = await service.Search(property);
            if (categories.IsSuccess)
            {
                return Ok(categories.Value);
            }
            else
            {
                return StatusCode(500, new
                {
                    message = "Error al buscar usuario",
                    error = categories.Errors
                });
            }
        }

        // POST: api/User

        [HttpPost("create")]
        public async Task<ActionResult> Create([FromBody] User User)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await service.Insert(User);

            if (result.IsSuccess)
            {
                return CreatedAtAction(
                    nameof(Select),
                    new { id = result.Value },
                    new
                    {
                        id = result.Value,
                        message = "Usuario creado exitosamente",
                        data = User
                    }
                );
            }
            else
            {
                return StatusCode(500, new
                {
                    message = "Error al crear el usuario",
                    error = result.Errors
                });
            }
        }

        // PUT: api/User/5

        [HttpPut("update")]
        public async Task<ActionResult> Update([FromBody] User User)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await service.Update(User);

            if (result.IsSuccess)
            {
                return Ok(new
                {
                    message = "Usuario actualizado exitosamente",
                    data = User
                });
            }
            else
            {
                if (result.Errors.Any(e => e.Contains("No se encontró")))
                {
                    return NotFound(new
                    {
                        message = $"Usuario con ID {User.Id} no encontrada"
                    });
                }
                else
                {
                    return StatusCode(500, new
                    {
                        message = "Error al actualizar el usuario",
                        error = result.Errors
                    });
                }
            }
        }

        // DELETE: api/User/5

        [HttpDelete("delete/{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var result = await service.Delete(id);

            if (result.IsSuccess)
            {
                return Ok(new
                {
                    message = $"Usuario con ID {id} eliminada exitosamente"
                });
            }
            else
            {
                if (result.Errors.Any(e => e.Contains("No se encontró")))
                {
                    return NotFound(new
                    {
                        message = $"Usuario con ID {id} no encontrada"
                    });
                }
                else
                {
                    return StatusCode(500, new
                    {
                        message = "Error al eliminar el usario",
                        error = result.Errors
                    });
                }
            }
        }
    }
}
