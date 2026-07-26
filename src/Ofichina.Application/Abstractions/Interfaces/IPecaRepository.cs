using Ofichina.Contracts;
using Ofichina.Contracts.Common;
using Ofichina.Domain.Entities;

namespace Ofichina.Application.Abstractions.Interfaces
{
    public interface IPecaRepository : IRepository<Peca>
    {
        Task<PagedResponse<Peca>> GetAllPecasPaginadas(Pagination pagination, CancellationToken cancellationToken = default);
    }
}
