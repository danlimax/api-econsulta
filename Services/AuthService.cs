using api_econsulta.Data;
using api_econsulta.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace api_econsulta.Services
{
    public class AuthService
    {
        private readonly EconsultaDbContext _context;
        private readonly JwtSettings _jwtSettings;

        public AuthService(
            EconsultaDbContext context,
            IOptions<JwtSettings> jwtOptions,
            ILogger<AuthService> logger)
        {
            _context = context;
            _jwtSettings = jwtOptions.Value;
        }

        public async Task<(string Role, string Token)> ValidateLoginAsync(string email, string password)
        {
            var lowerEmail = email.ToLower();

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == lowerEmail);
            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                throw new UnauthorizedAccessException("Credenciais inválidas.");

            string role = user.Role == "medico" ? "medico" : "paciente";
            return (role, GenerateToken(user.Id, user.Email, role));
        }

        public string GenerateToken(int userId, string email, string role)
        {
            if (string.IsNullOrEmpty(_jwtSettings.Key) || _jwtSettings.Key.Length < 16)
                throw new InvalidOperationException("JWT secret key inválida.");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, userId.ToString()),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(ClaimTypes.Email, email),
                new(ClaimTypes.NameIdentifier, userId.ToString()),
                new(ClaimTypes.Role, role)
            };

            var expires = DateTime.UtcNow.AddHours(_jwtSettings.ExpirationHours);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}