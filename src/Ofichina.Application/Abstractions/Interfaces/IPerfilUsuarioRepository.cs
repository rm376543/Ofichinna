using Ofichina.Domain.Entities;
using Ofichina.Domain.Common;

namespace Ofichina.Application.Abstractions.Interfaces;

public interface IPerfilUsuarioRepository : IRepository<UsuarioPerfil>
{
    Task<bool> ExisteAsync(Guid clienteId, Guid perfilId, CancellationToken cancellationToken = default);
    Task<UsuarioPerfil?> GetByUsuarioIdPerfilIdAsync(Guid usuarioId, Guid perfilId, CancellationToken cancellationToken = default);
}