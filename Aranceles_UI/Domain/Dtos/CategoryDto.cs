using System.ComponentModel.DataAnnotations;

namespace Aranceles_UI.Domain.Dtos;

public class CategoryDto
{
    public int Id { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public decimal BaseAmount { get; set; }
}