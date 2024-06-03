using System.ComponentModel.DataAnnotations;

namespace Zad10.Dtos;

public class RequestPrescriptionDto
{
    [Required]
    public int IdDoctor { get; set; }
    
    [Required]
    public List<PrescriptionMedicamentDto> medicaments { get; set; }
    public DateTime Date { get; set; }
    [Required]
    public DateTime DueDate { get; set; }
    
}