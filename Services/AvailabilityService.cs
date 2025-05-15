using api_econsulta.Data;
using api_econsulta.DTOs;
using api_econsulta.Models;
using Microsoft.EntityFrameworkCore;


namespace api_econsulta.Services
{
    public class AvailabilityService(EconsultaDbContext context)
    {
        private readonly EconsultaDbContext _context = context;

        public async Task<Availability?> Create(AddAvailabilityDto dto)
        {
            var doctor = await _context.Users.FindAsync(dto.DoctorId);
            if (doctor == null || doctor.Role != "medico")
                return null;

            var availability = new Availability
            {
                DoctorId = dto.DoctorId,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime
            };

            _context.Availabilities.Add(availability);
            await _context.SaveChangesAsync();
            return availability;
        }

        public async Task<List<Availability>> GetByDoctor(Guid doctorId)
        {
            return await _context.Availabilities
                .Where(a => a.DoctorId == doctorId)
                .OrderBy(a => a.StartTime)
                .ToListAsync();
        }

        public async Task<List<Availability>> GetAvailableSlots()
        {
            var now = DateTime.UtcNow;
            var bookedTimes = await _context.Availabilities.Select(a => a.StartTime).ToListAsync();

            return await _context.Availabilities
                .Where(a => a.StartTime > now && !bookedTimes.Contains(a.StartTime))
                .Include(a => a.Doctor)
                .OrderBy(a => a.StartTime)
                .ToListAsync();
        }

        public async Task<bool> Update(Guid id, AddAvailabilityDto dto)
        {
            var availability = await _context.Availabilities.FindAsync(id);
            if (availability == null)
                return false;

            availability.StartTime = dto.StartTime;
            availability.EndTime = dto.EndTime;

            _context.Availabilities.Update(availability);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> Delete(Guid id)
        {
            var availability = await _context.Availabilities.FindAsync(id);
            if (availability == null)
                return false;

            _context.Availabilities.Remove(availability);
            await _context.SaveChangesAsync();
            return true;
        }
    }

}