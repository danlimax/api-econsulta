using api_econsulta.Data;
using api_econsulta.Models;
using Microsoft.EntityFrameworkCore;

namespace api_econsulta.Services
{
    public class DoctorUserService(EconsultaDbContext context)
    {
        private readonly EconsultaDbContext _context = context;

        public async Task<List<DoctorUser>> GetAllAsync()
        {
            return await _context.DoctorUsers.ToListAsync();
        }

        public async Task<DoctorUser?> GetByIdAsync(Guid id)
        {
            return await _context.DoctorUsers.FindAsync(id);
        }

        public async Task<DoctorUser> CreateAsync(DoctorUser doctor)
        {
            doctor.Id = Guid.NewGuid();
            doctor.PasswordHash = BCrypt.Net.BCrypt.HashPassword(doctor.PasswordHash);
            _context.DoctorUsers.Add(doctor);
            await _context.SaveChangesAsync();
            return doctor;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var doctor = await _context.DoctorUsers.FindAsync(id);
            if (doctor == null) return false;

            _context.DoctorUsers.Remove(doctor);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
