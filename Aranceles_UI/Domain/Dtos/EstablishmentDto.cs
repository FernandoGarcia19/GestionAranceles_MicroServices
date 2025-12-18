using System.ComponentModel.DataAnnotations;

namespace Aranceles_UI.Domain.Dtos;

public class EstablishmentDto
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 100 caracteres.")]
    [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ0-9\s.]+$", ErrorMessage = "El nombre solo puede contener letras, números, espacios y puntos.")]
    public string Name { get; set; }

    [Required(ErrorMessage = "El NIT es obligatorio.")]
    [StringLength(15, MinimumLength = 7, ErrorMessage = "El NIT debe tener entre 7 y 15 caracteres.")]
    [RegularExpression(@"^\d{7,10}$", ErrorMessage = "El NIT debe contener entre 7 y 10 dígitos numéricos.")]
    public string TaxId { get; set; }

    [Required(ErrorMessage = "La licencia sanitaria es obligatoria.")]
    [StringLength(50, MinimumLength = 5, ErrorMessage = "La licencia sanitaria debe tener entre 5 y 50 caracteres.")]
    [RegularExpression(@"^[a-zA-Z0-9\-]+$", ErrorMessage = "La licencia sanitaria solo puede contener letras, números y guiones.")]
    public string SanitaryLicense { get; set; }

    [Required(ErrorMessage = "La fecha de vencimiento de la licencia sanitaria es obligatoria.")]
    [DataType(DataType.Date)]
    public DateTime? SanitaryLicenseExpiry { get; set; }

    [Required(ErrorMessage = "La dirección es obligatoria.")]
    [StringLength(200, MinimumLength = 10, ErrorMessage = "La dirección debe tener entre 10 y 200 caracteres.")]
    [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ0-9\s,.\-#°]+$", ErrorMessage = "La dirección solo puede contener letras, números, espacios, comas, puntos, guiones, # y °.")]
    public string Address { get; set; }

    [Required(ErrorMessage = "El teléfono es obligatorio.")]
    [StringLength(8, MinimumLength = 7, ErrorMessage = "El número de teléfono puede tener entre 7 u 8 dígitos.")]
    [RegularExpression(@"^\d{7,8}$", ErrorMessage = "El teléfono solo puede contener números.")]
    public string Phone { get; set; }

    [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
    [StringLength(100, ErrorMessage = "El correo no puede exceder 100 caracteres.")]
    [EmailAddress(ErrorMessage = "Correo electrónico inválido.")]
    [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9-]+(\.[a-zA-Z0-9-]+)+$", ErrorMessage = "El correo electrónico debe contener un punto (.) en el dominio.")]
    public string Email { get; set; }

    [Required(ErrorMessage = "El tipo de establecimiento es obligatorio")]
    public string EstablishmentType { get; set; }
    [Required(ErrorMessage = "Debe seleccionar un encargado")]
    public int PersonInChargeId { get; set; }

    // public DateTime CreatedDate { get; set; }
    // public DateTime LastUpdate { get; set; }
    // public bool Status { get; set; }
    public int CreatedBy { get; set; }
}