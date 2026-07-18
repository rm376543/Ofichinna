using Ofichina.Domain.Entities;

namespace Ofichina.Domain.Interfaces;

public interface IPerfilUsuarioRepository : IRepository<UsuarioPerfil>
{
    Task<bool> ExisteAsync(Guid clienteId, Guid perfilId, CancellationToken cancellationToken = default);
    Task<UsuarioPerfil?> GetByUsuarioIdPerfilIdAsync(Guid usuarioId, Guid perfilId, CancellationToken cancellationToken = default);
}