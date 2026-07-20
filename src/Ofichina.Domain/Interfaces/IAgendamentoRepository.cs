using Ofichina.Domain.Common;
using Ofichina.Domain.Aggregates;

namespace Ofichina.Domain.Interfaces;

public interface IAgendamentoRepository : IRepository<Agendamento>
{
    Task<PagedResult<Agendamento>> GetPagedByClientePessoaAsync(Guid pessoaId, Pagination pagination, CancellationToken cancellationToken = default);

    Task<Agendamento?> GetByIdAndPessoaAsync(Guid agendamentoId, Guid pessoaId, CancellationToken cancellationToken = default);

    Task<bool> ExisteConflitoConsultorAsync(Guid consultorPessoaId, DateOnly dataAgendamento, TimeOnly horarioAgendamento, CancellationToken cancellationToken = default);

    Task<bool> ExisteConflitoVeiculoAsync(Guid veiculoId, DateOnly dataAgendamento, TimeOnly horarioAgendamento, CancellationToken cancellationToken = default);
}
