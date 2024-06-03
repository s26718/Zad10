using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using Zad10.Dtos;
using Zad10.Exceptions;
using Zad10.Services;
using Microsoft.EntityFrameworkCore;

namespace Zad10;

[Route("api/prescriptions")]
[ApiController]

public class PrescriptionController : ControllerBase
{
    private IPrescriptionService _prescriptionService;

    public PrescriptionController(IPrescriptionService prescriptionService)
    {
        _prescriptionService = prescriptionService;
    }

    [HttpPost("new_prescription")]
    public async Task<IActionResult> CreateNewPrescriptionAsync(CreatePrescriptionRequestDto request)
    {
        try
        {
            await _prescriptionService.HandleNewPrescriptionRequestAsync(request);
        }
        catch (NoSuchDoctorException exc)
        {
            return StatusCode(StatusCodes.Status400BadRequest, "no doctor with such id exists");
        }
        catch (TooManyMedicamentsException exc)
        {
            return StatusCode(StatusCodes.Status400BadRequest, "max 10 of medicaments on one prescription");
        }
        catch (NoSuchMedicamentException exc)
        {
            return StatusCode(StatusCodes.Status400BadRequest, "one of the medicaments does not exits");
        }
        catch (DateMismatchException exc)
        {
            return StatusCode(StatusCodes.Status400BadRequest, "date mismatch error");
        }

        return Ok();
    }

}