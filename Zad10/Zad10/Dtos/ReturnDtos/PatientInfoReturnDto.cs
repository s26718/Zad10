using System.ComponentModel.DataAnnotations;
using Zad10.Models;

namespace Zad10.Dtos;

public class PatientInfoReturnDto
{
    [Required]
    public int IdPatient { get; set; }
    [Required, MaxLength(100)]
    public string FirstName { get; set; }
    [Required, MaxLength(100)]
    public string LastName { get; set; }
    [Required]
    public DateTime BirthDate { get; set; }

    [Required] public List<ReturnPrescriptionDto> Prescriptions { get; set; } = new List<ReturnPrescriptionDto>();
}