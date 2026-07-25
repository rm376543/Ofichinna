using Ofichina.Domain.Entities;

namespace Ofichina.Application.Abstractions.Interfaces;

public interface IPessoaRepository : IRepository<Pessoa>
{
    Task<Pessoa?> GetByUsuarioIdAsync(Guid usuarioId, CancellationToken cancellationToken = default);
    Task<Pessoa?> GetByIdWithVeiculosAsync(Guid pessoaId, CancellationToken cancellationToken = default);
}

