using api_econsulta.Data;
using api_econsulta.DTOs;
using api_econsulta.Models;
using Microsoft.EntityFrameworkCore;

namespace api_econsulta.Services
{
    public class AppointmentService(EconsultaDbContext context)
    {
        private readonly EconsultaDbContext _context = context;

        // Verifica se já existe agendamento para a disponibilidade
        public async Task<bool> IsSlotTaken(Guid doctorId, DateTime time)
        {
            return await _context.Appointments
                .AnyAsync(a => a.DoctorId == doctorId && a.Time == time);
        }

        public async Task<Appointment?> Create(CreateAppointmentDto dto)
        {
            // Impede que dois pacientes agendem o mesmo horário com o mesmo médico
            if (await IsSlotTaken(dto.DoctorId, dto.Time))
                return null;

            var doctor = await _context.DoctorUsers.FindAsync(dto.DoctorId);
            var patient = await _context.PatientUsers.FindAsync(dto.PatientId);

            if (doctor == null || patient == null)
                return null;

            var appointment = new Appointment
            {
                DoctorId = doctor.Id,
                PatientId = patient.Id,
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
