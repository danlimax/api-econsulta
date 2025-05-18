using api_econsulta.DTOs;
using api_econsulta.Services;
using Microsoft.AspNetCore.Mvc;

namespace api_econsulta.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(AuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            try
            {
                var (role, token) = await _authService.ValidateLoginAsync(dto.Email, dto.Password);
                return Ok(new { token, role });
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Falha de login.");
                return Unauthorized(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado no login.");
                return StatusCode(500, new { error = "Erro interno no servidor." });
            }
        }
    }
}
