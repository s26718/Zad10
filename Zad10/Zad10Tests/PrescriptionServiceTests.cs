using Zad10.Dtos;
using Zad10.Exceptions;
using Zad10.Repositories;
using Zad10.Services;
using Zad10Tests.Fakes;

namespace Zad10Tests;

public class PrescriptionServiceTests
{
    private readonly IPrescriptionService _service;
    private readonly IPrescriptionRepository _repository = new FakePrescriptionRepository();

    public PrescriptionServiceTests()
    {
        _service = new PrescriptionService(_repository);
    }
    
    [Fact]
    public async Task HandleNewPrescriptionRequestAsync_ShouldThrowNoSuchDoctorException_WhenDoctorDoesNotExist()
    {
        // Arrange
        var request = new CreatePrescriptionRequestDto()
        {
            patient = new PatientDto { IdPatient = 1, FirstName = "John", LastName = "Doe", BirthDate = new DateTime(1980, 1, 1) },
            prescriptionInfo = new RequestPrescriptionDto()
            {
                IdDoctor = 999,
                Date = DateTime.Now,
                DueDate = DateTime.Now.AddDays(5),
                medicaments = new List<PrescriptionMedicamentDto> { new PrescriptionMedicamentDto { IdMedicament = 1, Dose = 2, Description = "Test" } }
            }
        };

        // Act & Assert
        await Assert.ThrowsAsync<NoSuchDoctorException>(() => _service.HandleNewPrescriptionRequestAsync(request));
    }
    
    [Fact]
    public async Task HandleNewPrescriptionRequestAsync_ShouldThrowTooManyMedicamentsException_WhenMoreThan10Medicaments()
    {
        // Arrange
        var medicaments = Enumerable.Range(1, 11).Select(i => new PrescriptionMedicamentDto { IdMedicament = i, Dose = i, Description = $"Test {i}" }).ToList();
        var request = new CreatePrescriptionRequestDto
        {
            patient = new PatientDto { IdPatient = 1, FirstName = "John", LastName = "Doe", BirthDate = new DateTime(1980, 1, 1) },
            prescriptionInfo = new RequestPrescriptionDto
            {
                IdDoctor = 1,
                Date = DateTime.Now,
                DueDate = DateTime.Now.AddDays(5),
                medicaments = medicaments
            }
        };

        // Act & Assert
        await Assert.ThrowsAsync<TooManyMedicamentsException>(() => _service.HandleNewPrescriptionRequestAsync(request));
    }
    [Fact]
    public async Task HandleNewPrescriptionRequestAsync_ShouldThrowDateMismatchException_WhenDueDateIsBeforeDate()
    {
        // Arrange
        var request = new CreatePrescriptionRequestDto
        {
            patient = new PatientDto { IdPatient = 1, FirstName = "John", LastName = "Doe", BirthDate = new DateTime(1980, 1, 1) },
            prescriptionInfo = new RequestPrescriptionDto()
            {
                IdDoctor = 1,
                Date = DateTime.Now,
                DueDate = DateTime.Now.AddDays(-1),
                medicaments = new List<PrescriptionMedicamentDto> { new PrescriptionMedicamentDto { IdMedicament = 1, Dose = 2, Description = "Test" } }
            }
        };

        // Act & Assert
        await Assert.ThrowsAsync<DateMismatchException>(() => _service.HandleNewPrescriptionRequestAsync(request));
    }
    
    [Fact]
    public async Task HandleNewPrescriptionRequestAsync_ShouldThrowNoSuchMedicamentException_WhenMedicamentDoesNotExist()
    {
        // Arrange
        var request = new CreatePrescriptionRequestDto
        {
            patient = new PatientDto { IdPatient = 1, FirstName = "John", LastName = "Doe", BirthDate = new DateTime(1980, 1, 1) },
            prescriptionInfo = new RequestPrescriptionDto()
            {
                IdDoctor = 1,
                Date = DateTime.Now,
                DueDate = DateTime.Now.AddDays(5),
                medicaments = new List<PrescriptionMedicamentDto> { new PrescriptionMedicamentDto { IdMedicament = 999, Dose = 2, Description = "Test" } }
            }
        };

        // Act & Assert
        await Assert.ThrowsAsync<NoSuchMedicamentException>(() => _service.HandleNewPrescriptionRequestAsync(request));
    }
    
    [Fact]
    public async Task HandleNewPrescriptionRequestAsync_ShouldAddNewPatient_WhenPatientDoesNotExist()
    {
        // Arrange
        var request = new CreatePrescriptionRequestDto
        {
            patient = new PatientDto { IdPatient = 999, FirstName = "New", LastName = "Patient", BirthDate = new DateTime(2000, 1, 1) },
            prescriptionInfo = new RequestPrescriptionDto()
            {
                IdDoctor = 1,
                Date = DateTime.Now,
                DueDate = DateTime.Now.AddDays(5),
                medicaments = new List<PrescriptionMedicamentDto> { new PrescriptionMedicamentDto { IdMedicament = 1, Dose = 2, Description = "Test" } }
            }
        };

        // Act
        await _service.HandleNewPrescriptionRequestAsync(request);

        // Assert
        var newPatient = await _repository.GetPatientByIdAsync(request.patient.IdPatient);
        Assert.NotNull(newPatient);
    }
    
    [Fact]
    public async Task GetPatientInfoAsync_ShouldReturnPatientInfo_WhenPatientExists()
    {
        // Arrange
        var patientId = 1;

        // Act
        var patientInfo = await _service.GetPatientInfoAsync(patientId);

        // Assert
        Assert.NotNull(patientInfo);
        Assert.Equal(patientId, patientInfo.IdPatient);
        Assert.NotEmpty(patientInfo.Prescriptions);
    }
    [Fact]
    public async Task GetPatientInfoAsync_ShouldThrowNoSuchPatientException_WhenPatientDoesNotExist()
    {
        // Arrange
        var patientId = 999999;

        // Act & Assert
        await Assert.ThrowsAsync<NoSuchPatientException>(() => _service.GetPatientInfoAsync(patientId));
    }
    
    [Fact]
    public async Task HandleNewPrescriptionRequestAsync_ShouldInsertPrescription_WhenAllDataIsValid()
    {
        // Arrange
        var request = new CreatePrescriptionRequestDto
        {
            patient = new PatientDto { IdPatient = 1, FirstName = "John", LastName = "Doe", BirthDate = new DateTime(1980, 1, 1) },
            prescriptionInfo = new RequestPrescriptionDto
            {
                IdDoctor = 1,
                Date = DateTime.Now,
                DueDate = DateTime.Now.AddDays(5),
                medicaments = new List<PrescriptionMedicamentDto> { new PrescriptionMedicamentDto { IdMedicament = 1, Dose = 2, Description = "Test" } }
            }
        };

        // Act
        await _service.HandleNewPrescriptionRequestAsync(request);

        // Assert
        var prescriptions = await _repository.GetPrescriptionsForPatientByIdAsync(request.patient.IdPatient);
        Assert.Contains(prescriptions, p => p.Date == request.prescriptionInfo.Date && p.DueDate == request.prescriptionInfo.DueDate);
    }

}