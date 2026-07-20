using Ofichina.Domain.Common;
using Ofichina.Domain.Aggregates;

namespace Ofichina.Domain.Interfaces;

public interface IAgendamentoRepository : IRepository<Agendamento>
{
    Task<PagedResult<Agendamento>> GetPagedByClientePessoaAsync(Guid pessoaId, Pagination pagination, CancellationToken cancellationToken = default);

    Task<Agendamento?> GetByIdAndPessoaAsync(Guid agendamentoId, Guid pessoaId, CancellationToken cancellationToken = default);

    Task<bool> ExisteConflitoConsultorAsync(Guid horarioConsultorId, CancellationToken cancellationToken = default);

    Task<bool> ExisteConflitoVeiculoAsync(Guid veiculoId, Guid diaDisponibilidadeId, Guid horarioConsultorId, CancellationToken cancellationToken = default);
}
