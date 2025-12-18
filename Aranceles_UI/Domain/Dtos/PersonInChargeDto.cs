using System.ComponentModel.DataAnnotations;

namespace Aranceles_UI.Domain.Dtos;

public class PersonInChargeDto
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 50 caracteres.")]
    [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$", ErrorMessage = "El nombre solo puede contener letras y espacios.")]
    public string FirstName { get; set; }

    [Required(ErrorMessage = "El apellido es obligatorio.")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "El apellido debe tener entre 3 y 50 caracteres.")]
    [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$", ErrorMessage = "El apellido solo puede contener letras y espacios.")]
    public string LastName { get; set; }

    [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
    [StringLength(100, ErrorMessage = "El correo no puede exceder 100 caracteres.")]
    [EmailAddress(ErrorMessage = "El correo electrónico es inválido.")]
    [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9-]+(\.[a-zA-Z0-9-]+)+$", ErrorMessage = "El correo electrónico debe contener un punto (.) en el dominio.")]
    public string Email { get; set; }

    [Required(ErrorMessage = "El teléfono es obligatorio.")]
    [StringLength(8, MinimumLength = 7, ErrorMessage = "El número de teléfono debe tener entre 7 y 8 dígitos.")]
    [RegularExpression(@"^\d{7,8}$", ErrorMessage = "El teléfono solo puede contener números.")]
    public string Phone { get; set; }

    [Required(ErrorMessage = "El CI es obligatorio.")]
    [StringLength(15, MinimumLength = 5, ErrorMessage = "El CI debe tener entre 5 y 15 caracteres.")]
    [RegularExpression(@"^[a-zA-Z0-9\-]+$", ErrorMessage = "El CI solo puede contener números, letras y guiones.")]
    public string Ci { get; set; }

    // public DateTime CreatedDate { get; set; }
    // public DateTime UpdateDate { get; set; }
    // public bool Status { get; set; }
    public int CreatedBy { get; set; }
}