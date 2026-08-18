using Microsoft.Extensions.Logging.Abstractions;
using Ofichina.Application.Abstractions.Interfaces.Repository;
using Ofichina.Application.UseCases.Agendamentos.Handlers;
using Ofichina.Application.UseCases.Agendamentos.Queries;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Extension;
using Ofichina.Domain.Aggregates;
using Ofichina.Domain.Entities;
using Ofichina.Domain.ValueObjects;

namespace Ofichina.UnitTests.Application.Agendamentos;

public sealed class GetAgendamentoByIdQueryHandlerTests
{
    [Fact]
    public async Task Deve_Retornar_Detalhe_A_Partir_Da_View_Quando_Existir()
    {
        var pessoa = CriarPessoa();
        var view = new VwAgendamentoPessoa
        {
            AgendamentosId = Guid.NewGuid(),
            PessoaId = pessoa.Id,
            Nome = pessoa.Nome,
            Documento = pessoa.Documento?.Numero ?? string.Empty,
            Telefone = pessoa.Telefone.ToString(),
            Placa = "ABC1D23",
            Marca = "Fiat",
            Modelo = "Uno",
            AnoFabricacao = 2020,
            Cor = "Branco",
            Hodometro = 45000,
            Consultor = "Maria Souza",
            DtAgendamento = new DateTime(2026, 8, 15, 0, 0, 0, DateTimeKind.Utc),
            HorarioAgendamento = new TimeOnly(14, 30),
            CreatedAt = new DateTime(2026, 8, 10, 0, 0, 0, DateTimeKind.Utc),
            UpdatedAt = new DateTime(2026, 8, 11, 0, 0, 0, DateTimeKind.Utc),
            DeletedAt = null
        };

        var pessoaRepository = new FakePessoaRepository(pessoa);
        var agendamentoRepository = new FakeAgendamentoRepository(view);
        var handler = new GetAgendamentoByIdQueryHandler(
            agendamentoRepository,
            pessoaRepository,
            NullLogger<GetAgendamentoByIdQueryHandler>.Instance);

        var result = await handler.HandleAsync(new GetAgendamentoByIdQuery(pessoa.Id, view.AgendamentosId));

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(view.AgendamentosId, result.Value!.AgendamentosId);
        Assert.Equal(view.PessoaId, result.Value.PessoaId);
        Assert.Equal(view.Nome, result.Value.Nome);
        Assert.Equal(view.Documento, result.Value.Documento);
        Assert.Equal(view.Telefone, result.Value.Telefone);
        Assert.Equal(view.Placa, result.Value.Placa);
        Assert.Equal(view.Marca, result.Value.Marca);
        Assert.Equal(view.Modelo, result.Value.Modelo);
        Assert.Equal(view.AnoFabricacao, result.Value.AnoFabricacao);
        Assert.Equal(view.Cor, result.Value.Cor);
        Assert.Equal(view.Hodometro, result.Value.Hodometro);
        Assert.Equal(view.Consultor, result.Value.Consultor);
        Assert.Equal(view.DtAgendamento.ToDateString(), result.Value.DtAgendamento);
        Assert.Equal(view.HorarioAgendamento, result.Value.HorarioAgendamento);
        Assert.Equal(view.CreatedAt.ToString("dd/MM/yyyy"), result.Value.CreatedAt);
        Assert.Equal(view.UpdatedAt?.ToString("dd/MM/yyyy"), result.Value.UpdatedAt);
        Assert.Equal(view.DeletedAt?.ToString("dd/MM/yyyy"), result.Value.DeletedAt);
    }

    [Fact]
    public async Task Deve_Retornar_Erro_Quando_Pessoa_Nao_Existir()
    {
        var agendamentoRepository = new FakeAgendamentoRepository(null);
        var pessoaRepository = new FakePessoaRepository(null);
        var handler = new GetAgendamentoByIdQueryHandler(
            agendamentoRepository,
            pessoaRepository,
            NullLogger<GetAgendamentoByIdQueryHandler>.Instance);

        var result = await handler.HandleAsync(new GetAgendamentoByIdQuery(Guid.NewGuid(), Guid.NewGuid()));

        Assert.False(result.IsSuccess);
        Assert.Equal("Pessoa não encontrada.", result.Error);
    }

    private static Pessoa CriarPessoa()
    {
        return new Pessoa(
            "João Silva",
            new Cpf("12345678909"),
            new Telefone("11987654321"),
            new Endereco("Rua Teste", "100", null, "Centro", "São Paulo", "SP", new Cep("01001000")),
            Guid.NewGuid());
    }

    private sealed class FakePessoaRepository : IPessoaRepository
    {
        private readonly Pessoa? _pessoa;

        public FakePessoaRepository(Pessoa? pessoa)
        {
            _pessoa = pessoa;
        }

        public Task AddAsync(Pessoa entity, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task<Pessoa?> GetByIdAsync(Guid pessoaId, CancellationToken cancellationToken = default, bool tracking = false)
            => Task.FromResult(_pessoa is not null && _pessoa.Id == pessoaId ? _pessoa : null);

        public Task<IEnumerable<Pessoa>> GetAllAsync(CancellationToken cancellationToken = default) => Task.FromResult<IEnumerable<Pessoa>>(_pessoa is null ? [] : [_pessoa]);

        public Task<PagedResponse<Pessoa>> GetPagedAsync(Pagination pagination, CancellationToken cancellationToken = default) => Task.FromResult(new PagedResponse<Pessoa>());

        public Task UpdateAsync(Pessoa entity, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task DeleteAsync(Pessoa entity, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task HardDeleteAsync(Pessoa entity, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task<Pessoa?> GetByUsuarioIdAsync(Guid usuarioId, CancellationToken cancellationToken = default) => Task.FromResult<Pessoa?>(null);

        public Task<IReadOnlyCollection<Pessoa>> GetByIdsAsync(IEnumerable<Guid> pessoaIds, CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyCollection<Pessoa>>([]);

        public Task<Pessoa?> GetByIdAsync(Guid pessoaId, bool includeVeiculos = false, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }

    private sealed class FakeAgendamentoRepository : IAgendamentoRepository
    {
        private readonly VwAgendamentoPessoa? _view;

        public FakeAgendamentoRepository(VwAgendamentoPessoa? view)
        {
            _view = view;
        }

        public Task AddAsync(Agendamento entity, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task DeleteAsync(Agendamento entity, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task<IEnumerable<Agendamento>> GetAllAsync(CancellationToken cancellationToken = default) => Task.FromResult<IEnumerable<Agendamento>>([]);

        public Task<PagedResponse<Agendamento>> GetPagedAsync(Pagination pagination, CancellationToken cancellationToken = default) => Task.FromResult(new PagedResponse<Agendamento>());

        public Task<Agendamento?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default, bool tracking = false) => Task.FromResult<Agendamento?>(null);

        public Task UpdateAsync(Agendamento entity, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task HardDeleteAsync(Agendamento entity, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task<PagedResponse<Agendamento>> GetPagedByClientePessoaAsync(Guid pessoaId, Pagination pagination, CancellationToken cancellationToken = default) => Task.FromResult(new PagedResponse<Agendamento>());

        public Task<IReadOnlyCollection<Agendamento>> GetAllWithIncludesAsync(CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyCollection<Agendamento>>([]);

        public Task<Agendamento?> GetByIdAndPessoaAsync(Guid agendamentoId, Guid pessoaId, CancellationToken cancellationToken = default) => Task.FromResult<Agendamento?>(null);

        public Task<bool> ExisteConflitoConsultorAsync(Guid horarioConsultorId, CancellationToken cancellationToken = default) => Task.FromResult(false);

        public Task<bool> ExisteConflitoVeiculoAsync(Guid veiculoId, Guid diaDisponibilidadeId, Guid horarioConsultorId, CancellationToken cancellationToken = default) => Task.FromResult(false);

        public Task<Agendamento?> BuscarAgendamentosPorPessoaId(Guid pessoaId, CancellationToken cancellationToken = default) => Task.FromResult<Agendamento?>(null);

        public Task<IReadOnlyCollection<VwAgendamentoPessoa>> GetAgendamentosUsuarioViewByPessoaAsync(Guid pessoaId, CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyCollection<VwAgendamentoPessoa>>(_view is null ? [] : [_view]);

        public Task<VwAgendamentoPessoa?> GetAgendamentoUsuarioViewByIdAsync(Guid pessoaId, Guid agendamentosId, CancellationToken cancellationToken = default)
            => Task.FromResult(_view is not null && _view.PessoaId == pessoaId && _view.AgendamentosId == agendamentosId ? _view : null);
    }
}
