using Microsoft.Extensions.Logging.Abstractions;
using Ofichina.Application.Abstractions.Interfaces;
using Ofichina.Application.UseCases.ServicosPecas.Commands;
using Ofichina.Application.UseCases.ServicosPecas.Handlers;
using Ofichina.Contracts.Common;
using Ofichina.Domain.Common;
using Ofichina.Domain.Entities;

namespace Ofichina.UnitTests.Application.Servicos.Pecas;

public sealed class ServicoPecaCommandHandlerTests
{
    [Fact]
    public async Task Deve_Desativar_Uma_Peca_Do_Servico()
    {
        var servico = CriarServicoComPecas(2);
        var pecaId = servico.Pecas.First().Id;
        var handler = CriarHandler(servico, out var unitOfWork);

        var result = await handler.HandleAsync(new DeleteServicoPecaCommand
        {
            ServicoId = servico.Id,
            ServicoPecaId = pecaId
        });

        Assert.True(result.IsSuccess);
        Assert.True(servico.Pecas.Single(x => x.Id == pecaId).EstaExcluida());
        Assert.False(servico.Pecas.Single(x => x.Id != pecaId).EstaExcluida());
        Assert.Equal(1, unitOfWork.SaveChangesCalls);
    }

    [Fact]
    public async Task Deve_Desativar_Todas_As_Pecas_Do_Servico()
    {
        var servico = CriarServicoComPecas(3);
        var handler = CriarHandler(servico, out var unitOfWork);

        var result = await handler.HandleAsync(new DeleteAllServicoPecasCommand
        {
            ServicoId = servico.Id
        });

        Assert.True(result.IsSuccess);
        Assert.All(servico.Pecas, peca => Assert.True(peca.EstaExcluida()));
        Assert.Equal(1, unitOfWork.SaveChangesCalls);
    }

    [Fact]
    public async Task Deve_Retornar_Erro_Quando_Peca_Ja_Estiver_Utilizada_Ao_Desativar_Uma_Peca()
    {
        var servico = CriarServicoComPecas(1);
        var peca = servico.Pecas.Single();
        peca.MarcarComoUtilizada();

        var handler = CriarHandler(servico, out var unitOfWork);

        var result = await handler.HandleAsync(new DeleteServicoPecaCommand
        {
            ServicoId = servico.Id,
            ServicoPecaId = peca.Id
        });

        Assert.False(result.IsSuccess);
        Assert.Equal("Não é possível remover uma peça já utilizada.", result.Error);
        Assert.Equal(0, unitOfWork.SaveChangesCalls);
    }

    [Fact]
    public async Task Deve_Retornar_Erro_Quando_Peca_Ja_Estiver_Utilizada_Ao_Desativar_Todas_As_Pecas()
    {
        var servico = CriarServicoComPecas(2);
        servico.Pecas.First().MarcarComoUtilizada();

        var handler = CriarHandler(servico, out var unitOfWork);

        var result = await handler.HandleAsync(new DeleteAllServicoPecasCommand
        {
            ServicoId = servico.Id
        });

        Assert.False(result.IsSuccess);
        Assert.Equal("Não é possível remover uma peça já utilizada.", result.Error);
        Assert.Equal(0, unitOfWork.SaveChangesCalls);
    }

    [Fact]
    public async Task Deve_Retornar_Erro_404_Quando_Servico_Nao_For_Encontrado_Ao_Desativar_Uma_Peca()
    {
        var servico = CriarServicoComPecas(1);
        var handler = CriarHandler(servico, out var unitOfWork);

        var result = await handler.HandleAsync(new DeleteServicoPecaCommand
        {
            ServicoId = Guid.NewGuid(),
            ServicoPecaId = servico.Pecas.Single().Id
        });

        Assert.False(result.IsSuccess);
        Assert.Equal("Serviço não encontrado.", result.Error);
        Assert.Equal(0, unitOfWork.SaveChangesCalls);
    }

    [Fact]
    public async Task Deve_Retornar_Erro_404_Quando_Servico_Nao_For_Encontrado_Ao_Desativar_Todas_As_Pecas()
    {
        var servico = CriarServicoComPecas(1);
        var handler = CriarHandler(servico, out var unitOfWork);

        var result = await handler.HandleAsync(new DeleteAllServicoPecasCommand
        {
            ServicoId = Guid.NewGuid()
        });

        Assert.False(result.IsSuccess);
        Assert.Equal("Serviço não encontrado.", result.Error);
        Assert.Equal(0, unitOfWork.SaveChangesCalls);
    }

    private static ServicoPecaCommandHandlerBase CriarHandler(Servico servico, out FakeUnitOfWork unitOfWork)
    {
        var repository = new FakeServicoRepository(servico);
        unitOfWork = new FakeUnitOfWork();
        return new ServicoPecaCommandHandlerBase(repository, unitOfWork);
    }

    private static Servico CriarServicoComPecas(int quantidade)
    {
        var servico = new Servico("Serviço teste", null, 100m);

        for (var i = 0; i < quantidade; i++)
        {
            servico.AdicionarPeca(Guid.NewGuid(), 1);
        }

        return servico;
    }

    private sealed class ServicoPecaCommandHandlerBase
    {
        private readonly DeleteServicoPecaCommandHandler _deleteHandler;
        private readonly DeleteAllServicoPecasCommandHandler _deleteAllHandler;

        public ServicoPecaCommandHandlerBase(IServicoRepository servicoRepository, IUnitOfWork unitOfWork)
        {
            _deleteHandler = new DeleteServicoPecaCommandHandler(
                servicoRepository,
                unitOfWork,
                NullLogger<DeleteServicoPecaCommandHandler>.Instance);

            _deleteAllHandler = new DeleteAllServicoPecasCommandHandler(
                servicoRepository,
                unitOfWork,
                NullLogger<DeleteAllServicoPecasCommandHandler>.Instance);
        }

        public Task<Result> HandleAsync(DeleteServicoPecaCommand command) => _deleteHandler.HandleAsync(command);

        public Task<Result> HandleAsync(DeleteAllServicoPecasCommand command) => _deleteAllHandler.HandleAsync(command);
    }

    private sealed class FakeServicoRepository : IServicoRepository
    {
        private readonly Servico _servico;

        public FakeServicoRepository(Servico servico)
        {
            _servico = servico;
        }

        public Task AddAsync(Servico entity, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task<Servico?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default, bool tracking = false)
            => Task.FromResult<Servico?>(null);

        public Task<IEnumerable<Servico>> GetAllAsync(CancellationToken cancellationToken = default)
            => Task.FromResult<IEnumerable<Servico>>([]);

        public Task<PagedResult<Servico>> GetPagedAsync(Pagination pagination, CancellationToken cancellationToken = default)
            => Task.FromResult(new PagedResult<Servico>([], 0, 1, pagination.PageSize));

        public Task UpdateAsync(Servico entity, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task DeleteAsync(Servico entity, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task HardDeleteAsync(Servico entity, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task<Servico?> GetByIdAsync(Guid id, bool includePecas = false, CancellationToken cancellationToken = default, bool tracking = false)
            => Task.FromResult<Servico?>(id == _servico.Id ? _servico : null);

        public Task<IReadOnlyCollection<Servico>> GetAllAsync(bool includePecas = false, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyCollection<Servico>>([]);
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
