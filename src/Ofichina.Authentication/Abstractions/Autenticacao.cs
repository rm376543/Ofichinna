using Ofichina.Contracts.Requests.Autenticacao;
using Ofichina.Contracts.Requests.Cliente;
using Ofichina.Contracts.Requests.Usuario;
using Ofichina.Contracts.Responses;
using Ofichina.Domain.Entities;
using Ofichina.Domain.ValueObjects;

namespace Ofichina.Authentication.Abstractions;

/// <summary>
/// Contrato principal do fluxo de autenticação.
/// </summary>
public interface IAutenticacaoService
{
    Task<Result<AutenticacaoResponse>> AutenticarAsync(AutenticacaoRequest request, CancellationToken cancellationToken = default);

    Task<Result<AutenticacaoResponse>> CadastrarAsync(CreateClienteRequest request, CancellationToken cancellationToken = default);
}

/// <summary>
/// Contrato para consulta de usuários autenticáveis.
/// </summary>
public interface IUsuarioAutenticacaoRepository
{
    Task<Usuario?> ObterPorEmailAsync(string email, CancellationToken cancellationToken = default);
}

/// <summary>
/// Contrato para geração de tokens JWT.
/// </summary>
public interface IJwtTokenService
{
    Task<TokenJwtResponse> GerarTokenAsync(Usuario usuario, IReadOnlyCollection<string> perfis, CancellationToken cancellationToken = default);
}

/// <summary>
/// Contrato para leitura/validação de perfis associados ao usuário.
/// </summary>
public interface IPerfilAutorizacaoService
{
    Task<IReadOnlyCollection<string>> ObterPerfisAsync(Guid usuarioId, CancellationToken cancellationToken = default);

    Task<bool> PossuiPerfilAsync(Guid usuarioId, string perfil, CancellationToken cancellationToken = default);
}

/// <summary>
/// Contrato para hash e validação de senha.
/// </summary>
public interface ISenhaHasher
{
    string GerarHash(string senha);

    bool Verificar(string senha, string hash);
}