using api_econsulta.Data;
using api_econsulta.Models;
using Microsoft.EntityFrameworkCore;

namespace api_econsulta.Services
{
    public class AppointmentService(EconsultaDbContext context)
    {
        private readonly EconsultaDbContext _context = context;

        public async Task<Schedule> CreateAppointment(int appointmentId, int patientId)
        {
            var strategy = _context.Database.CreateExecutionStrategy();

            return await strategy.ExecuteAsync(async () =>
            {
                await using var transaction = await _context.Database.BeginTransactionAsync();

                try
                {
                    var schedule = await GetByIdLockForUpdateAsync(appointmentId);

                    if (schedule.PatientId != null)
                        throw new Exception("Appointment already has a patient.");

                    if (schedule.StartTime < DateTime.UtcNow)
                        throw new Exception("Cannot create appointment in the past.");

                    if (schedule.EndTime <= schedule.StartTime)
                        throw new Exception("End time must be after start time.");

                    schedule.PatientId = patientId;
                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();

                    return schedule;
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    throw new Exception("Error updating appointment. Please try again.", ex);
                }
                catch (Exception ex)
                {
                    throw new Exception("Error creating appointment. Please try again.", ex);
                }
            });
        }

        public async Task<Schedule?> GetByIdAsync(int id)
        {
            return await _context.Schedule.FindAsync(id);
        }

        public async Task<Schedule> GetByIdLockForUpdateAsync(int id)
        {
            // Include doctor and patient
            var schedule = await _context.Schedule
                .FromSqlRaw(@"
            SELECT *
            FROM ""schedule""
            WHERE ""id"" = {0}
            FOR UPDATE
        ", id).Include(s => s.Doctor).
        FirstOrDefaultAsync();

            if (schedule == null)
            {
                throw new Exception("Schedule not found.");
            }

            return schedule;
        }
    }
}