using System.ComponentModel.DataAnnotations;

namespace Zad10.Dtos;

public class CreatePrescriptionRequestDto
{
    
    [Required]
    public PatientDto patient { get; set; }

    [Required]
    public RequestPrescriptionDto prescriptionInfo { get; set; }
}