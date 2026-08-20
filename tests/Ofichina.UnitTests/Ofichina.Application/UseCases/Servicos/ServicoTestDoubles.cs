using Ofichina.Application.Abstractions.Common;
using Ofichina.Application.Abstractions.Interfaces.Repository;
using Ofichina.Contracts.Common;
using Ofichina.Domain.Entities;

namespace Ofichina.UnitTests.Application.UseCases.Servicos;

internal sealed class ServicoRepositoryTestDouble : IServicoRepository
{
    public Servico? ServicoPorId { get; set; }

    public PagedResponse<Servico> PagedResponse { get; set; } = new();

    public Pagination? UltimaPaginacao { get; private set; }

    public Servico? UltimoServicoAdicionado { get; private set; }

    public Servico? UltimoServicoAtualizado { get; private set; }

    public Servico? UltimoServicoRemovido { get; private set; }

    public bool ThrowOnGetById { get; set; }

    public bool ThrowOnGetPaged { get; set; }

    public Task AddAsync(Servico entity, CancellationToken cancellationToken = default)
    {
        UltimoServicoAdicionado = entity;
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Servico entity, CancellationToken cancellationToken = default)
    {
        UltimoServicoRemovido = entity;
        return Task.CompletedTask;
    }

    public Task<IEnumerable<Servico>> GetAllAsync(CancellationToken cancellationToken = default)
        => Task.FromResult<IEnumerable<Servico>>([]);

    public Task<IReadOnlyCollection<Servico>> GetAllAsync(bool includePecas = false, CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyCollection<Servico>>([]);

    public Task<Servico?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default, bool tracking = false)
    {
        if (ThrowOnGetById)
            throw new InvalidOperationException("Falha simulada ao obter serviço.");

        return Task.FromResult(ServicoPorId is not null && ServicoPorId.Id == id ? ServicoPorId : null);
    }

    public Task<Servico?> GetByIdAsync(Guid id, bool includePecas = false, CancellationToken cancellationToken = default, bool tracking = false)
    {
        if (ThrowOnGetById)
            throw new InvalidOperationException("Falha simulada ao obter serviço.");

        return Task.FromResult(ServicoPorId is not null && ServicoPorId.Id == id ? ServicoPorId : null);
    }

    public Task<PagedResponse<Servico>> GetPagedAsync(Pagination pagination, CancellationToken cancellationToken = default)
    {
        UltimaPaginacao = pagination;

        if (ThrowOnGetPaged)
            throw new InvalidOperationException("Falha simulada ao listar serviços.");

        return Task.FromResult(PagedResponse);
    }

    public Task UpdateAsync(Servico entity, CancellationToken cancellationToken = default)
    {
        UltimoServicoAtualizado = entity;
        return Task.CompletedTask;
    }

    public Task HardDeleteAsync(Servico entity, CancellationToken cancellationToken = default) => Task.CompletedTask;
}

internal sealed class TestUnitOfWork : IUnitOfWork
{
    public int SaveChangesCalls { get; private set; }

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;

    public Task BeginTransactionAsync() => Task.CompletedTask;

    public Task CommitTransactionAsync() => Task.CompletedTask;

    public Task RollbackTransactionAsync() => Task.CompletedTask;

    public Task<int> SaveChangesAsync()
    {
        SaveChangesCalls++;
        return Task.FromResult(1);
    }
}