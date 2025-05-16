using api_econsulta.Data;
using api_econsulta.Models;
using Microsoft.EntityFrameworkCore;

namespace api_econsulta.Services
{
    public class PatientUserService(EconsultaDbContext context)
    {
        private readonly EconsultaDbContext _context = context;

        public async Task<List<PatientUser>> GetAllAsync()
        {
            return await _context.PatientUsers.ToListAsync();
        }

        public async Task<PatientUser?> GetByIdAsync(Guid id)
        {
            return await _context.PatientUsers.FindAsync(id);
        }

        public async Task<PatientUser> CreateAsync(PatientUser patient)
        {
            patient.Id = Guid.NewGuid();
            patient.PasswordHash = BCrypt.Net.BCrypt.HashPassword(patient.PasswordHash);
            _context.PatientUsers.Add(patient);
            await _context.SaveChangesAsync();
            return patient;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var patient = await _context.PatientUsers.FindAsync(id);
            if (patient == null) return false;

            _context.PatientUsers.Remove(patient);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
