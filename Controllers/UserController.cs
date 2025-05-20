using api_econsulta.DTOs;
using api_econsulta.Models;
using api_econsulta.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using api_econsulta.Exceptions;

namespace api_econsulta.Controllers
{
    [ApiController]
    [Route("api/user")]
    public class UserController(UserService userService) : ControllerBase
    {
        private readonly UserService _userService = userService;

        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetById(int id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null) return NotFound();
            return Ok(user);
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<ActionResult<User>> GetMe()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim)) return Unauthorized();
            var userId = int.Parse(userIdClaim);
            var user = await _userService.GetByIdAsync(userId);
            if (user == null) return NotFound();
            return Ok(user);
        }
        
        [Authorize]
        [HttpGet("doctors")]
        public async Task<ActionResult<List<DoctorDto>>> GetDoctors()
        {
            var doctors = await _userService.GetAllDoctorsAsync();
            return Ok(doctors);
        }
        
        [Authorize]
        [HttpGet("patients")]
        public async Task<ActionResult<List<DoctorDto>>> GetPatients()
        {
            var patients = await _userService.GetAllPatientsAsync();
            return Ok(patients);
        }

        
        [HttpPost("patient")]
        public async Task<ActionResult<User>> CreatePatient(UserRegisterDto dto)
        {
            var user = new User
            {
                Email = dto.Email,
                Role = "paciente",
                Name = dto.Name,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            try
            {
                var created = await _userService.RegisterAsync(user, dto.Password);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (EmailAlreadyExistsException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("doctor")]
        public async Task<ActionResult<User>> CreateDoctor(UserRegisterDto dto)
        {
            var user = new User
            {
                Email = dto.Email,
                Role = "medico",
                Name = dto.Name,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            try
            {
                var created = await _userService.RegisterAsync(user, dto.Password);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (EmailAlreadyExistsException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
