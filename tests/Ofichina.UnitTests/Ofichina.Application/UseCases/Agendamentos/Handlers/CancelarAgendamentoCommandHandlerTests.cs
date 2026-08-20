using Microsoft.Extensions.Logging.Abstractions;
using Ofichina.Application.Abstractions.Interfaces.Repository;
using Ofichina.Application.UseCases.Agendamentos.Commands;
using Ofichina.Application.UseCases.Agendamentos.Handlers;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Requests.Agendamento;
using Ofichina.Domain.Aggregates;
using Ofichina.Domain.Entities;
using Ofichina.Domain.Enums;

namespace Ofichina.UnitTests.Application.UseCases.Agendamentos.Handlers;

public sealed class CancelarAgendamentoCommandHandlerTests
{
    [Fact]
    public async Task Deve_Cancelar_Agendamento_Quando_Existir()
    {
        var agendamento = new Agendamento(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        var repository = new FakeAgendamentoRepository(agendamento);
        var unitOfWork = new FakeUnitOfWork();

        var handler = new CancelarAgendamentoCommandHandler(
            repository,
            unitOfWork,
            NullLogger<CancelarAgendamentoCommandHandler>.Instance);

        var command = new CancelarAgendamentoCommand(new CancelarAgendamentoRequest
        {
            AgendamentoId = agendamento.Id
        });

        var result = await handler.HandleAsync(command);

        Assert.True(result.IsSuccess);
        Assert.Equal(StatusAgendamento.CANCELADO, agendamento.Status);
        Assert.True(repository.FoiAtualizado);
        Assert.Equal(1, unitOfWork.SaveChangesCalls);
    }

    [Fact]
    public async Task Deve_Falhar_Quando_Agendamento_Nao_Existir()
    {
        var repository = new FakeAgendamentoRepository(null);
        var unitOfWork = new FakeUnitOfWork();

        var handler = new CancelarAgendamentoCommandHandler(
            repository,
            unitOfWork,
            NullLogger<CancelarAgendamentoCommandHandler>.Instance);

        var command = new CancelarAgendamentoCommand(new CancelarAgendamentoRequest
        {
            AgendamentoId = Guid.NewGuid()
        });

        var result = await handler.HandleAsync(command);

        Assert.False(result.IsSuccess);
        Assert.Equal("Agendamento não encontrado.", result.Error);
        Assert.False(repository.FoiAtualizado);
        Assert.Equal(0, unitOfWork.SaveChangesCalls);
    }

    [Fact]
    public async Task Deve_Falhar_Quando_Agendamento_Ja_Estiver_Finalizado()
    {
        var agendamento = new Agendamento(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        agendamento.Iniciar();
        agendamento.Finalizar();

        var repository = new FakeAgendamentoRepository(agendamento);
        var unitOfWork = new FakeUnitOfWork();

        var handler = new CancelarAgendamentoCommandHandler(
            repository,
            unitOfWork,
            NullLogger<CancelarAgendamentoCommandHandler>.Instance);

        var command = new CancelarAgendamentoCommand(new CancelarAgendamentoRequest
        {
            AgendamentoId = agendamento.Id
        });

        var result = await handler.HandleAsync(command);

        Assert.False(result.IsSuccess);
        Assert.Contains("finalizado", result.Error ?? string.Empty, StringComparison.OrdinalIgnoreCase);
        Assert.False(repository.FoiAtualizado);
        Assert.Equal(0, unitOfWork.SaveChangesCalls);
    }

    private sealed class FakeAgendamentoRepository : IAgendamentoRepository
    {
        private readonly Agendamento? _agendamento;

        public bool FoiAtualizado { get; private set; }

        public FakeAgendamentoRepository(Agendamento? agendamento)
        {
            _agendamento = agendamento;
        }

        public Task AddAsync(Agendamento entity, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task DeleteAsync(Agendamento entity, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task<IEnumerable<Agendamento>> GetAllAsync(CancellationToken cancellationToken = default)
            => Task.FromResult<IEnumerable<Agendamento>>(_agendamento is null ? [] : [_agendamento]);

        public Task<PagedResponse<Agendamento>> GetPagedAsync(Pagination pagination, CancellationToken cancellationToken = default)
            => Task.FromResult(new PagedResponse<Agendamento>());

        public Task<Agendamento?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default, bool tracking = false)
            => Task.FromResult(_agendamento is not null && _agendamento.Id == id ? _agendamento : null);

        public Task UpdateAsync(Agendamento entity, CancellationToken cancellationToken = default)
        {
            FoiAtualizado = true;
            return Task.CompletedTask;
        }

        public Task HardDeleteAsync(Agendamento entity, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task<PagedResponse<Agendamento>> GetPagedByClientePessoaAsync(Guid pessoaId, Pagination pagination, CancellationToken cancellationToken = default)
            => Task.FromResult(new PagedResponse<Agendamento>());

        public Task<IReadOnlyCollection<Agendamento>> GetAllWithIncludesAsync(CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyCollection<Agendamento>>(_agendamento is null ? [] : [_agendamento]);

        public Task<Agendamento?> GetByIdAndPessoaAsync(Guid agendamentoId, Guid pessoaId, CancellationToken cancellationToken = default)
            => Task.FromResult(_agendamento is not null && _agendamento.Id == agendamentoId ? _agendamento : null);

        public Task<bool> ExisteConflitoConsultorAsync(Guid horarioConsultorId, CancellationToken cancellationToken = default)
            => Task.FromResult(false);

        public Task<bool> ExisteConflitoVeiculoAsync(Guid veiculoId, Guid diaDisponibilidadeId, Guid horarioConsultorId, CancellationToken cancellationToken = default)
            => Task.FromResult(false);

        public Task<Agendamento?> BuscarAgendamentosPorPessoaId(Guid pessoaId, CancellationToken cancellationToken = default)
            => Task.FromResult<Agendamento?>(_agendamento);

        public Task<VwAgendamentoPessoa?> GetAgendamentoUsuarioViewByIdAsync(Guid pessoaId, Guid agendamentosId, CancellationToken cancellationToken = default)
            => Task.FromResult<VwAgendamentoPessoa?>(null);

        public Task<IReadOnlyCollection<VwAgendamentoPessoa>> GetAgendamentosUsuarioViewByPessoaAsync(Guid pessoaId, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyCollection<VwAgendamentoPessoa>>([]);
    }

    private sealed class FakeUnitOfWork : IUnitOfWork
    {
        public int SaveChangesCalls { get; private set; }

        public ValueTask DisposeAsync() => ValueTask.CompletedTask;

        public Task BeginTransactionAsync() => Task.CompletedTask;

        public Task CommitTransactionAsync() => Task.CompletedTask;

        public Task<int> SaveChangesAsync()
        {
            SaveChangesCalls++;
            return Task.FromResult(1);
        }

        public Task RollbackTransactionAsync() => Task.CompletedTask;
    }
}