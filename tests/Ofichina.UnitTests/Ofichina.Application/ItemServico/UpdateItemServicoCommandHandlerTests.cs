using System.Reflection;
using Microsoft.Extensions.Logging.Abstractions;
using Ofichina.Application.Abstractions.Common;
using Ofichina.Application.Abstractions.Interfaces;
using Ofichina.Application.UseCases.ItensServico.Commands;
using Ofichina.Application.UseCases.ItensServico.Handlers;
using Ofichina.Domain.Aggregates;
using Ofichina.Domain.Common;
using Ofichina.Domain.Entities;

namespace Ofichina.UnitTests.Application.ItensServico;

public sealed class UpdateItemServicoCommandHandlerTests
{
    [Fact]
    public async Task Deve_Atualizar_Item_Quando_Nova_ServicoPeca_For_Encontrada()
    {
        var ordemServico = CriarOrdemServico();
        var itemServico = ordemServico.AdicionarServico();
        var servicoPecaAtual = CriarServicoPeca(Guid.NewGuid(), Guid.NewGuid(), 1);
        itemServico.AdicionarPeca(servicoPecaAtual, 1);

        var novaServicoPeca = CriarServicoPeca(Guid.NewGuid(), Guid.NewGuid(), 1);
        var handler = CriarHandler(ordemServico, itemServico, novaServicoPeca, out var unitOfWork);

        var result = await handler.HandleAsync(new UpdateItemServicoCommand
        {
            OrdemServicoId = ordemServico.Id,
            Id = itemServico.Id,
        });

        Assert.True(result.IsSuccess);
        Assert.Equal(novaServicoPeca.Id, itemServico.Pecas.First().Id);
        Assert.Equal(1, unitOfWork.SaveChangesCalls);
    }

    [Fact]
    public async Task Deve_Retornar_Erro_Quando_Novo_Servico_Nao_For_Encontrado()
    {
        var ordemServico = CriarOrdemServico();
        var itemServico = ordemServico.AdicionarServico();
        var servicoPecaAtual = CriarServicoPeca(Guid.NewGuid(), Guid.NewGuid(), 1);
        itemServico.AdicionarPeca(servicoPecaAtual, 1);

        var handler = CriarHandler(ordemServico, itemServico, null, out var unitOfWork);
        var novaServicoPecaId = Guid.NewGuid();

        var result = await handler.HandleAsync(new UpdateItemServicoCommand
        {
            Id = itemServico.Id,
            OrdemServicoId = ordemServico.Id
        });

        Assert.False(result.IsSuccess);
        Assert.Equal("Peça de serviço não encontrada.", result.Error);
        Assert.Equal(0, unitOfWork.SaveChangesCalls);
    }

    private static UpdateItemServicoCommandHandler CriarHandler(OrdemServico ordemServico, ItemServico itemServico, ServicoPeca? novaServicoPeca, out FakeUnitOfWork unitOfWork)
    {
        var ordemRepository = new FakeOrdemServicoRepository(ordemServico);
        var itemServicoRepository = new FakeItemServicoRepository(itemServico);
        var servicoPecaRepository = new FakeServicoPecaRepository(novaServicoPeca);
        unitOfWork = new FakeUnitOfWork();

        return new UpdateItemServicoCommandHandler(
            ordemRepository,
            itemServicoRepository,
            servicoPecaRepository,
            unitOfWork,
            NullLogger<UpdateItemServicoCommandHandler>.Instance);
    }

    private static OrdemServico CriarOrdemServico()
    {
        return new OrdemServico(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            100,
            "Problema relatado de teste",
            null);
    }

    private static ServicoPeca CriarServicoPeca(Guid servicoId, Guid pecaId, int quantidade)
    {
        return (ServicoPeca)Activator.CreateInstance(
            typeof(ServicoPeca),
            BindingFlags.Instance | BindingFlags.NonPublic,
            binder: null,
            args: [servicoId, pecaId, quantidade],
            culture: null)!;
    }

    private sealed class FakeOrdemServicoRepository : IOrdemServicoRepository
    {
        private readonly OrdemServico _ordemServico;

        public FakeOrdemServicoRepository(OrdemServico ordemServico)
        {
            _ordemServico = ordemServico;
        }

        public Task AddAsync(OrdemServico entity, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task<OrdemServico?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default, bool tracking = false)
            => Task.FromResult<OrdemServico?>(id == _ordemServico.Id ? _ordemServico : null);

        public Task<IEnumerable<OrdemServico>> GetAllAsync(CancellationToken cancellationToken = default)
            => Task.FromResult<IEnumerable<OrdemServico>>([]);

        public Task<PagedResult<OrdemServico>> GetPagedAsync(Pagination pagination, CancellationToken cancellationToken = default)
            => Task.FromResult(new PagedResult<OrdemServico>([], 0, 1, pagination.PageSize));

        public Task UpdateAsync(OrdemServico entity, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task DeleteAsync(OrdemServico entity, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task HardDeleteAsync(OrdemServico entity, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task<OrdemServico?> GetByIdAsync(Guid id, bool includeItens = false, CancellationToken cancellationToken = default, bool tracking = false)
            => Task.FromResult<OrdemServico?>(id == _ordemServico.Id ? _ordemServico : null);

        public Task<IReadOnlyCollection<OrdemServico>> GetAllAsync(bool includeItens = false, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyCollection<OrdemServico>>([]);
    }

    private sealed class FakeServicoPecaRepository : IRepository<ServicoPeca>
    {
        private readonly ServicoPeca? _servicoPeca;

        public FakeServicoPecaRepository(ServicoPeca? servicoPeca)
        {
            _servicoPeca = servicoPeca;
        }

        public Task AddAsync(ServicoPeca entity, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task<ServicoPeca?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default, bool tracking = false)
            => Task.FromResult(_servicoPeca is not null && _servicoPeca.Id == id ? _servicoPeca : null);

        public Task<IEnumerable<ServicoPeca>> GetAllAsync(CancellationToken cancellationToken = default)
            => Task.FromResult<IEnumerable<ServicoPeca>>([]);

        public Task<PagedResult<ServicoPeca>> GetPagedAsync(Pagination pagination, CancellationToken cancellationToken = default)
            => Task.FromResult(new PagedResult<ServicoPeca>([], 0, 1, pagination.PageSize));

        public Task UpdateAsync(ServicoPeca entity, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task DeleteAsync(ServicoPeca entity, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task HardDeleteAsync(ServicoPeca entity, CancellationToken cancellationToken = default) => Task.CompletedTask;
    }

    private sealed class FakeItemServicoRepository : IItemServicoRepository
    {
        private readonly ItemServico _itemServico;

        public FakeItemServicoRepository(ItemServico itemServico)
        {
            _itemServico = itemServico;
        }

        public Task AddAsync(ItemServico entity, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task<ItemServico?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default, bool tracking = false)
            => Task.FromResult<ItemServico?>(_itemServico.Id == id ? _itemServico : null);

        public Task<IEnumerable<ItemServico>> GetAllAsync(CancellationToken cancellationToken = default)
            => Task.FromResult<IEnumerable<ItemServico>>([]);

        public Task<PagedResult<ItemServico>> GetPagedAsync(Pagination pagination, CancellationToken cancellationToken = default)
            => Task.FromResult(new PagedResult<ItemServico>([], 0, 1, pagination.PageSize));

        public Task UpdateAsync(ItemServico entity, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task DeleteAsync(ItemServico entity, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task HardDeleteAsync(ItemServico entity, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task<ItemServico?> GetByOrdemServicoIdAndServicoPecaIdAsync(Guid ordemServicoId, Guid servicoPecaId, CancellationToken cancellationToken = default, bool tracking = false)
            => Task.FromResult<ItemServico?>(_itemServico.OrdemServicoId == ordemServicoId && _itemServico.Pecas.Any(p => p.Id == servicoPecaId) ? _itemServico : null);

        public Task<ItemServico?> GetByOrdemServicoIdAndItemServicoIdAsync(Guid ordemServicoId, Guid itemServicoId, CancellationToken cancellationToken = default, bool tracking = false, bool includeRelacionados = false)
            => Task.FromResult<ItemServico?>(_itemServico.OrdemServicoId == ordemServicoId && _itemServico.Id == itemServicoId ? _itemServico : null);

        public Task<IReadOnlyCollection<ItemServico>> GetByOrdemServicoIdAsync(Guid ordemServicoId, CancellationToken cancellationToken = default, bool includeRelacionados = false, bool tracking = false)
            => Task.FromResult<IReadOnlyCollection<ItemServico>>(_itemServico.OrdemServicoId == ordemServicoId ? [_itemServico] : []);

        public Task<ItemServico> AdicionarAsync(Guid ordemServicoId, Guid servicoPecaId, CancellationToken cancellationToken = default)
            => Task.FromResult(_itemServico);
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
