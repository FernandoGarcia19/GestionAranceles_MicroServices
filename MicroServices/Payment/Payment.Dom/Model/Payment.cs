using System.ComponentModel.DataAnnotations;

namespace Payment.Dom.Model;

public class Payment
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El ID del establecimiento es obligatorio.")]
    [Range(1, int.MaxValue, ErrorMessage = "El ID del establecimiento debe ser mayor a 0.")]
    public int EstablishmentId { get; set; }

    [Required(ErrorMessage = "El ID de la categoría es obligatorio.")]
    [Range(1, int.MaxValue, ErrorMessage = "El ID de la categoría debe ser mayor a 0.")]
    public int CategoryId { get; set; }

    [Required(ErrorMessage = "La fecha de pago es obligatoria.")]
    public DateTime PaymentDate { get; set; }

    [Required(ErrorMessage = "El monto pagado es obligatorio.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "El monto pagado debe ser mayor a 0.")]
    public decimal AmountPaid { get; set; }

    [Required(ErrorMessage = "El método de pago es obligatorio.")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "El método de pago debe tener entre 3 y 50 caracteres.")]
    public string PaymentMethod { get; set; }

    [Required(ErrorMessage = "El número de recibo es obligatorio.")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "El número de recibo debe tener entre 3 y 100 caracteres.")]
    public string ReceiptNumber { get; set; }

    public DateTime CreatedDate { get; set; }
    public DateTime UpdateDate { get; set; }
    public bool Status { get; set; }
    public int CreatedBy { get; set; }
}
