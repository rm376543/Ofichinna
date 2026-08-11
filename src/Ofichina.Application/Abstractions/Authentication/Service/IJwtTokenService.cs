using Ofichina.Contracts.Responses.Authentication;
using Ofichina.Domain.Entities;

namespace Ofichina.Application.Abstractions.Authentication.Service
{
    /// <summary>
    /// Contrato para geração de tokens JWT.
    /// </summary>
    public interface IJwtTokenService
    {
        Task<JwtResponse> GerarTokenAsync(
            Usuario usuario,
            IReadOnlyCollection<string> perfis,
            CancellationToken cancellationToken = default);
    }
}
