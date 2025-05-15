using Microsoft.AspNetCore.Mvc;
using api_econsulta.DTOs;
using api_econsulta.Services;

namespace api_econsulta.Controllers
{
[ApiController]
[Route("api/auth")]
public class AuthController(AuthService authService) : ControllerBase
{
    private readonly AuthService _authService = authService;

    [HttpPost("register")]
    public async Task<IActionResult> Register(UserRegisterDto dto)
    {
        if (await _authService.EmailExists(dto.Email))
            return BadRequest("Email já cadastrado.");

        await _authService.Register(dto);
        return Ok(new { message = "Usuário registrado com sucesso." });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(UserLoginDto dto)
    {
        var user = await _authService.ValidateUser(dto.Email, dto.Password);
        if (user == null)
            return Unauthorized("Credenciais inválidas.");

        var token = _authService.GenerateToken(user);
        return Ok(new { token });
    }
}

}
