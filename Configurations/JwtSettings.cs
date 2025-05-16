namespace api_econsulta.Configurations
{
    public class JwtSettings
    {
        /// <summary>
        /// Chave secreta para assinatura de tokens JWT
        /// </summary>
        public string Key { get; set; } = string.Empty;

        /// <summary>
        /// Emissor do token (opcional)
        /// </summary>
        public string? Issuer { get; set; }

        /// <summary>
        /// Audiência do token (opcional)
        /// </summary>
        public string? Audience { get; set; }

        /// <summary>
        /// Tempo de expiração do token em horas
        /// </summary>
        public int ExpirationHours { get; set; } = 24;
    }
}