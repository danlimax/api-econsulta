using api_econsulta.Models;
using Microsoft.EntityFrameworkCore;

namespace api_econsulta.Data
{
    public class EconsultaDbContext(DbContextOptions<EconsultaDbContext> options) : DbContext(options)
    {
        public DbSet<DoctorUser> DoctorUsers { get; set; }
        public DbSet<PatientUser> PatientUsers { get; set; }
        public DbSet<Availability> Availabilities { get; set; }
        public DbSet<Appointment> Appointments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Disponibilidade vinculada ao médico
            modelBuilder.Entity<Availability>()
    .HasOne(a => a.DoctorUser)
    .WithMany(d => d.Availabilities) // Se existir essa coleção em DoctorUser
    .HasForeignKey(a => a.DoctorUserId);


            // Agendamento com médico
            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Doctor)
                .WithMany()
                .HasForeignKey(a => a.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Agendamento com paciente
            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Patient)
                .WithMany(p => p.Appointments)
                .HasForeignKey(a => a.PatientId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
