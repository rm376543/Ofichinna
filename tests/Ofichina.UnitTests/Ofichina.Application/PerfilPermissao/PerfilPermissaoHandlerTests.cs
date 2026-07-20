using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Ofichina.Application.UseCases.PerfilPermissoes.Commands;
using Ofichina.Application.UseCases.PerfilPermissoes.Handlers;
using Ofichina.Application.UseCases.PerfilPermissoes.Queries;
using Ofichina.Infrastructure.Repositories;
using Ofichina.Infrastructure.Persistence;

namespace Ofichina.UnitTests.Application.PerfilPermissao;

public sealed class PerfilPermissaoHandlerTests
{
    [Fact]
    public async Task VincularHandler_Deve_Criar_Vinculo()
    {
        await using var context = CreateContext();
        var perfil = new global::Ofichina.Domain.Entities.Perfil("ADMIN", "Administrador");
        var permissao = new global::Ofichina.Domain.Entities.Permissao("PERMISSAO_CADASTRAR", "Permite cadastrar registros");

        await context.Perfis.AddAsync(perfil);
        await context.Permissoes.AddAsync(permissao);
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        var perfilRepository = new PerfilRepository(context);
        var permissaoRepository = new PermissaoRepository(context);
        var vinculoRepository = new PerfilPermissaoRepository(context);
        var unitOfWork = new UnitOfWork(context);
        var handler = new VincularPermissaoPerfilCommandHandler(perfilRepository, permissaoRepository, vinculoRepository, unitOfWork, CreateLogger<VincularPermissaoPerfilCommandHandler>());

        var result = await handler.HandleAsync(new VincularPermissaoPerfilCommand(perfil.Id, permissao.Id));

        Assert.True(result.IsSuccess);
        Assert.Equal(1, await context.PerfisPermissoes.CountAsync());
    }

    [Fact]
    public async Task GetHandler_Deve_Listar_Permissoes_Do_Perfil()
    {
        await using var context = CreateContext();
        var perfil = new global::Ofichina.Domain.Entities.Perfil("ADMIN", "Administrador");
        var permissao = new global::Ofichina.Domain.Entities.Permissao("PERMISSAO_LISTAR", "Permite listar registros");
        await context.Perfis.AddAsync(perfil);
        await context.Permissoes.AddAsync(permissao);
        await context.PerfisPermissoes.AddAsync(new global::Ofichina.Domain.Entities.PerfilPermissao(perfil.Id, permissao.Id));
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        var perfilRepository = new PerfilRepository(context);
        var vinculoRepository = new PerfilPermissaoRepository(context);
        var handler = new GetPermissoesDoPerfilQueryHandler(perfilRepository, vinculoRepository, CreateLogger<GetPermissoesDoPerfilQueryHandler>());

        var result = await handler.HandleAsync(new GetPermissoesDoPerfilQuery(perfil.Id));

        Assert.True(result.IsSuccess);
        Assert.Single(result.Value!);
        Assert.Equal("PERMISSAO_LISTAR", result.Value!.First().Codigo);
    }

    [Fact]
    public async Task DesvincularHandler_Deve_Remover_Vinculo()
    {
        await using var context = CreateContext();
        var perfil = new global::Ofichina.Domain.Entities.Perfil("ADMIN", "Administrador");
        var permissao = new global::Ofichina.Domain.Entities.Permissao("PERMISSAO_REMOVER", "Permite remover registros");
        await context.Perfis.AddAsync(perfil);
        await context.Permissoes.AddAsync(permissao);
        await context.PerfisPermissoes.AddAsync(new global::Ofichina.Domain.Entities.PerfilPermissao(perfil.Id, permissao.Id));
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        var vinculoRepository = new PerfilPermissaoRepository(context);
        var unitOfWork = new UnitOfWork(context);
        var handler = new DesvincularPermissaoPerfilCommandHandler(vinculoRepository, unitOfWork, CreateLogger<DesvincularPermissaoPerfilCommandHandler>());

        var result = await handler.HandleAsync(new DesvincularPermissaoPerfilCommand(perfil.Id, permissao.Id));

        Assert.True(result.IsSuccess);
        Assert.Equal(0, await context.PerfisPermissoes.CountAsync());
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
