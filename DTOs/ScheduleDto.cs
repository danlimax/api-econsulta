using System.ComponentModel.DataAnnotations;

namespace api_econsulta.DTOs
{
    public class ScheduleCreateDto
    {
        [Required]
        public DateTime StartTime { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime EndTime { get; set; } = DateTime.UtcNow;
    }

    public class ScheduleUpdateDto
    {
        [Required]
        public DateTime StartTime { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime EndTime { get; set; } = DateTime.UtcNow;
    }
}
