using System.ComponentModel.DataAnnotations;

namespace api_econsulta.DTOs
{
    public class CreateAvailabilityDto
{
    [Required]
    public Guid DoctorId { get; set; }  // usar mesmo nome da entidade

    [Required]
    public DateTime StartTime { get; set; }

    [Required]
    public DateTime EndTime { get; set; }
}


}