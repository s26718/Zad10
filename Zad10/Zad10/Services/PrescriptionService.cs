using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Zad10.Dtos;
using Zad10.Exceptions;
using Zad10.Helpers;
using Zad10.Models;
using Zad10.Repositories;

namespace Zad10.Services;

public class PrescriptionService : IPrescriptionService

{
    private readonly IPrescriptionRepository _prescriptionRepository;
    private readonly IConfiguration _configuration;
    public PrescriptionService(IPrescriptionRepository prescriptionRepository, IConfiguration configuration)
    {
        _configuration = configuration;
        _prescriptionRepository = prescriptionRepository;

    }
    
    public async Task HandleNewPrescriptionRequestAsync(CreatePrescriptionRequestDto createPrescriptionRequestDto)
    {
        var doctor =
            await _prescriptionRepository.GetDoctorByIdAsync(createPrescriptionRequestDto.prescriptionInfo.IdDoctor);
        if (doctor == null)
        {
            throw new NoSuchDoctorException("no doctor with such id: " + createPrescriptionRequestDto.prescriptionInfo
                .IdDoctor);
        }

        if (createPrescriptionRequestDto.prescriptionInfo.medicaments.Count > 10)
        {
            throw new TooManyMedicamentsException("10 medicaments max. amount on prescription exceeded");
        }

        if (createPrescriptionRequestDto.prescriptionInfo.DueDate <
            createPrescriptionRequestDto.prescriptionInfo.Date)
        {
            throw new DateMismatchException("date date must be earlier than date");
        }

        foreach (var prescriptionMedicament in createPrescriptionRequestDto.prescriptionInfo.medicaments)
        {
            if (!await _prescriptionRepository.MedicamentExistsAsync(prescriptionMedicament.IdMedicament))
            {
                throw new NoSuchMedicamentException("no medicament with id: " + prescriptionMedicament.IdMedicament);
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
            throw new NoSuchPatientException("no patient with such id: " + idPatient);
        }

        //tutaj include na doktora
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
            //pobrac doctora
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

        patientInfo.Prescriptions = patientInfo.Prescriptions.OrderBy(p => p.DueDate).ToList();
        return patientInfo;
    }

    public async Task<AppUser> RegisterUserAsync(RegisterRequest registerRequest)
    {
        var hashedPasswordAndSalt = SecurityHelpers.GetHashedPasswordAndSalt(registerRequest.Password);

        var newUser = await _prescriptionRepository.RegisterUserToDbAsync(registerRequest, hashedPasswordAndSalt);
        return newUser;

    }

    public async Task<Tuple<string, string>> ValidateLoginAsync(LoginRequest loginRequest)
    {
        var user = await _prescriptionRepository.GetUserByLoginAsync(loginRequest.Login);
        if (user == null)
        {
            throw new Exception("user with login: " + loginRequest.Login + "not found");
        }

        string passwordHashFromDb = user.Password;
        //weryfikacja
        string curHashedPassword = SecurityHelpers.GetHashedPasswordWithSalt(loginRequest.Password, user.Salt);
        if (passwordHashFromDb != curHashedPassword)
        {
            throw new Exception("unathorized");
        }

        Claim[] userclaim = new[]
        {
            new Claim(ClaimTypes.Name, user.Login),
            new Claim(ClaimTypes.Role, "user"),
        };
        SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["SecretKey"]));
        SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        JwtSecurityToken token = new JwtSecurityToken(
            issuer: "issuer",
            audience: "audience",
            claims: userclaim,
            expires: DateTime.Now.AddMinutes(10),
            signingCredentials: creds
        );
        user.RefreshToken = SecurityHelpers.GenerateRefreshToken();
        user.RefreshTokenExp = DateTime.Now.AddDays(1);
        await _prescriptionRepository.SaveChangesAsync();
        
        var accessToken = new JwtSecurityTokenHandler().WriteToken(token);
        var refreshToken = user.RefreshToken;
        
        return new Tuple<string, string>(accessToken, refreshToken);
    }

    public async Task<Tuple<string, string>> RefreshLoginAsync(RefreshTokenRequest refreshTokenRequest)
    {
        var user = await _prescriptionRepository.GetUserByRefreshTokenAsync(refreshTokenRequest.RefreshToken);
        if (user == null)
        {
            throw new SecurityTokenException("Invalid refresh token");
        }

        if (user.RefreshTokenExp < DateTime.Now)
        {
            throw new SecurityTokenException("Refresh token expired");
        }
        Claim[] userclaim = new[]
        {
            new Claim(ClaimTypes.Name, user.Login),
            new Claim(ClaimTypes.Role, "user"),
        };
        SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["SecretKey"]));
        SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        JwtSecurityToken token = new JwtSecurityToken(
            issuer: "issuer",
            audience: "audience",
            claims: userclaim,
            expires: DateTime.Now.AddMinutes(10),
            signingCredentials: creds
        );
        user.RefreshToken = SecurityHelpers.GenerateRefreshToken();
        user.RefreshTokenExp = DateTime.Now.AddDays(1);
        await _prescriptionRepository.SaveChangesAsync();
        
        var accessToken = new JwtSecurityTokenHandler().WriteToken(token);
        var refreshToken = user.RefreshToken;
        
        return new Tuple<string, string>(accessToken, refreshToken);
    }
}