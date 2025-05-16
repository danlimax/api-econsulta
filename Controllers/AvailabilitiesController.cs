using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using api_econsulta.Services;
using api_econsulta.DTOs;
using System.Security.Claims;

namespace api_econsulta.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AvailabilitiesController : ControllerBase
    {
        private readonly AvailabilityService _availabilityService;

        public AvailabilitiesController(AvailabilityService availabilityService)
        {
            _availabilityService = availabilityService;
        }

        // POST api/availabilities
        [HttpPost]
        [Authorize(Policy = "DoctorOrAdminPolicy")]
        public async Task<IActionResult> CreateAvailability([FromBody] CreateAvailabilityDto model)
        {
            try
            {
                // Recupera o ID do médico a partir do token JWT
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var role = User.FindFirstValue(ClaimTypes.Role);

                if (role != "medico")
                    return Forbid("Apenas médicos podem criar disponibilidades.");

                if (!Guid.TryParse(userId, out var doctorId))
                    return Unauthorized("ID do médico inválido no token.");

                model.DoctorId = doctorId;

                var availability = await _availabilityService.CreateAvailabilityAsync(model);
                
                return Ok(new
                {
                    Message = "Disponibilidade cadastrada com sucesso.",
                    Availability = availability
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { Message = "Erro interno ao processar a solicitação" });
            }
        }

        // GET api/availabilities/doctor/{doctorId}
        [HttpGet("doctor/{doctorId}")]
        [Authorize] // Opcionalmente: [Authorize(Policy = "DoctorOrAdminPolicy")]
        public async Task<IActionResult> GetDoctorAvailabilities(Guid doctorId)
        {
            try
            {
                var availabilities = await _availabilityService.GetDoctorAvailabilitiesAsync(doctorId);
                return Ok(availabilities);
            }
            catch (Exception)
            {
                return StatusCode(500, new { Message = "Erro interno ao processar a solicitação" });
            }
        }

        // GET api/availabilities/available
        [HttpGet("available")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAvailableAvailabilities()
        {
            try
            {
                var availabilities = await _availabilityService.GetAvailableAvailabilitiesAsync();
                return Ok(availabilities);
            }
            catch (Exception)
            {
                return StatusCode(500, new { Message = "Erro interno ao processar a solicitação" });
            }
        }
    }
}
