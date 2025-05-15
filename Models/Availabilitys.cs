namespace api_econsulta.Models
{
    public class Availability
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid DoctorId { get; set; }
    public User Doctor { get; set; } = null!; // Role = medico
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
}
}