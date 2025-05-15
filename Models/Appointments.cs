namespace api_econsulta.Models
{
 public class Appointment
{
    public Guid  Id { get; set; }
    public Guid DoctorId { get; set; }
    public Guid  PatientId { get; set; }
    public DateTime Time { get; set; }

    public User Doctor { get; set; } = null!;
    public User Patient { get; set; } = null!;
}

}