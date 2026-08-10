namespace Ofichina.Application.Abstractions.Authentication.Service
{
    /// <summary>
    /// Contrato para leitura/validação de perfis associados ao usuário.
    /// </summary>
    public interface IProfileAuthService
    {
        Task<IReadOnlyCollection<string>> ObterPerfisAsync(
            Guid usuarioId,
            CancellationToken cancellationToken = default);

        Task<bool> PossuiPerfilAsync(
            Guid usuarioId,
            string perfil,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyCollection<string>> ObterPermissoesAsync(
            Guid usuarioId,
            CancellationToken cancellationToken = default);

        Task<bool> PossuiPermissaoAsync(
            Guid usuarioId,
            string permissao,
            CancellationToken cancellationToken = default);
    }
}
