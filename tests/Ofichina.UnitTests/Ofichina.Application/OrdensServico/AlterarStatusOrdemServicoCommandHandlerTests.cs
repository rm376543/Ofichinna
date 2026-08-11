using Microsoft.Extensions.Logging.Abstractions;
using Ofichina.Application.Abstractions.Authentication.Service;
using Ofichina.Application.Abstractions.Interfaces.Repository;
using Ofichina.Application.UseCases.OrdensServico.Commands;
using Ofichina.Application.UseCases.OrdensServico.Handlers;
using Ofichina.Contracts;
using Ofichina.Contracts.Common;
using Ofichina.Domain.Aggregates;
using Ofichina.Domain.Entities;
using Ofichina.Domain.Enums;
using System.Reflection;

namespace Ofichina.UnitTests.Application.OrdensServico;

public sealed class AlterarStatusOrdemServicoCommandHandlerTests
{
    [Fact]
    public async Task Finalizar_Deve_Carregar_Itens_E_Concluir_A_Ordem_De_Servico()
    {
        var ordemServico = CriarOrdemServicoEmExecucaoComItem();
        var ordemServicoRepository = new FakeOrdemServicoRepository(ordemServico);
        var historicoStatusRepository = new FakeHistoricoStatusRepository();
        var usuarioAtualService = new FakeUsuarioAtualService(Guid.NewGuid());
        var unitOfWork = new FakeUnitOfWork();

        var handler = new AlterarStatusOrdemServicoCommandHandler(
            ordemServicoRepository,
            historicoStatusRepository,
            usuarioAtualService,
            unitOfWork,
            NullLogger<AlterarStatusOrdemServicoCommandHandler>.Instance);

        var result = await handler.HandleAsync(
            new AlterarStatusOrdemServicoCommand(ordemServico.Id, "Finalizada"));

        Assert.True(result.IsSuccess, result.Error);
        Assert.True(ordemServicoRepository.IncludeItensRecebido);
        Assert.Equal(Ofichina.Domain.Enums.StatusOrdemServico.Finalizada, ordemServico.Status);
        Assert.NotNull(ordemServico.DataFinalizacao);
        Assert.Equal(1, historicoStatusRepository.Atualizacoes);
        Assert.Equal(1, unitOfWork.SaveChangesCalls);
    }

    [Fact]
    public async Task Finalizar_Deve_Falhar_Quando_Nao_Houver_Itens_Ativos_Na_Ordem()
    {
        var ordemServico = CriarOrdemServicoEmExecucaoSemItens();
        var ordemServicoRepository = new FakeOrdemServicoRepository(ordemServico);
        var historicoStatusRepository = new FakeHistoricoStatusRepository();
        var usuarioAtualService = new FakeUsuarioAtualService(Guid.NewGuid());
        var unitOfWork = new FakeUnitOfWork();

        var handler = new AlterarStatusOrdemServicoCommandHandler(
            ordemServicoRepository,
            historicoStatusRepository,
            usuarioAtualService,
            unitOfWork,
            NullLogger<AlterarStatusOrdemServicoCommandHandler>.Instance);

        var result = await handler.HandleAsync(
            new AlterarStatusOrdemServicoCommand(ordemServico.Id, "Finalizada"));

        Assert.False(result.IsSuccess);
        Assert.Equal("A ordem de serviço precisa possuir itens cadastrados.", result.Error);
        Assert.True(ordemServicoRepository.IncludeItensRecebido);
        Assert.Equal(0, historicoStatusRepository.Atualizacoes);
        Assert.Equal(0, unitOfWork.SaveChangesCalls);
    }

    private static OrdemServico CriarOrdemServicoEmExecucaoComItem()
    {
        var ordemServico = CriarOrdemServicoEmExecucaoSemItens();
        AdicionarItemServico(ordemServico);
        return ordemServico;
    }

    private static OrdemServico CriarOrdemServicoEmExecucaoSemItens()
    {
        var orcamento = CriarOrcamentoAprovado();
        var ordemServico = OrdemServico.CriarAPartirDoOrcamento(orcamento, null, Guid.NewGuid(), 78123);

        ordemServico.IniciarExecucao();

        return ordemServico;
    }

    private static Orcamento CriarOrcamentoAprovado()
    {
        var orcamento = new Orcamento(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTime.UtcNow.AddDays(10),
            0,
            "Barulho na suspensão");

        orcamento.AdicionarServico(Guid.NewGuid(), null, 1, StatusOrcamento.Criado);
        orcamento.IniciarDiagnostico();
        orcamento.FinalizarDiagnostico();
        orcamento.EnviarParaCliente();
        orcamento.Aprovar();

        return orcamento;
    }

    private static void AdicionarItemServico(OrdemServico ordemServico)
    {
        var field = typeof(OrdemServico).GetField("_servicos", BindingFlags.Instance | BindingFlags.NonPublic);

        Assert.NotNull(field);

        var servicos = (List<ItemServico>)field!.GetValue(ordemServico)!;
        servicos.Add(CriarItemServico(ordemServico.Id));
    }

    private static ItemServico CriarItemServico(Guid ordemServicoId)
    {
        var constructor = typeof(ItemServico).GetConstructor(
            BindingFlags.Instance | BindingFlags.NonPublic,
            null,
            [typeof(Guid?), typeof(Guid?), typeof(Guid), typeof(Guid?), typeof(int)],
            null);

        Assert.NotNull(constructor);

        return (ItemServico)constructor!.Invoke([null, ordemServicoId, Guid.NewGuid(), null, 1]);
    }

    private sealed class FakeOrdemServicoRepository : IOrdemServicoRepository
    {
        private readonly OrdemServico? _ordemServico;

        public bool IncludeItensRecebido { get; private set; }

        public FakeOrdemServicoRepository(OrdemServico? ordemServico)
        {
            _ordemServico = ordemServico;
        }

        public Task AddAsync(OrdemServico entity, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task DeleteAsync(OrdemServico entity, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task<IEnumerable<OrdemServico>> GetAllAsync(CancellationToken cancellationToken = default) => Task.FromResult<IEnumerable<OrdemServico>>([]);

        public Task<IReadOnlyCollection<OrdemServico>> GetAllAsync(bool includeItens = false, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyCollection<OrdemServico>>([]);

        public Task<PagedResponse<OrdemServico>> GetPagedAsync(Pagination pagination, CancellationToken cancellationToken = default) => Task.FromResult(new PagedResponse<OrdemServico>());

        public Task<OrdemServico?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default, bool tracking = false)
            => Task.FromResult(_ordemServico is not null && _ordemServico.Id == id ? _ordemServico : null);

        public Task<OrdemServico?> GetByIdAsync(Guid id, bool includeItens = false, CancellationToken cancellationToken = default, bool tracking = false)
        {
            IncludeItensRecebido = includeItens;
            return Task.FromResult(_ordemServico is not null && _ordemServico.Id == id ? _ordemServico : null);
        }

        public Task UpdateAsync(OrdemServico entity, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task HardDeleteAsync(OrdemServico entity, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task<OrdemServico?> GetByIdAsync(Guid id, bool includeItens = false, bool tracking = false, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }

    private sealed class FakeHistoricoStatusRepository : IRepository<HistoricoStatus>
    {
        public int Atualizacoes { get; private set; }

        public Task AddAsync(HistoricoStatus entity, CancellationToken cancellationToken = default)
        {
            Atualizacoes++;
            return Task.CompletedTask;
        }

        public Task DeleteAsync(HistoricoStatus entity, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task<IEnumerable<HistoricoStatus>> GetAllAsync(CancellationToken cancellationToken = default) => Task.FromResult<IEnumerable<HistoricoStatus>>([]);

        public Task<PagedResponse<HistoricoStatus>> GetPagedAsync(Pagination pagination, CancellationToken cancellationToken = default) => Task.FromResult(new PagedResponse<HistoricoStatus>());

        public Task<HistoricoStatus?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default, bool tracking = false) => Task.FromResult<HistoricoStatus?>(null);

        public Task UpdateAsync(HistoricoStatus entity, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task HardDeleteAsync(HistoricoStatus entity, CancellationToken cancellationToken = default) => Task.CompletedTask;
    }

    private sealed class FakeUsuarioAtualService : IUserService
    {
        private readonly Guid _usuarioId;

        public FakeUsuarioAtualService(Guid usuarioId)
        {
            _usuarioId = usuarioId;
        }

        public Guid? ObterUsuarioId() => _usuarioId;
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