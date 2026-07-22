using System.Reflection;
using Microsoft.Extensions.Logging.Abstractions;
using Ofichina.Application.Abstractions;
using Ofichina.Application.Abstractions.Interfaces;
using Ofichina.Application.UseCases.ItemServico.Commands;
using Ofichina.Application.UseCases.ItemServico.Handlers;
using Ofichina.Contracts.Common;
using OrdemServicoAggregate = Ofichina.Domain.Aggregates.OrdemServico;
using ItemServicoEntity = Ofichina.Domain.Entities.ItemServico;
using PecaServicoEntity = Ofichina.Domain.Entities.PecaServico;
using Ofichina.Domain.Common;
using Ofichina.Domain.Entities;
using Ofichina.Domain.Exceptions;

namespace Ofichina.UnitTests.Application.ItemServico;

public sealed class UpdateItemServicoCommandHandlerTests
{
    [Fact]
    public async Task Deve_Atualizar_Item_Quando_Nova_PecaServico_For_Encontrada()
    {
        var ordemServico = CriarOrdemServico();
        var itemServico = ordemServico.AdicionarServico(Guid.NewGuid());
        var pecaServicoAtual = CriarPecaServico(Guid.NewGuid(), Guid.NewGuid(), 1);
        DefinirPecaServico(itemServico, pecaServicoAtual);

        var novaPecaServico = CriarPecaServico(Guid.NewGuid(), Guid.NewGuid(), 1);
        var handler = CriarHandler(ordemServico, itemServico, novaPecaServico, out var unitOfWork);

        var result = await handler.HandleAsync(new UpdateItemServicoCommand
        {
            OrdemServicoId = ordemServico.Id,
            Id = itemServico.Id,
            PecaServicoId = novaPecaServico.Id
        });

        Assert.True(result.IsSuccess);
        Assert.Equal(novaPecaServico.Id, itemServico.PecaServicoId);
        Assert.Equal(1, unitOfWork.SaveChangesCalls);
    }

    [Fact]
    public async Task Deve_Retornar_Erro_Quando_Novo_Servico_Nao_For_Encontrado()
    {
        var ordemServico = CriarOrdemServico();
        var itemServico = ordemServico.AdicionarServico(Guid.NewGuid());
        var pecaServicoAtual = CriarPecaServico(Guid.NewGuid(), Guid.NewGuid(), 1);
        DefinirPecaServico(itemServico, pecaServicoAtual);

        var handler = CriarHandler(ordemServico, itemServico, null, out var unitOfWork);
        var novaPecaServicoId = Guid.NewGuid();

        var result = await handler.HandleAsync(new UpdateItemServicoCommand
        {
            OrdemServicoId = ordemServico.Id,
            Id = itemServico.Id,
            PecaServicoId = novaPecaServicoId
        });

        Assert.False(result.IsSuccess);
        Assert.Equal("Peça de serviço não encontrada.", result.Error);
        Assert.Equal(0, unitOfWork.SaveChangesCalls);
    }

    private static UpdateItemServicoCommandHandler CriarHandler(OrdemServicoAggregate ordemServico, ItemServicoEntity itemServico, PecaServicoEntity? novaPecaServico, out FakeUnitOfWork unitOfWork)
    {
        var ordemRepository = new FakeOrdemServicoRepository(ordemServico);
        var itemServicoRepository = new FakeItemServicoRepository(itemServico);
        var pecaServicoRepository = new FakePecaServicoRepository(novaPecaServico);
        unitOfWork = new FakeUnitOfWork();

        return new UpdateItemServicoCommandHandler(
            ordemRepository,
            itemServicoRepository,
            pecaServicoRepository,
            unitOfWork,
            NullLogger<UpdateItemServicoCommandHandler>.Instance);
    }

    private static OrdemServicoAggregate CriarOrdemServico()
    {
        return new OrdemServicoAggregate(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            100,
            "Problema relatado de teste",
            null);
    }

    private static PecaServicoEntity CriarPecaServico(Guid servicoId, Guid pecaId, int quantidade)
    {
        return (PecaServicoEntity)Activator.CreateInstance(
            typeof(PecaServicoEntity),
            BindingFlags.Instance | BindingFlags.NonPublic,
            binder: null,
            args: [servicoId, pecaId, quantidade],
            culture: null)!;
    }

    private static void DefinirPecaServico(ItemServicoEntity itemServico, PecaServicoEntity pecaServico)
    {
        typeof(ItemServicoEntity)
            .GetProperty(nameof(ItemServicoEntity.PecaServico), BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)!
            .SetValue(itemServico, pecaServico);
    }

    private sealed class FakeOrdemServicoRepository : IOrdemServicoRepository
    {
        private readonly OrdemServicoAggregate _ordemServico;

        public FakeOrdemServicoRepository(OrdemServicoAggregate ordemServico)
        {
            _ordemServico = ordemServico;
        }

        public Task AddAsync(OrdemServicoAggregate entity, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task<OrdemServicoAggregate?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default, bool tracking = false)
            => Task.FromResult<OrdemServicoAggregate?>(id == _ordemServico.Id ? _ordemServico : null);

        public Task<IEnumerable<OrdemServicoAggregate>> GetAllAsync(CancellationToken cancellationToken = default)
            => Task.FromResult<IEnumerable<OrdemServicoAggregate>>([]);

        public Task<PagedResult<OrdemServicoAggregate>> GetPagedAsync(Pagination pagination, CancellationToken cancellationToken = default)
            => Task.FromResult(new PagedResult<OrdemServicoAggregate>([], 0, 1, pagination.PageSize));

        public Task UpdateAsync(OrdemServicoAggregate entity, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task DeleteAsync(OrdemServicoAggregate entity, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task HardDeleteAsync(OrdemServicoAggregate entity, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task<OrdemServicoAggregate?> GetByIdAsync(Guid id, bool includeItens = false, CancellationToken cancellationToken = default, bool tracking = false)
            => Task.FromResult<OrdemServicoAggregate?>(id == _ordemServico.Id ? _ordemServico : null);

        public Task<IReadOnlyCollection<OrdemServicoAggregate>> GetAllAsync(bool includeItens = false, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyCollection<OrdemServicoAggregate>>([]);
    }

    private sealed class FakePecaServicoRepository : IRepository<PecaServicoEntity>
    {
        private readonly PecaServicoEntity? _pecaServico;

        public FakePecaServicoRepository(PecaServicoEntity? pecaServico)
        {
            _pecaServico = pecaServico;
        }

        public Task AddAsync(PecaServicoEntity entity, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task<PecaServicoEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default, bool tracking = false)
            => Task.FromResult(_pecaServico is not null && _pecaServico.Id == id ? _pecaServico : null);

        public Task<IEnumerable<PecaServicoEntity>> GetAllAsync(CancellationToken cancellationToken = default)
            => Task.FromResult<IEnumerable<PecaServicoEntity>>([]);

        public Task<PagedResult<PecaServicoEntity>> GetPagedAsync(Pagination pagination, CancellationToken cancellationToken = default)
            => Task.FromResult(new PagedResult<PecaServicoEntity>([], 0, 1, pagination.PageSize));

        public Task UpdateAsync(PecaServicoEntity entity, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task DeleteAsync(PecaServicoEntity entity, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task HardDeleteAsync(PecaServicoEntity entity, CancellationToken cancellationToken = default) => Task.CompletedTask;
    }

    private sealed class FakeItemServicoRepository : IItemServicoRepository
    {
        private readonly ItemServicoEntity _itemServico;

        public FakeItemServicoRepository(ItemServicoEntity itemServico)
        {
            _itemServico = itemServico;
        }

        public Task AddAsync(ItemServicoEntity entity, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task<ItemServicoEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default, bool tracking = false)
            => Task.FromResult<ItemServicoEntity?>(_itemServico.Id == id ? _itemServico : null);

        public Task<IEnumerable<ItemServicoEntity>> GetAllAsync(CancellationToken cancellationToken = default)
            => Task.FromResult<IEnumerable<ItemServicoEntity>>([]);

        public Task<PagedResult<ItemServicoEntity>> GetPagedAsync(Pagination pagination, CancellationToken cancellationToken = default)
            => Task.FromResult(new PagedResult<ItemServicoEntity>([], 0, 1, pagination.PageSize));

        public Task UpdateAsync(ItemServicoEntity entity, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task DeleteAsync(ItemServicoEntity entity, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task HardDeleteAsync(ItemServicoEntity entity, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task<ItemServicoEntity?> GetByOrdemServicoIdAndIdAsync(Guid ordemServicoId, Guid id, CancellationToken cancellationToken = default, bool tracking = false, bool includeRelacionados = false)
            => Task.FromResult<ItemServicoEntity?>(_itemServico.OrdemServicoId == ordemServicoId && _itemServico.Id == id ? _itemServico : null);

        public Task<ItemServicoEntity?> GetByOrdemServicoIdAndPecaServicoIdAsync(Guid ordemServicoId, Guid pecaServicoId, CancellationToken cancellationToken = default, bool tracking = false)
            => Task.FromResult<ItemServicoEntity?>(_itemServico.OrdemServicoId == ordemServicoId && _itemServico.PecaServicoId == pecaServicoId ? _itemServico : null);

        public Task<IReadOnlyCollection<ItemServicoEntity>> GetByOrdemServicoIdAsync(Guid ordemServicoId, CancellationToken cancellationToken = default, bool includeRelacionados = false, bool tracking = false)
            => Task.FromResult<IReadOnlyCollection<ItemServicoEntity>>(_itemServico.OrdemServicoId == ordemServicoId ? [_itemServico] : []);

        public Task<ItemServicoEntity> AdicionarAsync(Guid ordemServicoId, Guid pecaServicoId, CancellationToken cancellationToken = default)
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
