using Ofichina.Contracts.Common;
using Ofichina.Contracts.Requests.Autenticacao;
using Ofichina.Contracts.Requests.Usuario;
using Ofichina.Contracts.Responses.Authentication;

namespace Ofichina.Application.Abstractions.Authentication.Service
{
    /// <summary>
    /// Contrato principal do fluxo de autenticação.
    /// </summary>
    public interface IAuthService
    {
        Task<Result<AuthenticationResponse>> AutenticarAsync(
            AutenticacaoRequest request,
            CancellationToken cancellationToken = default);

        Task<Result<AuthenticationResponse>> CadastrarAsync(
            CadastrarUsuarioRequest request,
            CancellationToken cancellationToken = default);
    }
}
