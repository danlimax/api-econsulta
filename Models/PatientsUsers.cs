using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace api_econsulta.Models
{
    public class PatientUser
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public string PatientName { get; set; }  = null!;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [JsonIgnore] // Não expõe o hash da senha na serialização
        public string PasswordHash { get; set; } = null!;
        [Required]
        
         [JsonIgnore]
        public ICollection<Appointment> Appointments { get; set; } = null!;
    }
}