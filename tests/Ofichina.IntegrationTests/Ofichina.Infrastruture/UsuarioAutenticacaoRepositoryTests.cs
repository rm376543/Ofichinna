using Ofichina.Domain.Entities;
using Ofichina.Domain.ValueObjects;
using Ofichina.Infrastructure.Repositories;

namespace Ofichina.IntegrationTests.Infrastructure;

public sealed class UsuarioAutenticacaoRepositoryTests : IClassFixture<SqlServerFixture>
{
    private readonly SqlServerFixture _fixture;

    public UsuarioAutenticacaoRepositoryTests(SqlServerFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task ObterPorEmailAsync_DeveRetornarUsuario_ComPerfis()
    {
        await using var context = _fixture.CreateDbContext();

        var usuarioId = Guid.Parse("550e8400-e29b-41d4-a716-446655440000");
        var perfilId = Guid.Parse("11111111-1111-1111-1111-111111111111");

        Email email = new Email("admin@ofichinna.com");
        var usuario = new Usuario(email, "hash-da-senha");

        var perfil = new Perfil("ADMIN", "Perfil administrativo");

        var vinculo = new UsuarioPerfil(usuario.Id, perfil.Id);

        context.Perfis.Add(perfil);
        context.Usuarios.Add(usuario);
        context.UsuariosPerfis.Add(vinculo);
        await context.SaveChangesAsync();

        var repository = new UsuarioAutenticacaoRepository(context);

        var result = await repository.ObterPorEmailAsync("ADMIN@OFICHINNA.COM ");

        Assert.NotNull(result);
        Assert.Equal(usuarioId, result!.Id);
        Assert.Equal("admin@ofichinna.com", result.Email.Value);
        Assert.Single(result.Perfis);
        Assert.Equal("ADMIN", result.Perfis.First().Perfil.NomePerfil);
    }
}