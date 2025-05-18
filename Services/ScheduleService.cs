using api_econsulta.Data;
using api_econsulta.DTOs;
using api_econsulta.Models;
using Microsoft.EntityFrameworkCore;

namespace api_econsulta.Services
{
    public class ScheduleService
    {
        private readonly EconsultaDbContext _context;

        public ScheduleService(EconsultaDbContext context)
        {
            _context = context;
        }

        public async Task<Schedule> CreateSchedule(Schedule schedule)
        {
            _context.Schedule.Add(schedule);
            await _context.SaveChangesAsync();
            return schedule;
        }

        public async Task<IEnumerable<Schedule>> GetAllAvailableSpotsByDoctorId(int doctorId)
        {
            var availableSpots = await _context.Schedule
                .Where(s => s.DoctorId == doctorId)
                .Where(s => s.StartTime >= DateTime.UtcNow)
                .Where(s => s.PatientId == null)
                .OrderBy(s => s.StartTime)
                .ToListAsync();

            return availableSpots;
        }

        public async Task<IEnumerable<Schedule>> GetAllBookedAppointmentsByDoctorId(int doctorId)
        {
            var bookedAppointments = await _context.Schedule
                .Where(s => s.DoctorId == doctorId)
                .Where(s => s.StartTime >= DateTime.UtcNow)
                .Where(s => s.PatientId != null)
                .Include(s => s.Patient)
                .OrderBy(s => s.StartTime)
                .ToListAsync();

            return bookedAppointments;
        }

        public async Task<IEnumerable<Schedule>> GetAllBookedAppointmentsByPatientId(int patientId)
        {
            var bookedAppointments = await _context.Schedule
                .Where(s => s.PatientId == patientId)
                .Where(s => s.StartTime >= DateTime.UtcNow)
                .OrderBy(s => s.StartTime)
                .Include(s => s.Doctor)
                .ToListAsync();

            return bookedAppointments;
        }

        public async Task<Schedule?> GetByIdAsync(int id)
        {
            return await _context.Schedule.FindAsync(id);
        }

        public async Task<bool> UpdateAppointment(int id, ScheduleUpdateDto dto)
        {
            var schedule = await GetByIdAsync(id);
            if (schedule == null)
                throw new Exception("Schedule not found.");

            if (schedule.StartTime < DateTime.UtcNow)
                throw new Exception("Cannot update appointment in the past.");

            if (schedule.EndTime <= schedule.StartTime)
                    throw new Exception("End time must be after start time.");

            schedule.StartTime = dto.StartTime.ToUniversalTime();
            schedule.EndTime = dto.EndTime.ToUniversalTime();

            _context.Schedule.Update(schedule);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}