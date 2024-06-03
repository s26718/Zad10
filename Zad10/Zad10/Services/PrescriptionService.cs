using Zad10.Dtos;
using Zad10.Exceptions;
using Zad10.Models;
using Zad10.Repositories;

namespace Zad10.Services;

public class PrescriptionService : IPrescriptionService

{
    private readonly IPrescriptionRepository _prescriptionRepository;
    public PrescriptionService(IPrescriptionRepository prescriptionRepository)
    {
        _prescriptionRepository = prescriptionRepository;

    }
    
    public async Task HandleNewPrescriptionRequestAsync(CreatePrescriptionRequestDto createPrescriptionRequestDto)
    {
        var doctor =
            await _prescriptionRepository.GetDoctorByIdAsync(createPrescriptionRequestDto.prescriptionInfo.IdDoctor);
        if (doctor == null)
        {
            throw new NoSuchDoctorException();
        }

        if (createPrescriptionRequestDto.prescriptionInfo.medicaments.Count > 10)
        {
            throw new TooManyMedicamentsException();
        }

        if (createPrescriptionRequestDto.prescriptionInfo.DueDate <
            createPrescriptionRequestDto.prescriptionInfo.Date)
        {
            throw new DateMismatchException();
        }

        foreach (var prescriptionMedicament in createPrescriptionRequestDto.prescriptionInfo.medicaments)
        {
            if (!await _prescriptionRepository.MedicamentExistsAsync(prescriptionMedicament.IdMedicament))
            {
                throw new NoSuchMedicamentException();
            }
        }
        var patient =
            await _prescriptionRepository.GetPatientByIdAsync(createPrescriptionRequestDto.patient.IdPatient);
        if (patient == null)
        {
            patient = await _prescriptionRepository.InsertNewPatient(createPrescriptionRequestDto.patient);
            createPrescriptionRequestDto.patient.IdPatient = patient.Id;
        }
        await _prescriptionRepository.InsertPrescription(createPrescriptionRequestDto);
        await _prescriptionRepository.SaveChangesAsync();

    }

    public async Task<PatientInfoReturnDto> GetPatientInfoAsync(int idPatient)
    {
        
        var patient = await _prescriptionRepository.GetPatientByIdAsync(idPatient);
        if (patient == null)
        {
            throw new NoSuchPatientException();
        }

        var prescriptions = await _prescriptionRepository.GetPrescriptionsForPatientByIdAsync(idPatient);
        

        PatientInfoReturnDto patientInfo = new PatientInfoReturnDto
        {
            IdPatient = patient.Id,
            FirstName = patient.FirstName,
            LastName = patient.LastName,
            BirthDate = patient.Birthdate,
        };
        foreach (var prescription in prescriptions)
        {
            var doctor = await _prescriptionRepository.GetDoctorByIdAsync(prescription.IdDoctor);
            
            var medicaments = await _prescriptionRepository.GetMedicamentsForPrescriptionByIdAsync(prescription.Id);

            foreach (var medicament in medicaments)
            {
                var prescriptionMedicaments =
                    await _prescriptionRepository.GetPrescriptionMedicaments(prescription.Id, medicament.Id);
                patientInfo.Prescriptions.Add(
                    new ReturnPrescriptionDto
                    {
                        Doctor = new DoctorPatientInfoReturnDto
                        {
                            IdDoctor = doctor.Id,
                            FirstName = doctor.FirstName
                        },
                        IdPrescription = prescription.Id,
                        Date = prescription.Date,
                        DueDate = prescription.DueDate,
                        Medicaments = prescriptionMedicaments
                            .Select(pm => new PatientInfoMedicamentReturnDto
                            {
                                IdMedicament = pm.IdMedicament,
                                Name = medicament.Name,
                                Dose = pm.Dose,
                                Description = pm.Details
                            })
                            .ToList()
                        
                    
                    }
                );
            }
            
        }

        return patientInfo;
    }
}