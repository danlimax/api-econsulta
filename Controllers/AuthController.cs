using api_econsulta.DTOs;
using api_econsulta.Services;
using Microsoft.AspNetCore.Mvc;

namespace api_econsulta.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController(AuthService authService) : ControllerBase
    {
        private readonly AuthService _authService = authService;

        [HttpPost("register/doctor")]
        public async Task<IActionResult> RegisterDoctor(DoctorRegisterDto dto)
        {
            try
            {
                var doctor = await _authService.RegisterDoctorAsync(dto);
                return Ok(new { doctor.Id, doctor.Email, doctor.DoctorName, doctor.Specialty });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("register/patient")]
        public async Task<IActionResult> RegisterPatient(PatientRegisterDto dto)
        {
            try
            {
                var patient = await _authService.RegisterPatientAsync(dto);
                return Ok(new { patient.Id, patient.Email, patient.PatientName });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            try
            {
                var (role, token) = await _authService.ValidateDoctorOrPatient(dto.Email, dto.Password);
                return Ok(new { token, role });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { error = ex.Message });
            }
        }
    }
}
