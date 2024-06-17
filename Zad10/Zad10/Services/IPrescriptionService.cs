using Zad10.Dtos;
using Zad10.Models;

namespace Zad10.Services;

public interface IPrescriptionService
{
    Task HandleNewPrescriptionRequestAsync(CreatePrescriptionRequestDto createPrescriptionRequestDto);
    Task<PatientInfoReturnDto> GetPatientInfoAsync(int idPatient);
    Task<AppUser> RegisterUserAsync(RegisterRequest registerRequest);
    Task<Tuple<string, string>> ValidateLoginAsync(LoginRequest loginRequest);
    Task<Tuple<string, string>> RefreshLoginAsync(RefreshTokenRequest refreshTokenRequest);
    
}