using Microsoft.EntityFrameworkCore;
using api_econsulta.Data;
using api_econsulta.Models;
using api_econsulta.DTOs;

namespace api_econsulta.Services
{
    public class AvailabilityService(EconsultaDbContext context)
    {
        private readonly EconsultaDbContext _context = context;

        public async Task<Availability> CreateAvailabilityAsync(CreateAvailabilityDto dto)
        {
            var doctor = await _context.DoctorUsers.FindAsync(dto.DoctorId) 
                         ?? throw new KeyNotFoundException("Médico não encontrado.");

            var availability = new Availability
            {
                Id = Guid.NewGuid(),
                DoctorUserId = doctor.Id,
                Start = dto.StartTime,
                End = dto.EndTime,
                IsBooked = false
            };

            _context.Availabilities.Add(availability);
            await _context.SaveChangesAsync();

            return availability;
        }

        public async Task<List<AvailabilityDto>> GetDoctorAvailabilitiesAsync(Guid doctorId)
        {
            var availabilities = await _context.Availabilities
                .Where(a => a.DoctorUserId == doctorId)
                .Include(a => a.DoctorUser)
                .ToListAsync();

            return availabilities.Select(MapToDto).ToList();
        }

        // ✅ Método atualizado: agora retorna TODAS as disponibilidades com os dados dos médicos
        public async Task<List<AvailabilityDto>> GetAvailableAvailabilitiesAsync()
        {
            var availabilities = await _context.Availabilities
                .Include(a => a.DoctorUser)
                .ToListAsync();

            return availabilities.Select(MapToDto).ToList();
        }

        private AvailabilityDto MapToDto(Availability availability)
        {
            return new AvailabilityDto
            {
                Id = availability.Id,
                DoctorId = availability.DoctorUserId,
                StartTime = availability.Start,
                EndTime = availability.End,
                Doctor = new DoctorDto
                {
                    Id = availability.DoctorUser.Id,
                    DoctorName = availability.DoctorUser.DoctorName,
                    Email = availability.DoctorUser.Email,
                    Specialty = availability.DoctorUser.Specialty
                }
            };
        }
    }
}
