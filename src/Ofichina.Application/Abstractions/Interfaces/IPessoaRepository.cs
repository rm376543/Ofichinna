using Ofichina.Domain.Entities;
using Ofichina.Domain.Common;

namespace Ofichina.Application.Abstractions.Interfaces;

public interface IPessoaRepository : IRepository<Pessoa>
{
    Task<Pessoa?> GetByUsuarioIdAsync(Guid usuarioId, CancellationToken cancellationToken = default);
}

