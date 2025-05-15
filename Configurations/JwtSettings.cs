namespace api_econsulta.Configurations
{
    public class JwtSettings
    {
        public string Key { get; set; } = string.Empty;
        public int ExpirationHours { get; set; }
    }
}