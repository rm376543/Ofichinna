using Ofichina.Contracts.Common;

namespace Ofichina.Contracts.Responses.PerfilPermissoes
{
    public class DesvincularPerfilPermissao : BaseRequest
    {
        public Guid PerfilId { get; set; }
        public Guid PermissaoId { get; set; }
    }
}
