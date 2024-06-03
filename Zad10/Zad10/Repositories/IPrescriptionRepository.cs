using Zad10.Dtos;
using Zad10.Models;

namespace Zad10.Repositories;

public interface IPrescriptionRepository
{
    Task SaveChangesAsync();
    Task<Doctor?> GetDoctorByIdAsync(int id);
    Task<Patient?> GetPatientByIdAsync(int id);
    Task<Prescription?> GetPrescriptionByIdAsync(int id);
    Task InsertPrescription(CreatePrescriptionRequestDto request);
    Task<Patient?> InsertNewPatient(PatientDto patientDto);
    Task<bool> MedicamentExistsAsync(int idMedicament);
    Task<IEnumerable<Prescription>> GetPrescriptionsForPatientByIdAsync(int idPatient);
    Task<IEnumerable<Medicament>> GetMedicamentsForPrescriptionByIdAsync(int idPrescription);
    Task<List<PrescriptionMedicament>> GetPrescriptionMedicaments(int idPrescription, int idMedicament);
}