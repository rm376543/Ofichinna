namespace Ofichina.Contracts.Responses.Authentication
{
    /// <summary>
    /// Dados básicos do JWT emitido.
    /// </summary>
    public class JwtResponse
    {
        public string AccessToken { get; set; } = string.Empty;

        public DateTime ExpiraEm { get; set; }
    }
}
