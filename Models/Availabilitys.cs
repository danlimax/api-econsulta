namespace api_econsulta.Models
{
   public class Availability
{
    public Guid Id { get; set; }

    public Guid DoctorUserId { get; set; }
    public DoctorUser DoctorUser { get; set; } = null!;

    public DateTime Start { get; set; }
    public DateTime End { get; set; }

    public bool IsBooked { get; set; }
}


}