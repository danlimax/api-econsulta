using api_econsulta.Models;
using Microsoft.EntityFrameworkCore;


namespace api_econsulta.Data
{
    public class EconsultaDbContext : DbContext
    {
        public EconsultaDbContext(DbContextOptions<EconsultaDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Schedule> Schedule { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure User entity
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("users");
                
                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .UseIdentityColumn();
                
                entity.Property(e => e.Email)
                    .HasColumnName("email")
                    .HasColumnType("varchar")
                    .IsRequired();
                
                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasColumnType("varchar")
                    .IsRequired();
                
                entity.Property(e => e.Role)
                    .HasColumnName("role")
                    .HasColumnType("varchar")
                    .IsRequired();
                
                entity.Property(e => e.PasswordHash)
                    .HasColumnName("password_hash")
                    .HasColumnType("varchar")
                    .IsRequired();
                
                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasColumnType("timestamptz")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");
                
                entity.Property(e => e.UpdatedAt)
                    .HasColumnName("updated_at")
                    .HasColumnType("timestamptz")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            // Configure Schedule entity
            modelBuilder.Entity<Schedule>(entity =>
            {
                entity.ToTable("schedule");
                
                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .UseIdentityColumn();
                
                entity.Property(e => e.DoctorId)
                    .HasColumnName("doctor_id")
                    .IsRequired();

                entity.Property(e => e.PatientId)
                    .HasColumnName("patient_id");
                
                entity.Property(e => e.StartTime)
                    .HasColumnName("start_time")
                    .HasColumnType("timestamptz")
                    .IsRequired();
                
                entity.Property(e => e.EndTime)
                    .HasColumnName("end_time")
                    .HasColumnType("timestamptz")
                    .IsRequired();
                
                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasColumnType("timestamptz")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");
                
                entity.Property(e => e.UpdatedAt)
                    .HasColumnName("updated_at")
                    .HasColumnType("timestamptz")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                // Doctor schedule relationship
                entity.HasOne(s => s.Doctor)
                    .WithMany(u => u.DoctorSchedules)
                    .HasForeignKey(s => s.DoctorId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Patient schedule relationship
                entity.HasOne(s => s.Patient)
                    .WithMany(u => u.PatientSchedules)
                    .HasForeignKey(s => s.PatientId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}