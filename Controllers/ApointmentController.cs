using Microsoft.AspNetCore.Mvc;
using api_econsulta.DTOs;
using api_econsulta.Services;
using Microsoft.AspNetCore.Authorization;

namespace api_econsulta.Controllers;

[ApiController]
[Route("api/appointments")]
public class AppointmentController(AppointmentService appointmentService) : ControllerBase
{
    private readonly AppointmentService _appointmentService = appointmentService;

    [HttpPost]
    public async Task<IActionResult> Create(CreateAppointmentDto dto)
    {
        var appointment = await _appointmentService.Create(dto);
        if (appointment == null) return BadRequest("Horário já agendado ou dados inválidos.");
        return Ok(appointment);
    }
    
    [Authorize(Policy = "DoctorPolicy")]
    [HttpGet("doctor/{doctorId}")]
    public async Task<IActionResult> GetByDoctor(Guid doctorId)
    {
        var list = await _appointmentService.GetByDoctor(doctorId);
        return Ok(list);
    }
    [Authorize(Policy = "PatientPolicy")]
    [HttpGet("patient/{patientId}")]
    public async Task<IActionResult> GetByPatient(Guid patientId)
    {
        var list = await _appointmentService.GetByPatient(patientId);
        return Ok(list);
    }
}
