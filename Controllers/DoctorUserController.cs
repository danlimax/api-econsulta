using api_econsulta.Models;
using api_econsulta.Services;
using Microsoft.AspNetCore.Mvc;

namespace api_econsulta.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [ApiController]
    [Route("api/doctors")]
    public class DoctorUserController(DoctorUserService doctorService) : ControllerBase
    {
        private readonly DoctorUserService _doctorService = doctorService;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DoctorUser>>> GetAll()
        {
            return Ok(await _doctorService.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DoctorUser>> GetById(Guid id)
        {
            var doctor = await _doctorService.GetByIdAsync(id);
            if (doctor == null) return NotFound();
            return Ok(doctor);
        }

        [HttpPost]
        public async Task<ActionResult<DoctorUser>> Create(DoctorUser doctor)
        {
            var created = await _doctorService.CreateAsync(doctor);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _doctorService.DeleteAsync(id);
            return deleted ? NoContent() : NotFound();
        }
    }
}
