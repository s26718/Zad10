using System.Transactions;
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
            if (!await _prescriptionRepository.MedicamentExistsAsync(prescriptionMedicament.idMedicament))
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
    //TODO public async Task<PatientInfoReturnDto> GetPatientInfoAsync(int idPatient);
}