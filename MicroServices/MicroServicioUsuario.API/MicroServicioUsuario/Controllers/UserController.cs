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

        [HttpPost("insert")]
        public async Task<IActionResult> Insert([FromBody] User t)
        {
            var res = await service.Insert(t);
            return Ok(res);
        }

        [HttpGet]
        public async Task<IActionResult> Select()
        {
            var res = await service.Select();
            return Ok(res);
        }
    }
}
