using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using api_econsulta.Data;
using api_econsulta.DTOs;
using api_econsulta.Models;
using Microsoft.AspNetCore.Authorization;

namespace api_econsulta.Controllers;

[ApiController]
[Route("api/availabilities")]
public class AvailabilitiesController(EconsultaDbContext context) : ControllerBase
{
    private readonly EconsultaDbContext _context = context;

    // POST: /api/availabilities
    [HttpPost]
    public async Task<IActionResult> CreateAvailability(AddAvailabilityDto dto)
    {
        var doctor = await _context.Users.FindAsync(dto.DoctorId);
        if (doctor == null || doctor.Role != "medico")
        {
            return BadRequest("Médico não encontrado ou inválido.");
        }

        var availability = new Availability
        {
            DoctorId = dto.DoctorId,
            StartTime = dto.StartTime,
            EndTime = dto.EndTime
        };

        _context.Availabilities.Add(availability);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Disponibilidade cadastrada com sucesso." });
    }

    
    [Authorize(Policy = "DoctorPolicy")]
    [HttpGet("doctor/{doctorId}")]
    public async Task<IActionResult> GetByDoctor(Guid doctorId)
    {
        var availabilities = await _context.Availabilities
            .Where(a => a.DoctorId == doctorId)
            .OrderBy(a => a.StartTime)
            .ToListAsync();

        return Ok(availabilities);
    }

   
    [HttpGet("available")]
    public async Task<IActionResult> GetAvailableSlots()
    {
        var now = DateTime.UtcNow;

        
        var bookedSlots = await _context.Availabilities
            .Select(a => a.StartTime)
            .ToListAsync();

        var available = await _context.Availabilities
            .Where(a => a.StartTime > now && !bookedSlots.Contains(a.StartTime))
            .Include(a => a.Doctor)
            .OrderBy(a => a.StartTime)
            .ToListAsync();

        return Ok(available);
    }
}
