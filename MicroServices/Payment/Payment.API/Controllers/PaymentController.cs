using System.Security.Policy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.VisualBasic;
using Payment.API.Messaging;
using Payment.App.Service;
using Payment.Dom.Model;
using ZstdSharp.Unsafe;

namespace Payment.API.Controllers;

[Route("api/[controller]")]
[ApiController]
// [Authorize] // Require authentication for all endpoints
public class PaymentController: ControllerBase
{
    private readonly PaymentService _service;
    private readonly RabbitPaymentInitialPublisher _publisher;
    
    public PaymentController(PaymentService service, RabbitPaymentInitialPublisher  rabbitPublisher)
    {
        _service = service;
        _publisher = rabbitPublisher;
    }

    [HttpPost("insert")]
    public async Task<IActionResult> Insert([FromBody] Dom.Model.Payment t)
    {
        var res = await _service.Insert(t);
        if (res.IsSuccess)
        {
            var categoryIds = t.Items.Select(i => i.CategoryId);
            var categoryIncrements = t.Items.Select(i => i.Quantity);
            var paymentId = res.Value;
            _publisher.PublishPaymentAsync( new {  categoryIds,   categoryIncrements, paymentId  } );
            return CreatedAtAction(nameof(Get), new { id = res.Value }, new { id = res.Value });
        }
        return MapFailure(res.Errors);
    }
    
    [HttpPost("insert_with_category")]
    public async Task<IActionResult> InsertWithCategory([FromBody] Dom.Model.Payment t)
    {
        var res = await _service.Insert(t);
        if (res.IsSuccess)
        {
            // Payments, sagaStatus
            //            OK | PENDING | FAILED
            // Publisher.Publish()
            // UI.select_payments(  )
            return CreatedAtAction(nameof(Get), new { id = res.Value }, new { id = res.Value });
        }
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
    public async Task<IActionResult> Update([FromBody] Dom.Model.Payment t)
    {
        var res = await _service.Update(t);
        if (res.IsSuccess)
            return Ok(new { affected = res.Value });

        return MapFailure(res.Errors);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var paymentRes = await _service.SelectById(id);
        if (paymentRes.IsFailure)
            return MapFailure(paymentRes.Errors);

        var res = await _service.Delete(paymentRes.Value);
        if (res.IsSuccess)
            return Ok(new { affected = res.Value });

        return MapFailure(res.Errors);
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
    
    [HttpGet("search/{property}")]
    public async Task<ActionResult<List<Dom.Model.Payment>>> Search(string property)
    {
        var res = await _service.Search(property);
        if (res.IsSuccess)
        {
            return Ok(res.Value);
        }

        return StatusCode(500, new
        {
            message = "Error al buscar pagos",
            error = res.Errors
        });
    }
}
