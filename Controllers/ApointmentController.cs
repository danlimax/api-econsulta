using System.Security.Claims;
using api_econsulta.DTOs;
using api_econsulta.Models;
using api_econsulta.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api_econsulta.Controllers
{
    [ApiController]
    [Route("api/appointment")]
    public class AppointmentController(AppointmentService appointmentService) : ControllerBase
    {
        private readonly AppointmentService _appointmentService = appointmentService;

        [HttpGet("{id}")]
        public async Task<ActionResult<Schedule>> GetById(int id)
        {
            var appointment = await _appointmentService.GetByIdAsync(id);
            if (appointment == null) return NotFound();
            return Ok(appointment);
        }

        [Authorize(Roles = "paciente")]
        [HttpPost]
        public async Task<ActionResult<Schedule>> CreateAppointment(AppointmentCreateDto dto)
        {
            var claims = this.User.Claims;
            var userIdClaim = claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return BadRequest(new { message = "User ID not found in claims." });
            }

            var userId = int.Parse(userIdClaim.Value);
            if (userId <= 0)
            {
                return BadRequest(new { message = "Invalid user ID." });
            }

            try
            {
                var scheduleMax = await _appointmentService.CreateAppointment(dto.AppointmentId, userId);
                return Created($"/api/appointment/{scheduleMax.Id}", scheduleMax);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        
        
    }
}
