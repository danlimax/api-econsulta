using api_econsulta.Models;
using Microsoft.EntityFrameworkCore;

namespace api_econsulta.Data
{

public class EconsultaDbContext(DbContextOptions<EconsultaDbContext> options) : DbContext(options)
{
        public DbSet<User> Users { get; set; }
    public DbSet<Availability> Availabilities { get; set; }
    public DbSet<Appointment> Appointments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<Availability>()
            .HasOne(a => a.Doctor)
            .WithMany()
            .HasForeignKey(a => a.DoctorId);

        modelBuilder.Entity<Appointment>()
            .HasOne(a => a.Doctor)
            .WithMany()
            .HasForeignKey(a => a.DoctorId);

        modelBuilder.Entity<Appointment>()
            .HasOne(a => a.Patient)
            .WithMany(u => u.Appointments)
            .HasForeignKey(a => a.PatientId);
    }
}
}