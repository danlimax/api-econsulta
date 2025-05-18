using System.ComponentModel.DataAnnotations;

namespace api_econsulta.DTOs
{
    public class UserRegisterDto
    {
        [Required]
        public string Name { get; set; } = null!; 

        [Required, EmailAddress]
        public string Email { get; set; } = null!;

        [Required, MinLength(6)]
        public string Password { get; set; } = null!;
    }
}
