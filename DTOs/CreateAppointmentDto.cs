namespace api_econsulta.DTOs
{
    public class CreateAppointmentDto
{
   
    public Guid DoctorId { get; set; }
    public Guid PatientId { get; set; }  
    public DateTime Time { get; set; }
}



}