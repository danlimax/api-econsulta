using api_econsulta.Models;
using api_econsulta.Services;
using Microsoft.AspNetCore.Mvc;

namespace api_econsulta.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [ApiController]
    [Route("api/patients")]
    public class PatientUserController(PatientUserService patientService) : ControllerBase
    {
        private readonly PatientUserService _patientService = patientService;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PatientUser>>> GetAll()
        {
            return Ok(await _patientService.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PatientUser>> GetById(Guid id)
        {
            var patient = await _patientService.GetByIdAsync(id);
            if (patient == null) return NotFound();
            return Ok(patient);
        }

        [HttpPost]
        public async Task<ActionResult<PatientUser>> Create(PatientUser patient)
        {
            var created = await _patientService.CreateAsync(patient);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _patientService.DeleteAsync(id);
            return deleted ? NoContent() : NotFound();
        }
    }
}
