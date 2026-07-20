using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Ofichina.Application.UseCases.Permissoes.Commands;
using Ofichina.Application.UseCases.Permissoes.Handlers;
using Ofichina.Application.UseCases.Permissoes.Queries;
using Ofichina.Infrastructure.Repositories;
using Ofichina.Infrastructure.Persistence;

namespace Ofichina.UnitTests.Application.Permissao;

public sealed class PermissaoHandlerTests
{
    [Fact]
    public async Task CreateHandler_Deve_Criar_Permissao()
    {
        await using var context = CreateContext();
        var repository = new PermissaoRepository(context);
        var unitOfWork = new UnitOfWork(context);
        var handler = new CreatePermissaoCommandHandler(repository, unitOfWork, CreateLogger<CreatePermissaoCommandHandler>());

        var result = await handler.HandleAsync(new CreatePermissaoCommand("PERMISSAO_CADASTRAR", "Permite cadastrar registros"));

        Assert.True(result.IsSuccess);
        Assert.NotEqual(Guid.Empty, result.Value);
        Assert.Equal(1, await context.Permissoes.CountAsync());
    }

    [Fact]
    public async Task UpdateHandler_Deve_Atualizar_Permissao()
    {
        await using var context = CreateContext();
        var permissao = new global::Ofichina.Domain.Entities.Permissao("PERMISSAO_INICIAL", "Descrição inicial");
        await context.Permissoes.AddAsync(permissao);
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        var repository = new PermissaoRepository(context);
        var unitOfWork = new UnitOfWork(context);
        var handler = new UpdatePermissaoCommandHandler(repository, unitOfWork, CreateLogger<UpdatePermissaoCommandHandler>());

        var result = await handler.HandleAsync(new UpdatePermissaoCommand(permissao.Id, "PERMISSAO_ATUALIZADA", "Descrição atualizada"));

        Assert.True(result.IsSuccess);
        var atualizada = await context.Permissoes.SingleAsync(x => x.Id == permissao.Id);
        Assert.Equal("PERMISSAO_ATUALIZADA", atualizada.Codigo);
        Assert.Equal("Descrição atualizada", atualizada.Descricao);
    }

    [Fact]
    public async Task DeleteHandler_Deve_Realizar_SoftDelete()
    {
        await using var context = CreateContext();
        var permissao = new global::Ofichina.Domain.Entities.Permissao("PERMISSAO_REMOVER", "Descrição");
        await context.Permissoes.AddAsync(permissao);
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        var repository = new PermissaoRepository(context);
        var unitOfWork = new UnitOfWork(context);
        var handler = new DeletePermissaoCommandHandler(repository, unitOfWork, CreateLogger<DeletePermissaoCommandHandler>());

        var result = await handler.HandleAsync(new DeletePermissaoCommand(permissao.Id));

        Assert.True(result.IsSuccess);
        var excluida = await context.Permissoes.IgnoreQueryFilters().SingleAsync(x => x.Id == permissao.Id);
        Assert.NotNull(excluida.DeletedAt);
    }

    [Fact]
    public async Task GetHandlers_Devem_Retornar_Permissao()
    {
        await using var context = CreateContext();
        var permissao = new global::Ofichina.Domain.Entities.Permissao("PERMISSAO_LISTAR", "Descrição");
        await context.Permissoes.AddAsync(permissao);
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        var repository = new PermissaoRepository(context);
        var listHandler = new GetPermissoesQueryHandler(repository, CreateLogger<GetPermissoesQueryHandler>());
        var itemHandler = new GetPermissaoByIdQueryHandler(repository, CreateLogger<GetPermissaoByIdQueryHandler>());

        var listResult = await listHandler.HandleAsync(new GetPermissoesQuery());
        var itemResult = await itemHandler.HandleAsync(new GetPermissaoByIdQuery(permissao.Id));

        Assert.True(listResult.IsSuccess);
        Assert.Single(listResult.Value!);
        Assert.True(itemResult.IsSuccess);
        Assert.Equal(permissao.Id, itemResult.Value!.Id);
    }

    private static ApplicationDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options);
    }

    private static ILogger<T> CreateLogger<T>()
        => LoggerFactory.Create(builder => { }).CreateLogger<T>();
}
