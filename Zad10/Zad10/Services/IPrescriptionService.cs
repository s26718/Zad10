using Zad10.Dtos;

namespace Zad10.Services;

public interface IPrescriptionService
{
    Task HandleNewPrescriptionRequestAsync(CreatePrescriptionRequestDto createPrescriptionRequestDto);
    Task<PatientInfoReturnDto> GetPatientInfoAsync(int idPatient);
}