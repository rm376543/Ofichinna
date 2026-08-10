using Ofichina.Domain.Entities;

namespace Ofichina.Application.Abstractions.Authentication.Repository
{
    /// <summary>
    /// Contrato para consulta de usuários autenticáveis.
    /// </summary>
    public interface IUserAuthRepository
    {
        Task<Usuario?> ObterPorEmailAsync(string email, CancellationToken cancellationToken = default);
    }
}
