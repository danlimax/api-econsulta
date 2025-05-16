using api_econsulta.Data;
using api_econsulta.DTOs;
using api_econsulta.Models;
using api_econsulta.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace api_econsulta.Services
{
    public class AuthService(
        EconsultaDbContext context,
        IOptions<JwtSettings> jwtOptions,
        ILogger<AuthService> logger)
    {
        private readonly EconsultaDbContext _context = context;
        private readonly JwtSettings _jwtSettings = jwtOptions.Value;
        private readonly ILogger<AuthService> _logger = logger;

        public async Task<DoctorUser> RegisterDoctorAsync(DoctorRegisterDto dto)
        {
            if (await _context.DoctorUsers.AnyAsync(d => d.Email.ToLower() == dto.Email.ToLower()))
                throw new Exception("Email já cadastrado para médico.");

            var doctor = new DoctorUser
            {
                Id = Guid.NewGuid(),
                DoctorName = dto.DoctorName,
                Email = dto.Email.ToLower().Trim(),
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password, workFactor: 12),
                Specialty = dto.Specialty
            };

            _context.DoctorUsers.Add(doctor);
            await _context.SaveChangesAsync();

            return doctor;
        }

        public async Task<PatientUser> RegisterPatientAsync(PatientRegisterDto dto)
        {
            if (await _context.PatientUsers.AnyAsync(p => p.Email.ToLower() == dto.Email.ToLower()))
                throw new Exception("Email já cadastrado para paciente.");

            var patient = new PatientUser
            {
                Id = Guid.NewGuid(),
                PatientName = dto.PatientName, // Certifique-se de que o DTO tem esse campo
                Email = dto.Email.ToLower().Trim(),
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password, workFactor: 12)
            };

            _context.PatientUsers.Add(patient);
            await _context.SaveChangesAsync();

            return patient;
        }

       public async Task<(string Role, string Token)> ValidateDoctorOrPatient(string email, string password)
{
    var lowerEmail = email.ToLower();

    var doctor = await _context.DoctorUsers.FirstOrDefaultAsync(d => d.Email == lowerEmail);
    if (doctor != null && BCrypt.Net.BCrypt.Verify(password, doctor.PasswordHash))
        return ("medico", GenerateToken(doctor.Id, doctor.Email, "medico"));

    var patient = await _context.PatientUsers.FirstOrDefaultAsync(p => p.Email == lowerEmail);
    if (patient != null && BCrypt.Net.BCrypt.Verify(password, patient.PasswordHash))
        return ("paciente", GenerateToken(patient.Id, patient.Email, "paciente"));

    throw new UnauthorizedAccessException("Credenciais inválidas.");
}


        public string GenerateToken(Guid id, string email, string role)
        {
            if (string.IsNullOrEmpty(_jwtSettings.Key) || _jwtSettings.Key.Length < 16)
                throw new InvalidOperationException("JWT secret key inválida.");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, id.ToString()),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(ClaimTypes.Email, email),
                new(ClaimTypes.NameIdentifier, id.ToString()),
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
