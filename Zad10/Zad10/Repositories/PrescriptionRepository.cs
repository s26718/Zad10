using System.Data.Entity.Core.Mapping;
using Microsoft.EntityFrameworkCore;
using Zad10.Dtos;
using Zad10.Models;

namespace Zad10.Repositories;

public class PrescriptionRepository : IPrescriptionRepository
{
    
    private readonly PrescriptionContext _context;
    public PrescriptionRepository(PrescriptionContext context)
    {
        _context = context;
    }

    public async Task<Patient?> GetPatientByIdAsync(int id)
    {
        var patient = await _context.Patients.FirstOrDefaultAsync(p => p.Id == id);
        return patient;
    }

    public async Task<Doctor?> GetDoctorByIdAsync(int id)
    {
        var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.Id == id);
        return doctor;
    }
    public async Task<Prescription?> GetPrescriptionByIdAsync(int id)
    {
        var prescription = await _context.Prescriptions.FirstOrDefaultAsync(p => p.Id == id);
        return prescription;
    }

    public async Task<Patient?> InsertNewPatient(PatientDto patientDto)
    {
        var newPatient = new Patient
        {
            FirstName = patientDto.FirstName,
            LastName = patientDto.LastName,
            Birthdate = patientDto.BirthDate
        };
        _context.Patients.Add(newPatient);
        await _context.SaveChangesAsync();
        return await _context.Patients.FirstOrDefaultAsync(p => p.Id == newPatient.Id);
    }

    public async Task InsertPrescription(CreatePrescriptionRequestDto request)
    {
        Prescription prescription = new Prescription
        {
            Date = request.prescriptionInfo.Date,
            DueDate = request.prescriptionInfo.DueDate,
            IdPatient = request.patient.IdPatient,
            IdDoctor = request.prescriptionInfo.IdDoctor,
            PrescriptionMedicaments = request.prescriptionInfo.medicaments
                .Select(prescriptionMedicament => new PrescriptionMedicament
                {
                    IdMedicament = prescriptionMedicament.IdMedicament,
                    Dose = prescriptionMedicament.Dose,
                    Details = prescriptionMedicament.Description
                }).ToList()
        };
        await _context.Prescriptions.AddAsync(prescription);
    }

    public async Task<bool> MedicamentExistsAsync(int idMedicament)
    {
        var medicament = await _context.Medicaments.FirstOrDefaultAsync(medicament => medicament.Id == idMedicament);

        return medicament != null;
    }

    public async Task<IEnumerable<Prescription>> GetPrescriptionsForPatientByIdAsync(int idPatient)
    {
        var prescriptions = await _context.Prescriptions
            .Select(p => p)
            .Where(p => p.IdPatient == idPatient)
            .ToListAsync();
        return prescriptions;
    }

    public async Task<IEnumerable<Medicament>> GetMedicamentsForPrescriptionByIdAsync(int idPrescription)
    {
        var medicaments = await _context.PrescriptionMedicaments
            .Where(pm => pm.IdPrescription == idPrescription)
            .Select(pm => pm.Medicament)
            .ToListAsync();
        return medicaments;
    }

    public async Task<List<PrescriptionMedicament>> GetPrescriptionMedicaments(int idPrescription,
        int idMedicament)
    {
        return await _context.PrescriptionMedicaments
            .Where(pm => pm.IdPrescription == idPrescription && pm.IdMedicament == idMedicament)
            .ToListAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}