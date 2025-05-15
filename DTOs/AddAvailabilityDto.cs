namespace api_econsulta.DTOs
{
    public class AddAvailabilityDto
{
    public Guid  DoctorId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
}

}