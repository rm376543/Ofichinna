using Ofichina.Contracts.Common;
using Ofichina.Domain.Aggregates;
using Ofichina.Domain.Entities;

namespace Ofichina.Application.Abstractions.Interfaces.Repository;

public interface IAgendamentoRepository : IRepository<Agendamento>
{
    Task<PagedResponse<Agendamento>> GetPagedByClientePessoaAsync(Guid pessoaId, Pagination pagination, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Agendamento>> GetAllWithIncludesAsync(CancellationToken cancellationToken = default);

    Task<Agendamento?> GetByIdAndPessoaAsync(Guid agendamentoId, Guid pessoaId, CancellationToken cancellationToken = default);

    Task<bool> ExisteConflitoConsultorAsync(Guid horarioConsultorId, CancellationToken cancellationToken = default);

    Task<bool> ExisteConflitoVeiculoAsync(Guid veiculoId, Guid diaDisponibilidadeId, Guid horarioConsultorId, CancellationToken cancellationToken = default);

    Task<Agendamento?> BuscarAgendamentosPorPessoaId(Guid pessoaId, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<VwAgendamentoPessoa>> GetAgendamentosUsuarioViewByPessoaAsync(Guid pessoaId, CancellationToken cancellationToken = default);

    Task<VwAgendamentoPessoa?> GetAgendamentoUsuarioViewByIdAsync(Guid pessoaId, Guid agendamentosId, CancellationToken cancellationToken = default);
}
