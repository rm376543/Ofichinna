using Microsoft.Extensions.Logging.Abstractions;
using Ofichina.Application.Abstractions.Interfaces.Repository;
using Ofichina.Application.UseCases.ItensServico.Commands;
using Ofichina.Application.UseCases.ItensServico.Handlers;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Requests.ItensServico;
using Ofichina.Domain.Aggregates;
using Ofichina.Domain.Entities;
using System.Reflection;

namespace Ofichina.UnitTests.Application.UseCases.ItensServico.Handlers;

public sealed class ServicoSomenteCommandHandlerTests
{
    [Fact]
    public async Task CriarServicoOrcamento_Deve_Criar_Item_Com_PecaNula_E_QuantidadeUm()
    {
        var orcamento = CriarOrcamentoEmDiagnostico();
        var servico = new Servico("Troca de óleo", null, 120m);

        var orcamentoRepository = new FakeOrcamentoRepository(orcamento);
        var itemServicoRepository = new FakeItemServicoRepository();
        var servicoRepository = new FakeServicoRepository(servico);
        var unitOfWork = new FakeUnitOfWork();

        var handler = new CreateServicoOrcamentoCommandHandler(
            orcamentoRepository,
            itemServicoRepository,
            servicoRepository,
            unitOfWork,
            NullLogger<CreateServicoOrcamentoCommandHandler>.Instance);

        var result = await handler.HandleAsync(new CreateServicoOrcamentoCommand(new CreateServicoOrcamentoRequest
        {
            OrcamentoId = orcamento.Id,
            ServicoId = servico.Id
        }));

        Assert.True(result.IsSuccess, result.Error);
        Assert.NotNull(itemServicoRepository.ItemAdicionado);
        Assert.Equal(orcamento.Id, itemServicoRepository.ItemAdicionado!.OrcamentoId);
        Assert.Equal(servico.Id, itemServicoRepository.ItemAdicionado.ServicoId);
        Assert.Null(itemServicoRepository.ItemAdicionado.PecaId);
        Assert.Equal(1, itemServicoRepository.ItemAdicionado.Quantidade);
        Assert.Equal(1, unitOfWork.SaveChangesCalls);
    }

    [Fact]
    public async Task AtualizarServicoOrcamento_Deve_Forcar_PecaNula_E_QuantidadeUm_QuandoAtualInvalida()
    {
        var orcamento = CriarOrcamentoEmDiagnostico();
        var servicoAtual = new Servico("Serviço atual", null, 100m);
        var servicoNovo = new Servico("Serviço novo", null, 150m);
        var item = ItemServico.ParaOrcamento(orcamento.Id, servicoAtual.Id, null, 2);
        DefinirQuantidade(item, 0);

        var orcamentoRepository = new FakeOrcamentoRepository(orcamento);
        var itemServicoRepository = new FakeItemServicoRepository(item, item);
        var servicoRepository = new FakeServicoRepository(servicoNovo);
        var unitOfWork = new FakeUnitOfWork();

        var handler = new UpdateServicoOrcamentoCommandHandler(
            orcamentoRepository,
            itemServicoRepository,
            servicoRepository,
            unitOfWork,
            NullLogger<UpdateServicoOrcamentoCommandHandler>.Instance);

        var result = await handler.HandleAsync(new UpdateServicoOrcamentoCommand(new UpdateServicoOrcamentoRequest
        {
            ItemServicoId = item.Id,
            OrcamentoId = orcamento.Id,
            ServicoId = servicoNovo.Id
        }));

        Assert.True(result.IsSuccess, result.Error);
        Assert.Equal(servicoNovo.Id, item.ServicoId);
        Assert.Null(item.PecaId);
        Assert.Equal(1, item.Quantidade);
        Assert.Equal(1, unitOfWork.SaveChangesCalls);
    }

    [Fact]
    public async Task CriarServicoOrdemServico_Deve_Criar_Item_Com_PecaNula_E_QuantidadeUm()
    {
        var ordemServico = CriarOrdemServicoRecebida();
        var servico = new Servico("Balanceamento", null, 80m);

        var ordemServicoRepository = new FakeOrdemServicoRepository(ordemServico);
        var itemServicoRepository = new FakeItemServicoRepository();
        var servicoRepository = new FakeServicoRepository(servico);
        var unitOfWork = new FakeUnitOfWork();

        var handler = new CreateServicoOrdemServicoCommandHandler(
            ordemServicoRepository,
            itemServicoRepository,
            servicoRepository,
            unitOfWork,
            NullLogger<CreateServicoOrdemServicoCommandHandler>.Instance);

        var result = await handler.HandleAsync(new CreateServicoOrdemServicoCommand(new CreateServicoOrdemServicoRequest
        {
            OrdemServicoId = ordemServico.Id,
            ServicoId = servico.Id
        }));

        Assert.True(result.IsSuccess, result.Error);
        Assert.NotNull(itemServicoRepository.ItemAdicionado);
        Assert.Equal(ordemServico.Id, itemServicoRepository.ItemAdicionado!.OrdemServicoId);
        Assert.Equal(servico.Id, itemServicoRepository.ItemAdicionado.ServicoId);
        Assert.Null(itemServicoRepository.ItemAdicionado.PecaId);
        Assert.Equal(1, itemServicoRepository.ItemAdicionado.Quantidade);
        Assert.Equal(1, unitOfWork.SaveChangesCalls);
    }

    [Fact]
    public async Task AtualizarServicoOrdemServico_Deve_Forcar_PecaNula_E_QuantidadeUm_QuandoAtualInvalida()
    {
        var ordemServico = CriarOrdemServicoRecebida();
        var servicoAtual = new Servico("Serviço atual", null, 100m);
        var servicoNovo = new Servico("Serviço novo", null, 150m);
        var item = ItemServico.ParaOrdemServico(ordemServico.Id, servicoAtual.Id, null, 2);
        DefinirQuantidade(item, 0);

        var ordemServicoRepository = new FakeOrdemServicoRepository(ordemServico);
        var itemServicoRepository = new FakeItemServicoRepository(item, item);
        var servicoRepository = new FakeServicoRepository(servicoNovo);
        var unitOfWork = new FakeUnitOfWork();

        var handler = new UpdateServicoOrdemServicoCommandHandler(
            ordemServicoRepository,
            itemServicoRepository,
            servicoRepository,
            unitOfWork,
            NullLogger<UpdateServicoOrdemServicoCommandHandler>.Instance);

        var result = await handler.HandleAsync(new UpdateServicoOrdemServicoCommand(new UpdateServicoOrdemServicoRequest
        {
            ItemServicoId = item.Id,
            OrdemServicoId = ordemServico.Id,
            ServicoId = servicoNovo.Id
        }));

        Assert.True(result.IsSuccess, result.Error);
        Assert.Equal(servicoNovo.Id, item.ServicoId);
        Assert.Null(item.PecaId);
        Assert.Equal(1, item.Quantidade);
        Assert.Equal(1, unitOfWork.SaveChangesCalls);
    }

    private static Orcamento CriarOrcamentoEmDiagnostico()
    {
        var orcamento = new Orcamento(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTime.UtcNow.AddDays(10),
            0,
            "Teste de orçamento");

        orcamento.IniciarDiagnostico();
        return orcamento;
    }

    private static OrdemServico CriarOrdemServicoRecebida()
    {
        return new OrdemServico(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            1000,
            "Ruído no motor",
            "Teste de OS");
    }

    private static void DefinirQuantidade(ItemServico item, int quantidade)
    {
        var property = typeof(ItemServico).GetProperty(nameof(ItemServico.Quantidade), BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        Assert.NotNull(property);
        property!.SetValue(item, quantidade);
    }

    private sealed class FakeOrcamentoRepository : IOrcamentoRepository
    {
        private readonly Orcamento? _orcamento;

        public FakeOrcamentoRepository(Orcamento? orcamento)
        {
            _orcamento = orcamento;
        }

        public Task AddAsync(Orcamento entity, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task DeleteAsync(Orcamento entity, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task<IEnumerable<Orcamento>> GetAllAsync(CancellationToken cancellationToken = default) => Task.FromResult<IEnumerable<Orcamento>>([]);

        public Task<PagedResponse<Orcamento>> GetPagedAsync(Pagination pagination, CancellationToken cancellationToken = default) => Task.FromResult(new PagedResponse<Orcamento>());

        public Task<Orcamento?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default, bool tracking = false) => Task.FromResult(_orcamento is not null && _orcamento.Id == id ? _orcamento : null);

        public Task UpdateAsync(Orcamento entity, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task HardDeleteAsync(Orcamento entity, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task<Orcamento?> GetByIdAsync(Guid id, bool includeItens = false, CancellationToken cancellationToken = default, bool tracking = false)
            => Task.FromResult(_orcamento is not null && _orcamento.Id == id ? _orcamento : null);

        public Task<IReadOnlyCollection<Orcamento>> GetAllAsync(bool includeItens = false, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyCollection<Orcamento>>([]);
    }

    private sealed class FakeOrdemServicoRepository : IOrdemServicoRepository
    {
        private readonly OrdemServico? _ordemServico;

        public FakeOrdemServicoRepository(OrdemServico? ordemServico)
        {
            _ordemServico = ordemServico;
        }

        public Task AddAsync(OrdemServico entity, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task DeleteAsync(OrdemServico entity, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task<IEnumerable<OrdemServico>> GetAllAsync(CancellationToken cancellationToken = default) => Task.FromResult<IEnumerable<OrdemServico>>([]);

        public Task<PagedResponse<OrdemServico>> GetPagedAsync(Pagination pagination, CancellationToken cancellationToken = default) => Task.FromResult(new PagedResponse<OrdemServico>());

        public Task<OrdemServico?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default, bool tracking = false) => Task.FromResult(_ordemServico is not null && _ordemServico.Id == id ? _ordemServico : null);

        public Task UpdateAsync(OrdemServico entity, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task HardDeleteAsync(OrdemServico entity, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task<OrdemServico?> GetByIdAsync(Guid id, bool includeItens = false, bool tracking = false, CancellationToken cancellationToken = default)
            => Task.FromResult(_ordemServico is not null && _ordemServico.Id == id ? _ordemServico : null);

        public Task<IReadOnlyCollection<OrdemServico>> GetAllAsync(bool includeItens = false, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyCollection<OrdemServico>>([]);
    }

    private sealed class FakeServicoRepository : IRepository<Servico>
    {
        private readonly Servico? _servico;

        public FakeServicoRepository(Servico? servico)
        {
            _servico = servico;
        }

        public Task AddAsync(Servico entity, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task DeleteAsync(Servico entity, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task<IEnumerable<Servico>> GetAllAsync(CancellationToken cancellationToken = default) => Task.FromResult<IEnumerable<Servico>>([]);

        public Task<PagedResponse<Servico>> GetPagedAsync(Pagination pagination, CancellationToken cancellationToken = default) => Task.FromResult(new PagedResponse<Servico>());

        public Task<Servico?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default, bool tracking = false) => Task.FromResult(_servico is not null && _servico.Id == id ? _servico : null);

        public Task UpdateAsync(Servico entity, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task HardDeleteAsync(Servico entity, CancellationToken cancellationToken = default) => Task.CompletedTask;
    }

    private sealed class FakeItemServicoRepository : IItemServicoRepository
    {
        public ItemServico? ItemAdicionado { get; private set; }

        private readonly ItemServico? _itemPorId;
        private readonly ItemServico? _duplicado;

        public FakeItemServicoRepository(ItemServico? itemPorId = null, ItemServico? duplicado = null)
        {
            _itemPorId = itemPorId;
            _duplicado = duplicado;
        }

        public Task AddAsync(ItemServico entity, CancellationToken cancellationToken = default)
        {
            ItemAdicionado = entity;
            return Task.CompletedTask;
        }

        public Task DeleteAsync(ItemServico entity, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task<IEnumerable<ItemServico>> GetAllAsync(CancellationToken cancellationToken = default) => Task.FromResult<IEnumerable<ItemServico>>([]);

        public Task<PagedResponse<ItemServico>> GetPagedAsync(Pagination pagination, CancellationToken cancellationToken = default) => Task.FromResult(new PagedResponse<ItemServico>());

        public Task<ItemServico?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default, bool tracking = false) => Task.FromResult(_itemPorId is not null && _itemPorId.Id == id ? _itemPorId : null);

        public Task UpdateAsync(ItemServico entity, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task HardDeleteAsync(ItemServico entity, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task<ItemServico?> GetByOrdemServicoIdAndItemServicoIdAsync(Guid ordemServicoId, Guid itemServicoId, CancellationToken cancellationToken = default, bool tracking = false, bool includeRelacionados = false)
            => Task.FromResult(_itemPorId is not null && _itemPorId.OrdemServicoId == ordemServicoId && _itemPorId.Id == itemServicoId ? _itemPorId : null);

        public Task<ItemServico?> GetByOrdemServicoIdAndServicoIdAndPecaIdAsync(Guid ordemServicoId, Guid servicoId, Guid pecaId, CancellationToken cancellationToken = default, bool tracking = false)
            => Task.FromResult(_duplicado);

        public Task<ItemServico?> GetByOrdemServicoComPecaAsync(Guid ordemServicoId, Guid servicoId, Guid pecaId, CancellationToken cancellationToken = default, bool tracking = false)
            => Task.FromResult(_duplicado);

        public Task<ItemServico?> GetByOrdemServicoSemPecaAsync(Guid ordemServicoId, Guid servicoId, CancellationToken cancellationToken = default, bool tracking = false)
            => Task.FromResult(_duplicado);

        public Task<IReadOnlyCollection<ItemServico>> GetByOrdemServicoIdAsync(Guid ordemServicoId, CancellationToken cancellationToken = default, bool includeRelacionados = false, bool tracking = false)
            => Task.FromResult<IReadOnlyCollection<ItemServico>>([]);

        public Task<ItemServico> AddAsync(Guid ordemServicoId, Guid servicoId, Guid pecaId, int quantidade, CancellationToken cancellationToken = default)
            => throw new NotImplementedException();

        public Task<IReadOnlyCollection<ItemServico>> GetByOrdemServicoIdAndServicoIdAsync(Guid ordemServicoId, Guid servicoId, CancellationToken cancellationToken = default, bool tracking = false, bool includeRelacionados = false)
            => Task.FromResult<IReadOnlyCollection<ItemServico>>([]);

        public Task<IReadOnlyCollection<ItemServico>> GetByOrcamentoIdAsync(Guid orcamentoId, CancellationToken cancellationToken = default, bool includeRelacionados = false, bool tracking = false)
            => Task.FromResult<IReadOnlyCollection<ItemServico>>([]);

        public Task<ItemServico?> GetByOrcamentoIdAndItemServicoIdAsync(Guid orcamentoId, Guid itemServicoId, CancellationToken cancellationToken = default, bool includeRelacionados = false, bool tracking = false)
            => Task.FromResult(_itemPorId is not null && _itemPorId.OrcamentoId == orcamentoId && _itemPorId.Id == itemServicoId ? _itemPorId : null);

        public Task<ItemServico?> GetByOrcamentoServicoPecaIdAsync(Guid orcamentoId, Guid servicoId, Guid? pecaId, CancellationToken cancellationToken = default, bool tracking = false)
            => Task.FromResult(_duplicado);

        public Task<ItemServico?> GetByOrcamentoComPecaAsync(Guid orcamentoId, Guid servicoId, Guid pecaId, CancellationToken cancellationToken = default, bool tracking = false)
            => Task.FromResult(_duplicado);

        public Task<ItemServico?> GetByOrcamentoSemPecaAsync(Guid orcamentoId, Guid servicoId, CancellationToken cancellationToken = default, bool tracking = false)
            => Task.FromResult(_duplicado);
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