using System.ComponentModel.DataAnnotations;

namespace Zad10.Dtos;

public class PatientDto
{
    [Required]
    public int IdPatient { get; set; }
    [Required, MaxLength(100)]
    public string FirstName { get; set; }
    [Required, MaxLength(100)]
    public string LastName { get; set; }
    [Required]
    public DateTime BirthDate { get; set; }
}