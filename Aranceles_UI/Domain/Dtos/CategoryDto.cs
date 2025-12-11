using System.ComponentModel.DataAnnotations;

namespace Aranceles_UI.Domain.Dtos;

public class CategoryDto
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(50, MinimumLength = 3,ErrorMessage = "El nombre puede tener entre 3 a 50 caracteres.")]
    [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$", ErrorMessage = "El nombre solo puede contener letras y espacios.")]
    public string Name { get; set; }

    [Required(ErrorMessage = "La descripción es obligatoria")]
    [StringLength(200, MinimumLength = 3, ErrorMessage = "La descripción debe tener entre 3 y 200 caracteres.")]
    [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ0-9\s.,]+$", ErrorMessage = "La descripción solo puede contener letras, números, espacios, puntos y comas.")]
    public string Description { get; set; }

    [Required(ErrorMessage = "El monto base es obligatorio.")]
    [Range(0, 100000, ErrorMessage = "El monto base debe ser positivo y menor a 100000.")]
    public decimal? BaseAmount { get; set; }
}