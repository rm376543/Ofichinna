using Microsoft.Extensions.Logging.Abstractions;
using Ofichina.Application.UseCases.OrdensServico.Commands;
using Ofichina.Application.UseCases.OrdensServico.Handlers;
using Ofichina.Contracts.Common;
using Ofichina.Domain.Entities;
using Ofichina.Domain.Enums;
using Ofichina.Domain.ValueObjects;
using OrdemServicoAggregate = Ofichina.Domain.Aggregates.OrdemServico;

namespace Ofichina.UnitTests.Application.OrdensServico;

public sealed class CreateOrdemServicoCommandHandlerTests
{
    [Fact]
    public async Task Deve_Criar_Ordem_Sem_Servicos_E_Com_Status_Recebida()
    {
        var pessoa = CriarPessoa();
        var funcionario = CriarPessoa();
        var veiculo = CriarVeiculo(pessoa.Id);

        var pessoaRepository = new FakeRepository<Pessoa>(pessoa, funcionario);
        var veiculoRepository = new FakeRepository<Veiculo>(veiculo);
        var ordemRepository = new FakeOrdemServicoRepository();
        var unitOfWork = new FakeUnitOfWork();
        var handler = new CreateOrdemServicoCommandHandler(
            ordemRepository,
            pessoaRepository,
            veiculoRepository,
            unitOfWork,
            NullLogger<CreateOrdemServicoCommandHandler>.Instance);

        var command = new CreateOrdemServicoCommand
        {
            PessoaId = pessoa.Id,
            VeiculoId = veiculo.Id,
            FuncionarioId = funcionario.Id,
            HodometroEntrada = 77290,
            ProblemaRelatado = "Barulhos durante a aceleração",
            Observacoes = "carro de dev"
        };

        var result = await handler.HandleAsync(command);

        Assert.True(result.IsSuccess);
        Assert.NotEqual(Guid.Empty, result.Value);
        Assert.Single(ordemRepository.AddedEntities);

        var ordemCriada = ordemRepository.AddedEntities.Single();
        Assert.Equal(StatusOrdemServico.Recebida, ordemCriada.Status);
        Assert.Equal(pessoa.Id, ordemCriada.PessoaId);
        Assert.Equal(veiculo.Id, ordemCriada.VeiculoId);
        Assert.Equal(funcionario.Id, ordemCriada.FuncionarioId);
        Assert.Empty(ordemCriada.Servicos);
        Assert.Equal(1, unitOfWork.SaveChangesCalls);
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

    private static Veiculo CriarVeiculo(Guid pessoaId)
    {
        return new Veiculo(
            pessoaId,
            new Placa("ABC1234"),
            "Volkswagen",
            "Gol",
            2020,
            "Prata",
            new Hodometro(100000));
    }

    private sealed class FakeRepository<TEntity> : IRepository<TEntity> where TEntity : Entity
    {
        private readonly IReadOnlyCollection<TEntity> _entities;

        public FakeRepository(params TEntity[] entities)
        {
            _entities = entities;
        }

        public Task AddAsync(TEntity entity, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default, bool tracking = false)
            => Task.FromResult(_entities.SingleOrDefault(x => x.Id == id));

        public Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
            => Task.FromResult<IEnumerable<TEntity>>([]);

        public Task<PagedResult<TEntity>> GetPagedAsync(Pagination pagination, CancellationToken cancellationToken = default)
            => Task.FromResult(new PagedResult<TEntity>([], 0, 1, pagination.PageSize));

        public Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task HardDeleteAsync(TEntity entity, CancellationToken cancellationToken = default) => Task.CompletedTask;
    }

    private sealed class FakeOrdemServicoRepository : IOrdemServicoRepository
    {
        public List<OrdemServicoAggregate> AddedEntities { get; } = [];

        public Task AddAsync(OrdemServicoAggregate entity, CancellationToken cancellationToken = default)
        {
            AddedEntities.Add(entity);
            return Task.CompletedTask;
        }

        public Task<OrdemServicoAggregate?> GetByIdAsync(Guid id, bool includeItens = false, CancellationToken cancellationToken = default, bool tracking = false)
            => Task.FromResult<OrdemServicoAggregate?>(null);

        public Task<OrdemServicoAggregate?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default, bool tracking = false)
            => Task.FromResult<OrdemServicoAggregate?>(null);

        public Task<IReadOnlyCollection<OrdemServicoAggregate>> GetAllAsync(bool includeItens = false, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyCollection<OrdemServicoAggregate>>([]);

        public Task UpdateAsync(OrdemServicoAggregate entity, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task DeleteAsync(OrdemServicoAggregate entity, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task HardDeleteAsync(OrdemServicoAggregate entity, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task<IEnumerable<OrdemServicoAggregate>> GetAllAsync(CancellationToken cancellationToken = default)
            => Task.FromResult<IEnumerable<OrdemServicoAggregate>>([]);

        public Task<PagedResult<OrdemServicoAggregate>> GetPagedAsync(Pagination pagination, CancellationToken cancellationToken = default)
            => Task.FromResult(new PagedResult<OrdemServicoAggregate>([], 0, 1, pagination.PageSize));
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