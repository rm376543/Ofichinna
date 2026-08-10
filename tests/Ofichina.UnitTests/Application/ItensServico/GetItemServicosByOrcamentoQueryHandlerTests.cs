using Microsoft.Extensions.Logging.Abstractions;
using Ofichina.Application.UseCases.ItensServico.Handlers;
using Ofichina.Application.UseCases.ItensServico.Queries;
using Ofichina.Contracts;
using Ofichina.Contracts.Common;
using Ofichina.Domain.Aggregates;
using Ofichina.Domain.Entities;
using System.Reflection;

namespace Ofichina.UnitTests.Application.ItensServico;

public sealed class GetItemServicosByOrcamentoQueryHandlerTests
{
    [Fact]
    public async Task Deve_Agrupar_Itens_Do_Orcamento_Por_Servico_E_Preservar_Pecas()
    {
        var orcamento = new Orcamento(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTime.UtcNow.AddDays(10),
            0,
            "Teste");

        var servicoAlinhamento = new Servico("Alinhamento", null, 120m);
        var pecaBucha = new Peca("Bucha", null, "PEC-001", 30m, 10);
        var pecaKit = new Peca("Kit de alinhamento", null, "PEC-002", 20m, 5);
        var servicoBalanceamento = new Servico("Balanceamento", null, 80m);
        var pecaParafuso = new Peca("Parafuso", null, "PEC-003", 10m, 20);

        var itemAlinhamentoComPeca = ItemServico.ParaOrcamento(orcamento.Id, servicoAlinhamento.Id, pecaBucha.Id, 1);
        DefinirPropriedade(itemAlinhamentoComPeca, nameof(ItemServico.Servico), servicoAlinhamento);
        DefinirPropriedade(itemAlinhamentoComPeca, nameof(ItemServico.Peca), pecaBucha);

        var itemAlinhamentoKit = ItemServico.ParaOrcamento(orcamento.Id, servicoAlinhamento.Id, pecaKit.Id, 2);
        DefinirPropriedade(itemAlinhamentoKit, nameof(ItemServico.Servico), servicoAlinhamento);
        DefinirPropriedade(itemAlinhamentoKit, nameof(ItemServico.Peca), pecaKit);

        var itemBalanceamento = ItemServico.ParaOrcamento(orcamento.Id, servicoBalanceamento.Id, pecaParafuso.Id, 4);
        DefinirPropriedade(itemBalanceamento, nameof(ItemServico.Servico), servicoBalanceamento);
        DefinirPropriedade(itemBalanceamento, nameof(ItemServico.Peca), pecaParafuso);

        var orcamentoRepository = new FakeOrcamentoRepository(orcamento);
        var itemServicoRepository = new FakeItemServicoRepository([itemAlinhamentoComPeca, itemAlinhamentoKit, itemBalanceamento]);
        var handler = new GetItemServicosByOrcamentoQueryHandler(
            orcamentoRepository,
            itemServicoRepository,
            NullLogger<GetItemServicosByOrcamentoQueryHandler>.Instance);

        var result = await handler.HandleAsync(new GetItemServicosByOrcamentoQuery { OrcamentoId = orcamento.Id });

        Assert.True(result.IsSuccess);
        var response = Assert.Single(result.Value!);
        Assert.Equal(orcamento.Id, response.OrcamentoId);
        Assert.Equal(2, response.Servicos.Count);

        var alinhamento = response.Servicos.Single(x => x.ServicoId == servicoAlinhamento.Id);
        Assert.Equal("Alinhamento", alinhamento.Descricao);
        Assert.Equal(120m, alinhamento.ValorServico);
        Assert.Equal(2, alinhamento.Pecas.Count);
        Assert.Equal(190m, alinhamento.ValorTotal);
        Assert.Contains(alinhamento.Pecas, x => x.PecaId == pecaBucha.Id && x.ValorTotal == 30m);
        Assert.Contains(alinhamento.Pecas, x => x.PecaId == pecaKit.Id && x.ValorTotal == 40m);

        var balanceamento = response.Servicos.Single(x => x.ServicoId == servicoBalanceamento.Id);
        Assert.Equal("Balanceamento", balanceamento.Descricao);
        Assert.Single(balanceamento.Pecas);
        Assert.Equal(120m, balanceamento.ValorTotal);
        Assert.Equal(40m, balanceamento.Pecas.Single().ValorTotal);
    }

    private static void DefinirPropriedade<T>(T instancia, string propriedade, object? valor)
        where T : class
    {
        var property = typeof(T).GetProperty(propriedade, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        Assert.NotNull(property);
        property!.SetValue(instancia, valor);
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

    private sealed class FakeItemServicoRepository : IItemServicoRepository
    {
        private readonly IReadOnlyCollection<ItemServico> _itens;

        public FakeItemServicoRepository(IReadOnlyCollection<ItemServico> itens)
        {
            _itens = itens;
        }

        public Task AddAsync(ItemServico entity, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task DeleteAsync(ItemServico entity, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task<IEnumerable<ItemServico>> GetAllAsync(CancellationToken cancellationToken = default) => Task.FromResult<IEnumerable<ItemServico>>(_itens);

        public Task<PagedResponse<ItemServico>> GetPagedAsync(Pagination pagination, CancellationToken cancellationToken = default) => Task.FromResult(new PagedResponse<ItemServico>());

        public Task<ItemServico?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default, bool tracking = false) => Task.FromResult(_itens.FirstOrDefault(x => x.Id == id));

        public Task UpdateAsync(ItemServico entity, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task HardDeleteAsync(ItemServico entity, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task<IReadOnlyCollection<ItemServico>> GetByOrdemServicoIdAndServicoIdAsync(Guid ordemServicoId, Guid servicoId, CancellationToken cancellationToken = default, bool tracking = false, bool includeRelacionados = false)
            => throw new NotImplementedException();

        public Task<ItemServico?> GetByOrdemServicoIdAndServicoIdAndPecaIdAsync(Guid ordemServicoId, Guid servicoId, Guid pecaId, CancellationToken cancellationToken = default, bool tracking = false)
            => throw new NotImplementedException();

        public Task<IReadOnlyCollection<ItemServico>> GetByOrdemServicoIdAsync(Guid ordemServicoId, CancellationToken cancellationToken = default, bool includeRelacionados = false, bool tracking = false)
            => throw new NotImplementedException();

        public Task<ItemServico> AddAsync(Guid ordemServicoId, Guid servicoId, Guid pecaId, int quantidade, CancellationToken cancellationToken = default)
            => throw new NotImplementedException();

        public Task<IReadOnlyCollection<ItemServico>> GetByOrcamentoIdAsync(Guid orcamentoId, CancellationToken cancellationToken = default, bool includeRelacionados = false, bool tracking = false)
            => Task.FromResult(_itens.Where(x => x.OrcamentoId == orcamentoId).ToList().AsReadOnly() as IReadOnlyCollection<ItemServico>);

        public Task<ItemServico?> GetByOrcamentoIdAndItemServicoIdAsync(Guid orcamentoId, Guid itemServicoId, CancellationToken cancellationToken = default, bool includeRelacionados = false, bool tracking = false)
            => throw new NotImplementedException();

        public Task<ItemServico?> GetByOrcamentoServicoPecaIdAsync(Guid orcamentoId, Guid servicoId, Guid? pecaId, CancellationToken cancellationToken = default, bool tracking = false)
            => throw new NotImplementedException();

        public Task<ItemServico?> GetByOrdemServicoIdAndItemServicoIdAsync(Guid ordemServicoId, Guid itemServicoId, CancellationToken cancellationToken = default, bool tracking = false, bool includeRelacionados = false)
            => throw new NotImplementedException();
    }
}
