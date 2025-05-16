using System.ComponentModel.DataAnnotations;

namespace api_econsulta.DTOs
{
    public class DoctorRegisterDto
    {
        [Required]
        public string DoctorName { get; set; } = null!;

        [Required, EmailAddress]
        public string Email { get; set; } = null!;

        [Required, MinLength(6)]
        public string Password { get; set; } = null!;

        [Required]
        public string Specialty { get; set; } = null!;
    }
}
