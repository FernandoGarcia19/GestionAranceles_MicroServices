using Establishment.App.Service;
using Microsoft.AspNetCore.Mvc;
using Establishment.Dom.Model;

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
        if (res.IsSuccess)
            return CreatedAtAction(nameof(Get), new { id = res.Value }, new { id = res.Value });

        return MapFailure(res.Errors);
    }
    
    [HttpGet]
    public async Task<IActionResult> Select()
    {
        var res = await _service.Select();
        if (res.IsSuccess)
            return Ok(res.Value);

        return MapFailure(res.Errors);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var res = await _service.SelectById(id);
        if (res.IsSuccess)
            return Ok(res.Value);

        return MapFailure(res.Errors);
    }
    
    [HttpPut]
    public async Task<IActionResult> Update([FromBody] Dom.Model.Establishment t)
    {
        var res = await _service.Update(t);
        if (res.IsSuccess)
            return Ok(new { affected = res.Value });

        return MapFailure(res.Errors);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var selectRes = await _service.SelectById(id);
        if (!selectRes.IsSuccess)
            return MapFailure(selectRes.Errors);

        var res = await _service.Delete(selectRes.Value);
        if (res.IsSuccess)
            return Ok(new { affected = res.Value });

        return MapFailure(res.Errors);
    }

    [HttpGet("search/{property}")]
    public async Task<ActionResult<List<Dom.Model.Establishment>>> Search(string property)
    {
        var res = await _service.Search(property);
        if (res.IsSuccess)
        {
            return Ok(res.Value);
        }

        return StatusCode(500, new
        {
            message = "Error al buscar establecimientos",
            error = res.Errors
        });
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
}