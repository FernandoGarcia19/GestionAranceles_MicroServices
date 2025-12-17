using System.ComponentModel.DataAnnotations;

namespace Aranceles_UI.Domain.Dtos;

public class PaymentDto
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El ID del establecimiento es obligatorio.")]
    [Range(1, int.MaxValue, ErrorMessage = "El ID del establecimiento debe ser mayor a 0.")]
    public int EstablishmentId { get; set; }

    [Required(ErrorMessage = "La fecha de pago es obligatoria.")]
    public DateTime PaymentDate { get; set; } = DateTime.Now;

    [Required(ErrorMessage = "El monto pagado es obligatorio.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "El monto pagado debe ser mayor a 0.")]
    public decimal AmountPaid { get; set; }

    [Required(ErrorMessage = "El método de pago es obligatorio.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "El método de pago debe tener entre 2 y 50 caracteres.")]
    public string PaymentMethod { get; set; } = string.Empty;

    [Required(ErrorMessage = "El número de recibo es obligatorio.")]
    [Range(1, 16777215, ErrorMessage = "El número de recibo debe estar entre 1 y 16777215.")]
    public int ReceiptNumber { get; set; }

    // Payment items (categories with quantities and prices)
    public List<CategoryPaymentDto> Items { get; set; } = new List<CategoryPaymentDto>();
    
    // Navigation properties for display purposes
    public string? EstablishmentName { get; set; }
}