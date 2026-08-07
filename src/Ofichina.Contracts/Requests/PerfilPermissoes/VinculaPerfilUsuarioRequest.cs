namespace Ofichina.Contracts.Requests.PerfilPermissoes
{
    public class VinculaPerfilUsuarioRequest
    {
        public Guid PerfilId { get; set; }
        public Guid UsuarioId { get; set; }
    }
}
