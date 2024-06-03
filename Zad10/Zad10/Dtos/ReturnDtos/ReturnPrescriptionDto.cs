namespace Zad10.Dtos;

public class ReturnPrescriptionDto
{
    public int IdPrescription { get; set; }
    public DateTime Date { get; set; }
    public DateTime DueDate { get; set; }
    public List<PatientInfoMedicamentReturnDto> Medicaments { get; set; }
}