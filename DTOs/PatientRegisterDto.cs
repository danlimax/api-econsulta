using System.ComponentModel.DataAnnotations;

namespace api_econsulta.DTOs
{
    public class PatientRegisterDto
    {
        [Required]
        public string PatientName { get; set; } = null!; // Nome do paciente

        [Required, EmailAddress]
        public string Email { get; set; } = null!;

        [Required, MinLength(6)]
        public string Password { get; set; } = null!;
    }
}
