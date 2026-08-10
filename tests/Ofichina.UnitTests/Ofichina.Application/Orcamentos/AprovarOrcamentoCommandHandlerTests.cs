using Microsoft.Extensions.Logging.Abstractions;
using Ofichina.Application.Abstractions.Authentication.Service;
using Ofichina.Application.Abstractions.Interfaces.Repository;
using Ofichina.Application.Abstractions.Interfaces.Service;
using Ofichina.Application.UseCases.Orcamentos.Commands;
using Ofichina.Application.UseCases.Orcamentos.Handlers;
using Ofichina.Contracts;
using Ofichina.Contracts.Common;
using Ofichina.Domain.Aggregates;
using Ofichina.Domain.Entities;
using Ofichina.Domain.Enums;

namespace Ofichina.UnitTests.Application.Orcamentos;

public sealed class AprovarOrcamentoCommandHandlerTests
{
    [Fact]
    public async Task Deve_Criar_Ordem_De_Servico_Com_Status_Criado_E_Hodometro_Da_Aprovacao()
    {
        var orcamento = CriarOrcamentoAprovado();
        var orcamentoRepository = new FakeOrcamentoRepository(orcamento);
        var ordemServicoRepository = new FakeOrdemServicoRepository();
        var historicoRepository = new FakeHistoricoStatusRepository();
        var usuarioAtualService = new FakeUsuarioAtualService(Guid.NewGuid());
        var mecanicoDisponibilidadeService = new FakeMecanicoDisponibilidadeService(Guid.NewGuid());
        var unitOfWork = new FakeUnitOfWork();
        var handler = new AprovarOrcamentoCommandHandler(
            orcamentoRepository,
            mecanicoDisponibilidadeService,
            historicoRepository,
            usuarioAtualService,
            ordemServicoRepository,
            unitOfWork,
            NullLogger<AprovarOrcamentoCommandHandler>.Instance);

        var result = await handler.HandleAsync(new AprovarOrcamentoCommand(orcamento.Id, 78123));

        Assert.True(result.IsSuccess, result.Error);
        Assert.NotNull(ordemServicoRepository.OrdemServicoAdicionada);
        Assert.Equal(StatusOrdemServico.Criado, ordemServicoRepository.OrdemServicoAdicionada!.Status);
        Assert.Equal(78123, ordemServicoRepository.OrdemServicoAdicionada.Hodometro);
        Assert.Equal(orcamento.PessoaId, ordemServicoRepository.OrdemServicoAdicionada.PessoaId);
        Assert.Equal(orcamento.VeiculoId, ordemServicoRepository.OrdemServicoAdicionada.VeiculoId);
        Assert.Equal(orcamento.ConsultorId, ordemServicoRepository.OrdemServicoAdicionada.ConsultorId);
        Assert.Equal(orcamento.Observacoes, ordemServicoRepository.OrdemServicoAdicionada.ProblemaRelatado);
        Assert.Equal(1, unitOfWork.SaveChangesCalls);
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

        DefinirAgendamento(orcamento, new Agendamento(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 55220, "Visita técnica"));

        orcamento.AdicionarServico(Guid.NewGuid(), null, 1, StatusOrcamento.Criado);

        orcamento.IniciarDiagnostico();
        orcamento.FinalizarDiagnostico();
        orcamento.EnviarParaCliente();

        return orcamento;
    }

    private static void DefinirAgendamento(Orcamento orcamento, Agendamento agendamento)
    {
        var field = typeof(Orcamento).GetField("<Agendamento>k__BackingField", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);

        Assert.NotNull(field);
        field!.SetValue(orcamento, agendamento);
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

    private sealed class FakeOrdemServicoRepository : IRepository<OrdemServico>
    {
        public OrdemServico? OrdemServicoAdicionada { get; private set; }

        public Task AddAsync(OrdemServico entity, CancellationToken cancellationToken = default)
        {
            OrdemServicoAdicionada = entity;
            return Task.CompletedTask;
        }

        public Task DeleteAsync(OrdemServico entity, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task<IEnumerable<OrdemServico>> GetAllAsync(CancellationToken cancellationToken = default) => Task.FromResult<IEnumerable<OrdemServico>>([]);

        public Task<PagedResponse<OrdemServico>> GetPagedAsync(Pagination pagination, CancellationToken cancellationToken = default) => Task.FromResult(new PagedResponse<OrdemServico>());

        public Task<OrdemServico?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default, bool tracking = false) => Task.FromResult<OrdemServico?>(null);

        public Task UpdateAsync(OrdemServico entity, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task HardDeleteAsync(OrdemServico entity, CancellationToken cancellationToken = default) => Task.CompletedTask;
    }

    private sealed class FakeHistoricoStatusRepository : IRepository<HistoricoStatus>
    {
        public Task AddAsync(HistoricoStatus entity, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task DeleteAsync(HistoricoStatus entity, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task<IEnumerable<HistoricoStatus>> GetAllAsync(CancellationToken cancellationToken = default) => Task.FromResult<IEnumerable<HistoricoStatus>>([]);

        public Task<PagedResponse<HistoricoStatus>> GetPagedAsync(Pagination pagination, CancellationToken cancellationToken = default) => Task.FromResult(new PagedResponse<HistoricoStatus>());

        public Task<HistoricoStatus?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default, bool tracking = false) => Task.FromResult<HistoricoStatus?>(null);

        public Task UpdateAsync(HistoricoStatus entity, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task HardDeleteAsync(HistoricoStatus entity, CancellationToken cancellationToken = default) => Task.CompletedTask;
    }

    private sealed class FakeUsuarioAtualService : IUserService
    {
        private readonly Guid? _usuarioId;

        public FakeUsuarioAtualService(Guid? usuarioId)
        {
            _usuarioId = usuarioId;
        }

        public Guid? ObterUsuarioId() => _usuarioId;
    }

    private sealed class FakeMecanicoDisponibilidadeService : IMecanicoDisponibilidadeService
    {
        private readonly Guid? _mecanicoId;

        public FakeMecanicoDisponibilidadeService(Guid? mecanicoId)
        {
            _mecanicoId = mecanicoId;
        }

        public Task<Guid?> ObterMecanicoDisponivelAsync(CancellationToken cancellationToken = default) => Task.FromResult(_mecanicoId);
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
