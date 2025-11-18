using Establishment.App.Service;
using Microsoft.AspNetCore.Mvc;

namespace Establishment.API.Controller;
[Route("api/[controller]")]
[ApiController]
public class EstablishmentController: ControllerBase
{
    private readonly EstablishmentService _service;

    public EstablishmentController(EstablishmentService service)
    {
        _service = service;
    }

    [HttpPost("insert")]
    public async Task<IActionResult> Insert([FromBody] Dom.Model.Establishment t)
    {
        var res = await _service.Insert(t);
        return Ok(res);
    }
    
    [HttpGet]
    public async Task<IActionResult> Select()
    {
        var res = await _service.Select();
        return Ok(res);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var res = await _service.SelectById(id);
        return Ok(res);
    }
    
    [HttpPut]
    public async Task<IActionResult> Update([FromBody] Dom.Model.Establishment t)
    {
        var res = await _service.Update(t);
        return Ok(res);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        Dom.Model.Establishment person = await _service.SelectById(id);
        var res = await _service.Delete(person);
        return Ok(res);
    }
}