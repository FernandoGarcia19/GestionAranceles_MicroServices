namespace Aranceles_UI.Domain.Dtos;

public class EstablishmentDto
{
    public int Id { get; set; }

    public string Name { get; set; }

    public string TaxId { get; set; }

    public string SanitaryLicense { get; set; }

    public DateTime? SanitaryLicenseExpiry { get; set; }

    public string Address { get; set; }

    public string Phone { get; set; }

    public string Email { get; set; }

    public string EstablishmentType { get; set; }

    public int PersonInChargeId { get; set; }
}