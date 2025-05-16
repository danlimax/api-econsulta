using System.ComponentModel.DataAnnotations;

namespace api_econsulta.DTOs
{

    public class CreateAppointmentDto
    {
        public Guid DoctorId { get; set; }
        public Guid PatientId { get; set; }
        [Required]
       public DateTime Time { get; set; } 

        [Required]
        public Guid AvailabilityId { get; set; }
       
    }

}