using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Zad10.Dtos;
using Zad10.Exceptions;
using Zad10.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Zad10.Helpers;
using Zad10.Models;

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

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> RegisterUser(RegisterRequest registerRequest)
    {
        await _prescriptionService.RegisterUserAsync(registerRequest);
        return Ok();
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest loginRequest)
    {
        
        var (accessToken, refreshToken) = await _prescriptionService.ValidateLoginAsync(loginRequest);
        
        return Ok(new
        {
            accessToken = accessToken,
            refreshToken = refreshToken
        });
        
        
    }

    [Authorize(AuthenticationSchemes = "IgnoreTokenExpirationScheme")]
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(RefreshTokenRequest refreshTokenRequest)
    {
        
        var (accessToken, refreshToken) = await _prescriptionService.RefreshLoginAsync(refreshTokenRequest);
        
        return Ok(new
        {
            accessToken = accessToken,
            refreshToken = refreshToken
        });
        
        
    }

    [Authorize]
    [HttpPost("new_prescription")]
    public async Task<IActionResult> CreateNewPrescriptionAsync(CreatePrescriptionRequestDto request)
    {
        await _prescriptionService.HandleNewPrescriptionRequestAsync(request);
        return Ok();
    }
    [Authorize]
    [HttpGet("patients/{patientId:int}")]
    public async Task<IActionResult> GetPatientInfoAsync(int patientId)
    {

        return Ok(await _prescriptionService.GetPatientInfoAsync(patientId));
    }
    

}