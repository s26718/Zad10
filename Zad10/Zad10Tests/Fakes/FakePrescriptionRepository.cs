using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Zad10.Dtos;
using Zad10.Models;
using Zad10.Repositories;

namespace Zad10Tests.Fakes
{
    public class FakePrescriptionRepository : IPrescriptionRepository
    {
        private readonly List<Doctor> _doctors;
        private readonly List<Patient> _patients;
        private readonly List<Prescription> _prescriptions;
        private readonly List<Medicament> _medicaments;
        private readonly List<PrescriptionMedicament> _prescriptionMedicaments;

        public FakePrescriptionRepository()
        {
            _doctors = new List<Doctor>
            {
                new Doctor { Id = 1, FirstName = "Doctor", LastName = "Who", Email = "doctor1@example.com" },
                new Doctor { Id = 2, FirstName = "Gregory", LastName = "House", Email = "doctor2@example.com" }
            };

            _patients = new List<Patient>
            {
                new Patient { Id = 1, FirstName = "John", LastName = "Doe", Birthdate = new DateTime(1980, 1, 1) },
                new Patient { Id = 2, FirstName = "Jane", LastName = "Doe", Birthdate = new DateTime(1990, 5, 15) }
            };

            _medicaments = new List<Medicament>
            {
                new Medicament { Id = 1, Name = "Medicament A", Description = "Sample medicament A description" },
                new Medicament { Id = 2, Name = "Medicament B", Description = "Sample medicament B description" },
                new Medicament { Id = 3, Name = "Medicament C", Description = "Sample medicament C description" }
            };

            _prescriptions = new List<Prescription>
            {
                new Prescription
                {
                    Id = 1,
                    Date = new DateTime(2023, 1, 1),
                    DueDate = new DateTime(2023, 1, 10),
                    IdPatient = 1,
                    IdDoctor = 1,
                    PrescriptionMedicaments = new List<PrescriptionMedicament>
                    {
                        new PrescriptionMedicament { IdPrescription = 1, IdMedicament = 1, Dose = 2, Details = "Take twice daily" },
                        new PrescriptionMedicament { IdPrescription = 1, IdMedicament = 2, Dose = 1, Details = "Take once daily" }
                    }
                },
                new Prescription
                {
                    Id = 2,
                    Date = new DateTime(2023, 2, 1),
                    DueDate = new DateTime(2023, 2, 10),
                    IdPatient = 2,
                    IdDoctor = 2,
                    PrescriptionMedicaments = new List<PrescriptionMedicament>
                    {
                        new PrescriptionMedicament { IdPrescription = 2, IdMedicament = 3, Dose = 3, Details = "Take three times daily" }
                    }
                }
            };

            _prescriptionMedicaments = _prescriptions.SelectMany(p => p.PrescriptionMedicaments).ToList();
            foreach (var prescription in _prescriptions)
            {
                prescription.Patient = _patients.FirstOrDefault(p => p.Id == prescription.IdPatient);
                prescription.Doctor = _doctors.FirstOrDefault(d => d.Id == prescription.IdDoctor);
                foreach (var pm in prescription.PrescriptionMedicaments)
                {
                    pm.Prescription = prescription;
                    pm.Medicament = _medicaments.FirstOrDefault(m => m.Id == pm.IdMedicament);
                }
            }
        }

        public Task<Doctor?> GetDoctorByIdAsync(int id)
        {
            var doctor = _doctors.FirstOrDefault(d => d.Id == id);
            return Task.FromResult(doctor);
        }

        public Task<Patient?> GetPatientByIdAsync(int id)
        {
            var patient = _patients.FirstOrDefault(p => p.Id == id);
            return Task.FromResult(patient);
        }

        public Task<Prescription?> GetPrescriptionByIdAsync(int id)
        {
            var prescription = _prescriptions.FirstOrDefault(p => p.Id == id);
            return Task.FromResult(prescription);
        }

        public Task<Patient?> InsertNewPatient(PatientDto patientDto)
        {
            var newPatient = new Patient
            {
                Id = _patients.Any() ? _patients.Max(p => p.Id) + 1 : 1,
                FirstName = patientDto.FirstName,
                LastName = patientDto.LastName,
                Birthdate = patientDto.BirthDate
            };
            _patients.Add(newPatient);
            return Task.FromResult((Patient?)newPatient);
        }

        public Task InsertPrescription(CreatePrescriptionRequestDto request)
        {
            var newPrescription = new Prescription
            {
                Id = _prescriptions.Any() ? _prescriptions.Max(p => p.Id) + 1 : 1,
                Date = request.prescriptionInfo.Date,
                DueDate = request.prescriptionInfo.DueDate,
                IdPatient = request.patient.IdPatient,
                IdDoctor = request.prescriptionInfo.IdDoctor,
                PrescriptionMedicaments = request.prescriptionInfo.medicaments
                    .Select(prescriptionMedicament => new PrescriptionMedicament
                    {
                        IdPrescription = _prescriptions.Any() ? _prescriptions.Max(p => p.Id) + 1 : 1,
                        IdMedicament = prescriptionMedicament.IdMedicament,
                        Dose = prescriptionMedicament.Dose,
                        Details = prescriptionMedicament.Description
                    }).ToList()
            };

            _prescriptions.Add(newPrescription);
            _prescriptionMedicaments.AddRange(newPrescription.PrescriptionMedicaments);
            return Task.CompletedTask;
        }

        public Task<bool> MedicamentExistsAsync(int idMedicament)
        {
            var medicamentExists = _medicaments.Any(m => m.Id == idMedicament);
            return Task.FromResult(medicamentExists);
        }

        public Task<IEnumerable<Prescription>> GetPrescriptionsForPatientByIdAsync(int idPatient)
        {
            var prescriptions = _prescriptions.Where(p => p.IdPatient == idPatient);
            return Task.FromResult(prescriptions);
        }

        public Task<IEnumerable<Medicament>> GetMedicamentsForPrescriptionByIdAsync(int idPrescription)
        {
            var medicaments = _prescriptionMedicaments
                .Where(pm => pm.IdPrescription == idPrescription)
                .Select(pm => pm.Medicament);
            return Task.FromResult(medicaments);
        }

        public Task<List<PrescriptionMedicament>> GetPrescriptionMedicaments(int idPrescription, int idMedicament)
        {
            var prescriptionMedicaments = _prescriptionMedicaments
                .Where(pm => pm.IdPrescription == idPrescription && pm.IdMedicament == idMedicament)
                .ToList();
            return Task.FromResult(prescriptionMedicaments);
        }

        public Task<AppUser> RegisterUserToDbAsync(RegisterRequest registerRequest, Tuple<string, string> hashedPasswordAndSalt)
        {
            throw new NotImplementedException();
        }

        public Task SaveChangesAsync()
        {
            return Task.CompletedTask;
        }
    }
}
