using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace api_econsulta.Models
{
    public class DoctorUser
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public string DoctorName { get; set; }  = null!;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [JsonIgnore] // Não expõe o hash da senha na serialização
        public string PasswordHash { get; set; } = null!;
        [Required]
        public string Specialty { get; set; }  = null!;
        
        // Relacionamento com as disponibilidades do médico
        [JsonIgnore] // Evita ciclos de referência na serialização
        public ICollection<Availability> Availabilities { get; set; } = [];

    }
}