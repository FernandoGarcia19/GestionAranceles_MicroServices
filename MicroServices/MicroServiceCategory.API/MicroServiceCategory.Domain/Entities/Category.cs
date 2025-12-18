using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroServiceCategory.Domain.Entities
{
    public class Category
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "El nombre puede tener entre 3 a 50 caracteres.")]
        [RegularExpression(@"^(?!.* {2})[a-zA-ZÁÉÍÓÚáéíóúÑñ][a-zA-ZÁÉÍÓÚáéíóúÑñ\s\.,]*$",
        ErrorMessage = "Solo se permiten letras, espacios, puntos y comas.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "La descripción es obligatorio.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "El nombre puede tener entre 10 a 120 caracteres.")]
        [RegularExpression(@"^(?!.* {2})[a-zA-ZÁÉÍÓÚáéíóúÑñ][a-zA-ZÁÉÍÓÚáéíóúÑñ\s\.,]*$",
        ErrorMessage = "Solo se permiten letras, espacios, puntos y comas.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "El monto base es obligatorio.")]
        [Range(0, 100000, ErrorMessage = "El monto base debe ser mayor a 0.")]
        [RegularExpression(@"^[-+]?\d*.?\d*$")]
        public decimal BaseAmount { get; set; }
        public int CreatedBy { get; set; }
        public int NumberOfInserts { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastUpdate { get; set; }
        public byte Status { get; set; }
    }
}
