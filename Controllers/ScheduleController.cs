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

        
        [Authorize]
        [HttpGet("doctor/apointments/{Doctorid}")]
        public async Task<ActionResult<List<Schedule>>> GetAllSchedulesByDoctorId(int Doctorid)
        {
            var schedules = await _scheduleService.GetByDoctorIdAsync(Doctorid);
            if (schedules == null || !schedules.Any())
                return NotFound("Nenhum horário encontrado para esse médico.");

            return Ok(schedules);
        }

        [Authorize]
        [HttpGet("patient/apointments/{PatientId}")]
        public async Task<ActionResult<List<Schedule>>> GetAllSchedulesByPatientId(int PatientId)
        {
            var schedules = await _scheduleService.GetByPatientIdAsync(PatientId);
            if (schedules == null || !schedules.Any())
                return NotFound("Nenhum horário encontrado para esse paciente.");

            return Ok(schedules);
        }

        [Authorize(Roles = "medico")]
        [HttpPost]
        public async Task<ActionResult<IEnumerable<Schedule>>> CreateSchedules([FromBody] List<ScheduleCreateDto> dtos)
        {
            if (dtos == null || !dtos.Any())
            {
                return BadRequest(new { message = "No schedules provided." });
            }

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

            var schedules = dtos.Select(dto => new Schedule
            {
                DoctorId = userId,  
                StartTime = dto.StartTime.ToUniversalTime(),
                EndTime = dto.EndTime.ToUniversalTime(),
                PatientId = null
            }).ToList();

            try
            {
                var createdSchedules = await _scheduleService.CreateSchedules(schedules);
                return Created($"/api/schedule", createdSchedules);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = "medico")]
        [HttpPut("{id}")]
        public async Task<ActionResult<Schedule>> UpdateAppointment(int id, ScheduleUpdateDto dto)
        {
            if (id <= 0)
            {
                return BadRequest(new { message = "Invalid schedule ID." });
            }

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
        
        [Authorize(Roles = "paciente")]
        [HttpPut("apointment/{id}")]
        public async Task<ActionResult<Schedule>> UpdatePatientSchedules(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { message = "Invalid schedule ID." });
            }

            var claims = this.User.Claims;
            var userIdClaim = claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return BadRequest(new { message = "User ID not found in claims." });
            }

            var patientId = int.Parse(userIdClaim.Value);
            if (patientId <= 0)
            {
                return BadRequest(new { message = "Invalid patient ID." });
            }

            try
            {
                var bookedAppointment = await _scheduleService.UpdatePatientSchedule(id, patientId);
                return Ok(bookedAppointment);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}