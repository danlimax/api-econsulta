using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace api_econsulta.Models
{
    public class Schedule
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("doctor_id")]
        public int DoctorId { get; set; }

        [Column("patient_id")]
        [AllowNull]
        public int? PatientId { get; set; } = null;

        [Required]
        [Column("start_time")]
        public DateTime StartTime { get; set; }

        [Required]
        [Column("end_time")]
        public DateTime EndTime { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }

        [ForeignKey("DoctorId")]
        public virtual User Doctor { get; set; } = null!;

        [ForeignKey("PatientId")]
        [AllowNull]
        public virtual User Patient { get; set; } = null;
    }
}