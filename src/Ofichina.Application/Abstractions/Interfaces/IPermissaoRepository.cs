using Ofichina.Domain.Entities;
using Ofichina.Domain.Common;

namespace Ofichina.Application.Abstractions.Interfaces;

public interface IPermissaoRepository : IRepository<Permissao>
{
    Task<Permissao?> GetByCodigoAsync(string codigo, CancellationToken cancellationToken = default);
}
