using System.Security.Claims;
using api_econsulta.DTOs;
using api_econsulta.Models;
using api_econsulta.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api_econsulta.Controllers
{
    [ApiController]
    [Route("api/schedule")]
    public class ScheduleController(ScheduleService scheduleService) : ControllerBase
    {
        private readonly ScheduleService _scheduleService = scheduleService;

        [HttpGet("{id}")]
        public async Task<ActionResult<Schedule>> GetById(int id)
        {
            var schedule = await _scheduleService.GetByIdAsync(id);
            if (schedule == null) return NotFound();
            return Ok(schedule);
        }

        [Authorize(Roles = "paciente")]
        [HttpGet("/doctor/{doctorId}")]
        public async Task<ActionResult<IEnumerable<Schedule>>> GetAll()
        {
            var doctorId = HttpContext.Request.RouteValues["doctorId"];
            var doctorIdString = doctorId?.ToString();
            if (doctorIdString == null)
            {
                return BadRequest(new { message = "Doctor ID is required." });
            }

            var doctorIdInt = int.Parse(doctorIdString);
            if (doctorIdInt <= 0)
            {
                return BadRequest(new { message = "Invalid doctor ID." });
            }

            var availableSpots = await _scheduleService.GetAllAvailableSpotsByDoctorId(doctorIdInt);
            if (availableSpots == null || !availableSpots.Any())
            {
                return NotFound(new { message = "No available spots found." });
            }
            return Ok(availableSpots);
        }

        [Authorize(Roles = "medico")]
        [HttpGet("/doctor/appointments")]
        public async Task<ActionResult<IEnumerable<Schedule>>> GetAllBookedAppointmentsByDoctor()
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

            var bookedAppointments = await _scheduleService.GetAllBookedAppointmentsByDoctorId(userId);
            if (bookedAppointments == null || !bookedAppointments.Any())
            {
                return NotFound(new { message = "No booked appointments found." });
            }
            return Ok(bookedAppointments);
        }

        [Authorize(Roles = "paciente")]
        [HttpGet("/patient/appointments")]
        public async Task<ActionResult<IEnumerable<Schedule>>> GetAllBookedAppointmentsByPatient()
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

            var bookedAppointments = await _scheduleService.GetAllBookedAppointmentsByPatientId(userId);
            if (bookedAppointments == null || !bookedAppointments.Any())
            {
                return NotFound(new { message = "No booked appointments found." });
            }
            return Ok(bookedAppointments);
        }

        [Authorize(Roles = "medico")]
        [HttpPost("/schedule")]
        public async Task<ActionResult<Schedule>> CreateSchedule(ScheduleCreateDto dto)
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

            var schedule = new Schedule
            {
                DoctorId = userId,
                StartTime = dto.StartTime.ToUniversalTime(),
                EndTime = dto.EndTime.ToUniversalTime(),
            };

            try
            {
                var scheduleMax = await _scheduleService.CreateSchedule(schedule);
                return Created($"/api/schedule/{scheduleMax.Id}", scheduleMax);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        
        [Authorize(Roles = "doctor")]
        public async Task<ActionResult<Schedule>> UpdateAppointment(int id, ScheduleUpdateDto dto)
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
                var updatedAppointment = await _scheduleService.UpdateAppointment(id, dto);
                return Ok(updatedAppointment);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
