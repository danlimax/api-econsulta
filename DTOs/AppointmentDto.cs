using System.ComponentModel.DataAnnotations;

namespace api_econsulta.DTOs
{
    public class AppointmentCreateDto
    {
        [Required]
        public int AppointmentId { get; set; } = 0;
    }
}
