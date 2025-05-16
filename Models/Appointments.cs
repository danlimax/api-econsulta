using System.ComponentModel.DataAnnotations;

namespace api_econsulta.Models
{
     public class Appointment
    {
        [Key]
        public Guid Id { get; set; }
        
        public Guid PatientId { get; set; }
        
        public Guid DoctorId { get; set; }
        
        public Guid AvailabilityId { get; set; }
        
        [Required]
        public DateTime Time { get; set; }

        public string Status { get; set; } = null!; // "Agendado", "Realizado", "Cancelado", etc.
        
        // Navegação
        public PatientUser Patient { get; set; } = null!;
        public DoctorUser Doctor { get; set; } = null!;
        public Availability Availability { get; set; } = null!;
    }
}

