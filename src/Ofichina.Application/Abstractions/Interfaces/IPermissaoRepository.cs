using Ofichina.Domain.Entities;

namespace Ofichina.Application.Abstractions.Interfaces;

public interface IPermissaoRepository : IRepository<Permissao>
{
    Task<Permissao?> GetByCodigoAsync(string codigo, CancellationToken cancellationToken = default);
}
