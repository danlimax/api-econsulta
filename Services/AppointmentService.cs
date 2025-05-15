using api_econsulta.Data;
using api_econsulta.DTOs;
using api_econsulta.Models;
using Microsoft.EntityFrameworkCore;



namespace api_econsulta.Services
{
    public class AppointmentService
    {
        private readonly EconsultaDbContext _context;

        public AppointmentService(EconsultaDbContext context)
        {
            _context = context;
        }

        public async Task<bool> IsSlotTaken(Guid doctorId, DateTime startTime)
        {
            return await _context.Availabilities
                .AnyAsync(a => a.DoctorId == doctorId && a.StartTime == startTime);
        }

        public async Task<Appointment?> Create(CreateAppointmentDto dto)
        {
            if (await IsSlotTaken(dto.DoctorId, dto.Time))
                return null;

            var doctor = await _context.Users.FindAsync(dto.DoctorId);
            var patient = await _context.Users.FindAsync(dto.PatientId);
            if (doctor == null || doctor.Role != "medico" || patient == null || patient.Role != "paciente")
                return null;

            var appointment = new Appointment
            {
                DoctorId = dto.DoctorId,
                PatientId = dto.PatientId,
                Time = dto.Time
            };

            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();
            return appointment;
        }

        public async Task<List<Appointment>> GetByDoctor(Guid doctorId)
        {
            return await _context.Appointments
                .Where(a => a.DoctorId == doctorId)
                .Include(a => a.Patient)
                .ToListAsync();
        }

        public async Task<List<Appointment>> GetByPatient(Guid patientId)
        {
            return await _context.Appointments
                .Where(a => a.PatientId == patientId)
                .Include(a => a.Doctor)
                .ToListAsync();
        }
    }

}