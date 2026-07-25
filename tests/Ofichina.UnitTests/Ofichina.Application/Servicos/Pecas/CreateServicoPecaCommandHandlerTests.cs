using Microsoft.Extensions.Logging.Abstractions;
using Ofichina.Application.UseCases.ServicosPecas.Commands;
using Ofichina.Application.UseCases.ServicosPecas.Handlers;
using Ofichina.Contracts;
using Ofichina.Contracts.Common;
using Ofichina.Domain.Entities;

namespace Ofichina.UnitTests.Application.Servicos.Pecas;

public sealed class CreateServicoPecaCommandHandlerTests
{
    [Fact]
    public async Task Deve_Adicionar_Peca_Ao_Servico()
    {
        var servico = new Servico("Troca de óleo", null, 120m);
        var peca = new Peca("Filtro de óleo", null, "FILTRO-001", 35m, 10);
        var handler = CriarHandler(servico, peca, out var servicoPecasRepository, out var unitOfWork);

        var result = await handler.HandleAsync(new CreateServicoPecaCommand
        {
            ServicoId = servico.Id,
            PecaId = peca.Id,
            Quantidade = 2
        });

        Assert.True(result.IsSuccess);
        Assert.NotEqual(Guid.Empty, result.Value);
        Assert.Single(servicoPecasRepository.Itens);
        Assert.Equal(2, servicoPecasRepository.Itens.Single().Quantidade);
        Assert.Equal(1, unitOfWork.SaveChangesCalls);
    }

    [Fact]
    public async Task Deve_Retornar_Erro_Quando_Servico_Nao_For_Encontrado()
    {
        var servico = new Servico("Troca de óleo", null, 120m);
        var peca = new Peca("Filtro de óleo", null, "FILTRO-001", 35m, 10);
        var handler = CriarHandler(servico, peca, out var servicoPecasRepository, out var unitOfWork);

        var result = await handler.HandleAsync(new CreateServicoPecaCommand
        {
            ServicoId = Guid.NewGuid(),
            PecaId = peca.Id,
            Quantidade = 1
        });

        Assert.False(result.IsSuccess);
        Assert.Equal("Serviço não encontrado.", result.Error);
        Assert.Equal(0, unitOfWork.SaveChangesCalls);
        Assert.Empty(servicoPecasRepository.Itens);
    }

    [Fact]
    public async Task Deve_Retornar_Erro_Quando_Peca_Nao_For_Encontrada()
    {
        var servico = new Servico("Troca de óleo", null, 120m);
        var peca = new Peca("Filtro de óleo", null, "FILTRO-001", 35m, 10);
        var handler = CriarHandler(servico, peca, out var servicoPecasRepository, out var unitOfWork);

        var result = await handler.HandleAsync(new CreateServicoPecaCommand
        {
            ServicoId = servico.Id,
            PecaId = Guid.NewGuid(),
            Quantidade = 1
        });

        Assert.False(result.IsSuccess);
        Assert.Equal("Peça não encontrada.", result.Error);
        Assert.Equal(0, unitOfWork.SaveChangesCalls);
        Assert.Empty(servicoPecasRepository.Itens);
    }

    [Fact]
    public async Task Deve_Retornar_Erro_Quando_Peca_Ja_Estiver_Vinculada_Ao_Servico()
    {
        var servico = new Servico("Troca de óleo", null, 120m);
        var peca = new Peca("Filtro de óleo", null, "FILTRO-001", 35m, 10);

        var handler = CriarHandler(servico, peca, out var servicoPecasRepository, out var unitOfWork);

        await servicoPecasRepository.AdicionarAsync(servico.Id, peca.Id, 1);

        var result = await handler.HandleAsync(new CreateServicoPecaCommand
        {
            ServicoId = servico.Id,
            PecaId = peca.Id,
            Quantidade = 3
        });

        Assert.False(result.IsSuccess);
        Assert.Equal("A peça já foi adicionada ao serviço.", result.Error);
        Assert.Equal(0, unitOfWork.SaveChangesCalls);
        Assert.Single(servicoPecasRepository.Itens);
    }

    private static CreateServicoPecaCommandHandler CriarHandler(
        Servico servico,
        Peca peca,
        out FakeServicoPecasRepository servicoPecasRepository,
        out FakeUnitOfWork unitOfWork)
    {
        var servicoRepository = new FakeServicoRepository(servico);
        var pecaRepository = new FakePecaRepository(peca);
        servicoPecasRepository = new FakeServicoPecasRepository();
        unitOfWork = new FakeUnitOfWork();
        var logger = NullLogger<CreateServicoPecaCommandHandler>.Instance;

        return new CreateServicoPecaCommandHandler(
            servicoRepository,
            pecaRepository,
            servicoPecasRepository,
            unitOfWork,
            logger);
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

        public Task<PagedResponse<Servico>> GetPagedAsync(Pagination pagination, CancellationToken cancellationToken = default)
            => Task.FromResult(new PagedResponse<Servico>
            {
                Items = [],
                PageNumber = 1,
                PageSize = pagination.PageSize,
                TotalCount = 0,
                TotalPages = 0,
                HasNextPage = false,
                HasPreviousPage = false
            });

        public Task<PagedResponse<Servico>> GetAllServicosPaginadosAsync(Pagination pagination, CancellationToken cancellationToken = default)
            => Task.FromResult(new PagedResponse<Servico>
            {
                Items = [],
                PageNumber = 1,
                PageSize = pagination.PageSize,
                TotalCount = 0,
                TotalPages = 0,
                HasNextPage = false,
                HasPreviousPage = false
            });

        public Task UpdateAsync(Servico entity, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task DeleteAsync(Servico entity, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task HardDeleteAsync(Servico entity, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task<Servico?> GetByIdAsync(Guid id, bool includePecas = false, CancellationToken cancellationToken = default, bool tracking = false)
            => Task.FromResult<Servico?>(id == _servico.Id ? _servico : null);

        public Task<IReadOnlyCollection<Servico>> GetAllAsync(bool includePecas = false, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyCollection<Servico>>([]);
    }

    private sealed class FakePecaRepository : IRepository<Peca>
    {
        private readonly Peca _peca;

        public FakePecaRepository(Peca peca)
        {
            _peca = peca;
        }

        public Task AddAsync(Peca entity, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task<Peca?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default, bool tracking = false)
            => Task.FromResult<Peca?>(id == _peca.Id ? _peca : null);

        public Task<IEnumerable<Peca>> GetAllAsync(CancellationToken cancellationToken = default)
            => Task.FromResult<IEnumerable<Peca>>([]);

        public Task<PagedResponse<Peca>> GetPagedAsync(Pagination pagination, CancellationToken cancellationToken = default)
            => Task.FromResult(new PagedResponse<Peca>
            {
                Items = [],
                PageNumber = 1,
                PageSize = pagination.PageSize,
                TotalCount = 0,
                TotalPages = 0,
                HasNextPage = false,
                HasPreviousPage = false
            });

        public Task UpdateAsync(Peca entity, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task DeleteAsync(Peca entity, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task HardDeleteAsync(Peca entity, CancellationToken cancellationToken = default) => Task.CompletedTask;
    }

    private sealed class FakeServicoPecasRepository : IServicoPecasRepository
    {
        public List<ServicoPeca> Itens { get; } = [];

        public Task AddAsync(ServicoPeca entity, CancellationToken cancellationToken = default)
        {
            Itens.Add(entity);
            return Task.CompletedTask;
        }

        public Task<ServicoPeca?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default, bool tracking = false)
            => Task.FromResult<ServicoPeca?>(Itens.FirstOrDefault(x => x.Id == id));

        public Task<IEnumerable<ServicoPeca>> GetAllAsync(CancellationToken cancellationToken = default)
            => Task.FromResult<IEnumerable<ServicoPeca>>(Itens);

        public Task<PagedResponse<ServicoPeca>> GetPagedAsync(Pagination pagination, CancellationToken cancellationToken = default)
            => Task.FromResult(new PagedResponse<ServicoPeca>
            {
                Items = Itens,
                PageNumber = 1,
                PageSize = pagination.PageSize,
                TotalCount = Itens.Count,
                TotalPages = Itens.Count == 0 ? 0 : (int)Math.Ceiling(Itens.Count / (double)pagination.PageSize),
                HasNextPage = false,
                HasPreviousPage = false
            });

        public Task UpdateAsync(ServicoPeca entity, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task DeleteAsync(ServicoPeca entity, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task HardDeleteAsync(ServicoPeca entity, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task<ServicoPeca?> GetByServicoIdAndPecaIdAsync(Guid servicoId, Guid pecaId, CancellationToken cancellationToken = default, bool tracking = false)
            => Task.FromResult<ServicoPeca?>(Itens.FirstOrDefault(x => x.ServicoId == servicoId && x.PecaId == pecaId));

        public Task<IReadOnlyCollection<ServicoPeca>> GetByServicoIdAsync(Guid servicoId, CancellationToken cancellationToken = default, bool includePeca = false, bool tracking = false)
            => Task.FromResult<IReadOnlyCollection<ServicoPeca>>(Itens.Where(x => x.ServicoId == servicoId).ToList());

        public async Task<ServicoPeca> AdicionarAsync(Guid servicoId, Guid pecaId, int quantidade, CancellationToken cancellationToken = default)
        {
            var existente = await GetByServicoIdAndPecaIdAsync(servicoId, pecaId, cancellationToken, tracking: true);

            if (existente is not null && !existente.EstaExcluida())
                throw new InvalidOperationException("A peça já foi adicionada ao serviço.");

            var pecaServico = ServicoPeca.Criar(servicoId, pecaId, quantidade);
            Itens.Add(pecaServico);
            return pecaServico;
        }
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