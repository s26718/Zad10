using System.ComponentModel.DataAnnotations;

namespace Zad10.Dtos;

public class PatientInfoMedicamentReturnDto
{
    [Required]
    public int IdMedicament { get; set; }
    [Required, MaxLength(100)]
    public string Name { get; set; }
    [Required]
    public int Dose { get; set; }
    [Required, MaxLength(100)]
    public string? Description { get; set; }
}