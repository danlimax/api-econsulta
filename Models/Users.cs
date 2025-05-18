using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace api_econsulta.Models
{
    public class User
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [EmailAddress]
        [Column("email")]
        public string Email { get; set; } = null!;

        [Required]
        [Column("name")]
        public string Name { get; set; } = null!;

        [Required]
        [Column("role")]
        public string Role { get; set; } = null!;

        [Required]
        [JsonIgnore]
        [Column("password_hash")]
        public string PasswordHash { get; set; } = null!;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }

        [JsonIgnore]
        [InverseProperty("Doctor")]
        public ICollection<Schedule>? DoctorSchedules { get; set; }

        [JsonIgnore]
        [InverseProperty("Patient")]
        public ICollection<Schedule>? PatientSchedules { get; set; }
    }
}