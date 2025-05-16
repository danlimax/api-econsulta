namespace api_econsulta.DTOs
{
    public class DoctorDto
{
    public Guid Id { get; set; }
    public string DoctorName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Specialty { get; set; } = null!;
}

}