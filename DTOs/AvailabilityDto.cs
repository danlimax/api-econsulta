namespace api_econsulta.DTOs
{
   public class AvailabilityDto
{
    public Guid Id { get; set; }
    public Guid DoctorId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public DoctorDto Doctor { get; set; } = null!;
}
}