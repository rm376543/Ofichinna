using Microsoft.Extensions.Logging.Abstractions;
using Ofichina.Application.Abstractions.Common;
using Ofichina.Application.Abstractions.Interfaces;
using Ofichina.Application.UseCases.Checklists.Commands;
using Ofichina.Application.UseCases.Checklists.Handlers;
using Ofichina.Contracts;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Requests.Checklist;
using Ofichina.Domain.Aggregates;
using Ofichina.Domain.Entities;
using Ofichina.Domain.Enums;

namespace Ofichina.UnitTests.Application.Checklists;

public sealed class FinalizarChecklistCommandHandlerTests
{
    [Fact]
    public async Task Deve_Finalizar_Todos_Os_Checklists_Do_Agendamento()
    {
        var agendamento = CriarAgendamentoIniciado();
        var checklist1 = CriarChecklist(agendamento.Id);
        var checklist2 = CriarChecklist(agendamento.Id);

        var checklistRepository = new FakeChecklistRepository([checklist1, checklist2]);
        var agendamentoRepository = new FakeAgendamentoRepository(agendamento);
        var unitOfWork = new FakeUnitOfWork();

        var handler = new FinalizarChecklistCommandHandler(
            checklistRepository,
            agendamentoRepository,
            unitOfWork,
            NullLogger<FinalizarChecklistCommandHandler>.Instance);

        var command = new FinalizarChecklistCommand(new FinalizarChecklistRequest
        {
            AgendamentoId = agendamento.Id
        });

        var result = await handler.HandleAsync(command);

        Assert.True(result.IsSuccess);
        Assert.All(checklistRepository.Atualizados, checklist => Assert.True(checklist.Finalizado));
        Assert.Equal(StatusAgendamento.FINALIZADO, agendamento.Status);
        Assert.Equal(2, checklistRepository.Atualizacoes);
        Assert.Equal(1, agendamentoRepository.Atualizacoes);
        Assert.Equal(1, unitOfWork.SaveChangesCalls);
    }

    [Fact]
    public async Task Deve_Falhar_Quando_Nao_Houver_Checklists_Para_O_Agendamento()
    {
        var agendamento = CriarAgendamentoIniciado();
        var checklistRepository = new FakeChecklistRepository([]);
        var agendamentoRepository = new FakeAgendamentoRepository(agendamento);
        var unitOfWork = new FakeUnitOfWork();

        var handler = new FinalizarChecklistCommandHandler(
            checklistRepository,
            agendamentoRepository,
            unitOfWork,
            NullLogger<FinalizarChecklistCommandHandler>.Instance);

        var command = new FinalizarChecklistCommand(new FinalizarChecklistRequest
        {
            AgendamentoId = agendamento.Id
        });

        var result = await handler.HandleAsync(command);

        Assert.False(result.IsSuccess);
        Assert.Equal("Nenhum checklist encontrado para o agendamento informado.", result.Error);
        Assert.Empty(checklistRepository.Atualizados);
        Assert.Equal(0, agendamentoRepository.Atualizacoes);
        Assert.Equal(0, unitOfWork.SaveChangesCalls);
    }

    private static Agendamento CriarAgendamentoIniciado()
    {
        var agendamento = new Agendamento(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        agendamento.Iniciar();
        return agendamento;
    }

    private static Checklist CriarChecklist(Guid agendamentoId)
    {
        return new Checklist(
            agendamentoId,
            Guid.NewGuid(),
            Guid.NewGuid(),
            12000,
            "Luzes, freios e pneus",
            "Checklist inicial");
    }

    private sealed class FakeChecklistRepository : IRepository<Checklist>
    {
        private readonly List<Checklist> _checklists;

        public List<Checklist> Atualizados { get; } = [];

        public int Atualizacoes => Atualizados.Count;

        public FakeChecklistRepository(List<Checklist> checklists)
        {
            _checklists = checklists;
        }

        public Task AddAsync(Checklist entity, CancellationToken cancellationToken = default)
        {
            _checklists.Add(entity);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Checklist entity, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task<IEnumerable<Checklist>> GetAllAsync(CancellationToken cancellationToken = default)
            => Task.FromResult<IEnumerable<Checklist>>(_checklists);

        public Task<Checklist?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default, bool tracking = false)
            => Task.FromResult(_checklists.FirstOrDefault(x => x.Id == id));

        public Task UpdateAsync(Checklist entity, CancellationToken cancellationToken = default)
        {
            Atualizados.Add(entity);
            return Task.CompletedTask;
        }

        public Task HardDeleteAsync(Checklist entity, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task<PagedResponse<Checklist>> GetPagedAsync(Pagination pagination, CancellationToken cancellationToken = default)
            => Task.FromResult(new PagedResponse<Checklist>());
    }

    private sealed class FakeAgendamentoRepository : IAgendamentoRepository
    {
        private readonly Agendamento _agendamento;

        public int Atualizacoes { get; private set; }

        public FakeAgendamentoRepository(Agendamento agendamento)
        {
            _agendamento = agendamento;
        }

        public Task AddAsync(Agendamento entity, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task DeleteAsync(Agendamento entity, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task<IEnumerable<Agendamento>> GetAllAsync(CancellationToken cancellationToken = default) => Task.FromResult<IEnumerable<Agendamento>>([_agendamento]);

        public Task<PagedResponse<Agendamento>> GetPagedAsync(Pagination pagination, CancellationToken cancellationToken = default) => Task.FromResult(new PagedResponse<Agendamento>());

        public Task<Agendamento?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default, bool tracking = false) => Task.FromResult(_agendamento.Id == id ? _agendamento : null);

        public Task UpdateAsync(Agendamento entity, CancellationToken cancellationToken = default)
        {
            Atualizacoes++;
            return Task.CompletedTask;
        }

        public Task HardDeleteAsync(Agendamento entity, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task<PagedResponse<Agendamento>> GetPagedByClientePessoaAsync(Guid pessoaId, Pagination pagination, CancellationToken cancellationToken = default) => Task.FromResult(new PagedResponse<Agendamento>());

        public Task<IReadOnlyCollection<Agendamento>> GetAllWithIncludesAsync(CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyCollection<Agendamento>>([_agendamento]);

        public Task<Agendamento?> GetByIdAndPessoaAsync(Guid agendamentoId, Guid pessoaId, CancellationToken cancellationToken = default) => Task.FromResult(_agendamento.Id == agendamentoId ? _agendamento : null);

        public Task<bool> ExisteConflitoConsultorAsync(Guid horarioConsultorId, CancellationToken cancellationToken = default) => Task.FromResult(false);

        public Task<bool> ExisteConflitoVeiculoAsync(Guid veiculoId, Guid diaDisponibilidadeId, Guid horarioConsultorId, CancellationToken cancellationToken = default) => Task.FromResult(false);

        public Task<Agendamento?> BuscarAgendamentosPorPessoaId(Guid pessoaId, CancellationToken cancellationToken = default) => Task.FromResult<Agendamento?>(_agendamento);
    }

    private sealed class FakeUnitOfWork : IUnitOfWork
    {
        public int SaveChangesCalls { get; private set; }

        public Task<int> SaveChangesAsync()
        {
            SaveChangesCalls++;
            return Task.FromResult(1);
        }

        public Task BeginTransactionAsync() => Task.CompletedTask;

        public Task CommitTransactionAsync() => Task.CompletedTask;

        public Task RollbackTransactionAsync() => Task.CompletedTask;

        public ValueTask DisposeAsync() => ValueTask.CompletedTask;
    }
}
