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

        public async Task<IEnumerable<Schedule>> CreateSchedules(IEnumerable<Schedule> schedules)
        {
           
            foreach (var schedule in schedules)
            {
                if (schedule.EndTime <= schedule.StartTime)
                    throw new Exception("End time must be after start time for all appointments.");

                
                var conflictingAppointment = await _context.Schedule
                    .Where(s => s.DoctorId == schedule.DoctorId)
                    .Where(s =>
                        (schedule.StartTime >= s.StartTime && schedule.StartTime < s.EndTime) ||
                        (schedule.EndTime > s.StartTime && schedule.EndTime <= s.EndTime) ||
                        (schedule.StartTime <= s.StartTime && schedule.EndTime >= s.EndTime))
                    .FirstOrDefaultAsync();

                if (conflictingAppointment != null)
                    throw new Exception($"Conflicting appointment found for doctor at {schedule.StartTime.ToString("yyyy-MM-dd HH:mm")}");
            }

            await _context.Schedule.AddRangeAsync(schedules);
            await _context.SaveChangesAsync();
            return schedules;
        }

        public async Task<IEnumerable<Schedule>> UpdateSchedules(IEnumerable<Schedule> schedules)
        {
            
            var startDate = schedules.Min(s => s.StartTime.Date);
            var endDate = schedules.Max(s => s.StartTime.Date).AddDays(1);
            var doctorId = schedules.First().DoctorId;

            
            var existingSchedules = await _context.Schedule
                .Where(s => s.DoctorId == doctorId)
                .Where(s => s.StartTime >= startDate && s.StartTime < endDate)
                .ToListAsync();

            
            var schedulesToRemove = existingSchedules.Where(s => s.PatientId == null).ToList();

            if (schedulesToRemove.Any())
            {
                _context.Schedule.RemoveRange(schedulesToRemove);
            }

            
            foreach (var schedule in schedules)
            {
                if (schedule.EndTime <= schedule.StartTime)
                    throw new Exception("End time must be after start time for all appointments.");

             
                var conflictingBooked = existingSchedules
                    .Where(s => s.PatientId != null) 
                    .Where(s =>
                        (schedule.StartTime >= s.StartTime && schedule.StartTime < s.EndTime) ||
                        (schedule.EndTime > s.StartTime && schedule.EndTime <= s.EndTime) ||
                        (schedule.StartTime <= s.StartTime && schedule.EndTime >= s.EndTime))
                    .FirstOrDefault();

                if (conflictingBooked != null)
                    throw new Exception($"Conflicting with already booked appointment at {conflictingBooked.StartTime.ToString("yyyy-MM-dd HH:mm")}");
            }

           
            await _context.Schedule.AddRangeAsync(schedules);
            await _context.SaveChangesAsync();

            return schedules;
        }

        public async Task<List<Schedule>> GetByDoctorIdAsync(int doctorId)
        {
            return await _context.Schedule
                         .Where(s => s.DoctorId == doctorId)
                         .ToListAsync();
        }

          public async Task<List<Schedule>> GetByPatientIdAsync(int patientId)
        {
            return await _context.Schedule
                         .Where(s => s.PatientId == patientId)
                         .ToListAsync();
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

            if (dto.EndTime <= dto.StartTime)
                throw new Exception("End time must be after start time.");

            
            var conflictingAppointment = await _context.Schedule
                .Where(s => s.Id != id) 
                .Where(s => s.DoctorId == schedule.DoctorId)
                .Where(s =>
                    (dto.StartTime.ToUniversalTime() >= s.StartTime && dto.StartTime.ToUniversalTime() < s.EndTime) ||
                    (dto.EndTime.ToUniversalTime() > s.StartTime && dto.EndTime.ToUniversalTime() <= s.EndTime) ||
                    (dto.StartTime.ToUniversalTime() <= s.StartTime && dto.EndTime.ToUniversalTime() >= s.EndTime))
                .FirstOrDefaultAsync();

            if (conflictingAppointment != null)
                throw new Exception($"Conflicting appointment found at {conflictingAppointment.StartTime:yyyy-MM-dd HH:mm}");

            schedule.StartTime = dto.StartTime.ToUniversalTime();
            schedule.EndTime = dto.EndTime.ToUniversalTime();

            _context.Schedule.Update(schedule);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<Schedule> UpdatePatientSchedule(int scheduleId, int patientId)
        {
            var schedule = await _context.Schedule.FindAsync(scheduleId); 
            if (schedule == null)
            {
                throw new Exception("Schedule not found."); 
            }

            if (schedule.PatientId != null) 
            {
                throw new Exception("This schedule is already booked."); 
            }

            if (schedule.StartTime < DateTime.UtcNow) 
            {
                throw new Exception("Cannot book an appointment in the past."); 
            }

            schedule.PatientId = patientId; 
            _context.Schedule.Update(schedule); 
            await _context.SaveChangesAsync(); 

            return schedule; 
        }
    }
    
}