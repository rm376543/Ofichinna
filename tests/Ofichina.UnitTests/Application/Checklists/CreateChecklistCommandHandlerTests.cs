using Microsoft.Extensions.Logging.Abstractions;
using Ofichina.Application.Abstractions.Common;
using Ofichina.Application.Abstractions.Interfaces.Repository;
using Ofichina.Application.UseCases.Checklists.Commands;
using Ofichina.Application.UseCases.Checklists.Handlers;
using Ofichina.Contracts;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Requests.Checklist;
using Ofichina.Domain.Aggregates;
using Ofichina.Domain.Entities;
using Ofichina.Domain.Enums;
using Ofichina.Domain.ValueObjects;

namespace Ofichina.UnitTests.Application.Checklists;

public sealed class CreateChecklistCommandHandlerTests
{
    [Fact]
    public async Task Deve_Criar_Checklist_Quando_Agendamento_Estiver_Ativo()
    {
        var agendamento = new Agendamento(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        var checklistRepository = new FakeChecklistRepository();
        var agendamentoRepository = new FakeAgendamentoRepository(agendamento);
        var unitOfWork = new FakeUnitOfWork();

        var handler = new CreateChecklistCommandHandler(
            checklistRepository,
            agendamentoRepository,
            unitOfWork,
            NullLogger<CreateChecklistCommandHandler>.Instance);

        var command = new CreateChecklistCommand(new CreateChecklistRequest
        {
            AgendamentoId = agendamento.Id,
            ItensVerificados = "Luzes, freios e pneus",
            Observacoes = "Checklist de entrada"
        });

        var result = await handler.HandleAsync(command);

        Assert.True(result.IsSuccess);
        Assert.Single(checklistRepository.Adicionados);
        Assert.Equal(agendamento.Id, checklistRepository.Adicionados[0].AgendamentoId);
        Assert.Equal(1, unitOfWork.SaveChangesCalls);
    }

    [Fact]
    public async Task Deve_Falhar_Quando_Agendamento_Estiver_Cancelado()
    {
        var agendamento = new Agendamento(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        agendamento.Cancelar();

        var checklistRepository = new FakeChecklistRepository();
        var agendamentoRepository = new FakeAgendamentoRepository(agendamento);
        var unitOfWork = new FakeUnitOfWork();

        var handler = new CreateChecklistCommandHandler(
            checklistRepository,
            agendamentoRepository,
            unitOfWork,
            NullLogger<CreateChecklistCommandHandler>.Instance);

        var command = new CreateChecklistCommand(new CreateChecklistRequest
        {
            AgendamentoId = agendamento.Id,
            ItensVerificados = "Luzes, freios e pneus",
            Observacoes = "Checklist de entrada"
        });

        var result = await handler.HandleAsync(command);

        Assert.False(result.IsSuccess);
        Assert.Contains("cancelado", result.Error ?? string.Empty, StringComparison.OrdinalIgnoreCase);
        Assert.Empty(checklistRepository.Adicionados);
        Assert.Equal(0, unitOfWork.SaveChangesCalls);
    }

    [Fact]
    public async Task Deve_Falhar_Quando_Agendamento_Nao_Existir()
    {
        var checklistRepository = new FakeChecklistRepository();
        var agendamentoRepository = new FakeAgendamentoRepository(null);
        var unitOfWork = new FakeUnitOfWork();

        var handler = new CreateChecklistCommandHandler(
            checklistRepository,
            agendamentoRepository,
            unitOfWork,
            NullLogger<CreateChecklistCommandHandler>.Instance);

        var command = new CreateChecklistCommand(new CreateChecklistRequest
        {
            AgendamentoId = Guid.NewGuid(),
            ItensVerificados = "Luzes, freios e pneus",
            Observacoes = "Checklist de entrada"
        });

        var result = await handler.HandleAsync(command);

        Assert.False(result.IsSuccess);
        Assert.Equal("Agendamento não encontrado.", result.Error);
        Assert.Empty(checklistRepository.Adicionados);
        Assert.Equal(0, unitOfWork.SaveChangesCalls);
    }

    private static Pessoa CriarPessoa()
    {
        return new Pessoa(
            "Cliente Teste",
            new Cpf("39053344705"),
            new Telefone("11999999999"),
            new Endereco("Rua Teste", "100", null, "Centro", "São Paulo", "SP", new Cep("01001000")),
            Guid.NewGuid());
    }

    private static Veiculo CriarVeiculo()
    {
        return new Veiculo(
            Guid.NewGuid(),
            new Placa("ABC1234"),
            "Volkswagen",
            "Gol",
            2020,
            "Prata",
            new Hodometro(100000));
    }

    private sealed class FakeChecklistRepository : IRepository<Checklist>
    {
        public List<Checklist> Adicionados { get; } = [];

        public Task AddAsync(Checklist entity, CancellationToken cancellationToken = default)
        {
            Adicionados.Add(entity);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Checklist entity, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task<IEnumerable<Checklist>> GetAllAsync(CancellationToken cancellationToken = default) => Task.FromResult<IEnumerable<Checklist>>(Adicionados);

        public Task<Checklist?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default, bool tracking = false) => Task.FromResult(Adicionados.FirstOrDefault(x => x.Id == id));

        public Task UpdateAsync(Checklist entity, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task HardDeleteAsync(Checklist entity, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task<PagedResponse<Checklist>> GetPagedAsync(Pagination pagination, CancellationToken cancellationToken = default)
            => Task.FromResult(new PagedResponse<Checklist>());
    }

    private sealed class FakeAgendamentoRepository : IAgendamentoRepository
    {
        private readonly Agendamento? _agendamento;

        public FakeAgendamentoRepository(Agendamento? agendamento)
        {
            _agendamento = agendamento;
        }

        public Task AddAsync(Agendamento entity, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task DeleteAsync(Agendamento entity, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task<IEnumerable<Agendamento>> GetAllAsync(CancellationToken cancellationToken = default) => Task.FromResult<IEnumerable<Agendamento>>(_agendamento is null ? [] : [_agendamento]);

        public Task<PagedResponse<Agendamento>> GetPagedAsync(Pagination pagination, CancellationToken cancellationToken = default) => Task.FromResult(new PagedResponse<Agendamento>());

        public Task<Agendamento?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default, bool tracking = false) => Task.FromResult(_agendamento is not null && _agendamento.Id == id ? _agendamento : null);

        public Task UpdateAsync(Agendamento entity, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task HardDeleteAsync(Agendamento entity, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task<PagedResponse<Agendamento>> GetPagedByClientePessoaAsync(Guid pessoaId, Pagination pagination, CancellationToken cancellationToken = default) => Task.FromResult(new PagedResponse<Agendamento>());

        public Task<IReadOnlyCollection<Agendamento>> GetAllWithIncludesAsync(CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyCollection<Agendamento>>(_agendamento is null ? [] : [_agendamento]);

        public Task<Agendamento?> GetByIdAndPessoaAsync(Guid agendamentoId, Guid pessoaId, CancellationToken cancellationToken = default) => Task.FromResult(_agendamento is not null && _agendamento.Id == agendamentoId ? _agendamento : null);

        public Task<bool> ExisteConflitoConsultorAsync(Guid horarioConsultorId, CancellationToken cancellationToken = default) => Task.FromResult(false);

        public Task<bool> ExisteConflitoVeiculoAsync(Guid veiculoId, Guid diaDisponibilidadeId, Guid horarioConsultorId, CancellationToken cancellationToken = default) => Task.FromResult(false);

        public Task<Agendamento?> BuscarAgendamentosPorPessoaId(Guid pessoaId, CancellationToken cancellationToken = default) => Task.FromResult<Agendamento?>(_agendamento);
    }

    private sealed class FakeVeiculoRepository : IRepository<Veiculo>
    {
        public Veiculo? Entity { get; }

        public FakeVeiculoRepository(Veiculo? entity)
        {
            Entity = entity;
        }

        public Task AddAsync(Veiculo entity, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task DeleteAsync(Veiculo entity, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task<IEnumerable<Veiculo>> GetAllAsync(CancellationToken cancellationToken = default) => Task.FromResult<IEnumerable<Veiculo>>(Entity is null ? [] : [Entity]);

        public Task<Veiculo?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default, bool tracking = false) => Task.FromResult(Entity);

        public Task UpdateAsync(Veiculo entity, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task HardDeleteAsync(Veiculo entity, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task<PagedResponse<Veiculo>> GetPagedAsync(Pagination pagination, CancellationToken cancellationToken = default) => Task.FromResult(new PagedResponse<Veiculo>());
    }

    private sealed class FakePessoaRepository : IRepository<Pessoa>
    {
        public Pessoa? Entity { get; }

        public FakePessoaRepository(Pessoa? entity)
        {
            Entity = entity;
        }

        public Task AddAsync(Pessoa entity, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task DeleteAsync(Pessoa entity, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task<IEnumerable<Pessoa>> GetAllAsync(CancellationToken cancellationToken = default) => Task.FromResult<IEnumerable<Pessoa>>(Entity is null ? [] : [Entity]);

        public Task<Pessoa?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default, bool tracking = false) => Task.FromResult(Entity);

        public Task UpdateAsync(Pessoa entity, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task HardDeleteAsync(Pessoa entity, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task<PagedResponse<Pessoa>> GetPagedAsync(Pagination pagination, CancellationToken cancellationToken = default) => Task.FromResult(new PagedResponse<Pessoa>());
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
