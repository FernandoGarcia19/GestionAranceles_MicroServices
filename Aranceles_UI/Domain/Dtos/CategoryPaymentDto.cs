using System.ComponentModel.DataAnnotations;

namespace Aranceles_UI.Domain.Dtos;

public class CategoryPaymentDto
{
    // PaymentId is auto-assigned by the microservice on insert, no validation needed
    public int PaymentId { get; set; }

    [Required(ErrorMessage = "El ID de la categoría es obligatorio.")]
    [Range(1, int.MaxValue, ErrorMessage = "El ID de la categoría debe ser mayor a 0.")]
    public int CategoryId { get; set; }

    [Required(ErrorMessage = "La cantidad es obligatoria.")]
    [Range(1, 255, ErrorMessage = "La cantidad debe estar entre 1 y 255.")]
    public byte Quantity { get; set; }

    [Required(ErrorMessage = "El precio unitario es obligatorio.")]
    [Range(1, 16777215, ErrorMessage = "El precio unitario debe estar entre 1 y 16777215.")]
    public int UnitPrice { get; set; }
    
    // Navigation properties for display purposes
    public string? CategoryName { get; set; }
}
