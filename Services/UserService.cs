using api_econsulta.Data;
using api_econsulta.DTOs;
using api_econsulta.Exceptions;
using api_econsulta.Models;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace api_econsulta.Services
{
    public class UserService
    {
        private readonly EconsultaDbContext _context;

        public UserService(EconsultaDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }
        public async Task<List<DoctorDto>> GetAllDoctorsAsync()
        {
            return await _context.Users
                         .Where(u => u.Role == "medico") 
                         .Select(u => new DoctorDto
                         {
                             Id = u.Id,
                             Name = u.Name
                         })
                         .ToListAsync();
        }

        public async Task<List<DoctorDto>> GetAllPatientsAsync()
        {
            return await _context.Users
                         .Where(u => u.Role == "paciente") 
                         .Select(u => new DoctorDto
                         {
                             Id = u.Id,
                             Name = u.Name
                         })
                         .ToListAsync();
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User> RegisterAsync(User user, string password)
        {
            string normalizedEmail = user.Email.ToLower();
            user.Email = normalizedEmail;
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);

            _context.Users.Add(user);

            try
            {
                await _context.SaveChangesAsync();
                return user;
            }
            catch (DbUpdateException ex) when (ex.InnerException is PostgresException pgEx &&
                                               pgEx.SqlState == "23505" &&
                                               pgEx.ConstraintName == "AK_users_email")
            {
                throw new EmailAlreadyExistsException("Este e-mail já está em uso.");
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return false;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ValidatePasswordAsync(string email, string password)
        {
            var user = await GetByEmailAsync(email);
            if (user == null) return false;

            return BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
        }
    }
}