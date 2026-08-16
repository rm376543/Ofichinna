using Microsoft.EntityFrameworkCore;
using Ofichina.Domain.Aggregates;
using Ofichina.Domain.Entities;
using Ofichina.Domain.Enums;
using Ofichina.Domain.ValueObjects;
using Ofichina.Infrastructure.Persistence;
using Ofichina.Infrastructure.Services;

namespace Ofichina.UnitTests.Infrastructure.Services;

public sealed class InfrastructureServicesTests
{
    [Fact]
    public async Task PerfilAutorizacaoService_Deve_Retornar_Perfis_Permissoes_E_Validacoes()
    {
        var usuarioId = Guid.Parse("11111111-2222-3333-4444-555555555555");
        var perfil = new Perfil("ADMIN", "Administrador");
        var permissao = new Permissao("usuarios.listar", "Listar usuários");
        var perfilPermissao = new PerfilPermissao(perfil.Id, permissao.Id);
        var usuarioPerfil = new UsuarioPerfil(usuarioId, perfil.Id);

        var dbName = Guid.NewGuid().ToString();
        var context = CriarContexto(dbName);
        context.AddRange(perfil, permissao, perfilPermissao, usuarioPerfil);
        await context.SaveChangesAsync();

        await using var readContext = CriarContexto(dbName);
        var service = new PerfilAutorizacaoService(readContext);

        var perfis = await service.ObterPerfisAsync(usuarioId);
        var possuiPerfil = await service.PossuiPerfilAsync(usuarioId, " admin ");
        var permissoes = await service.ObterPermissoesAsync(usuarioId);
        var possuiPermissao = await service.PossuiPermissaoAsync(usuarioId, " usuarios.listar ");

        Assert.Equal(["ADMIN"], perfis);
        Assert.True(possuiPerfil);
        Assert.Equal(["usuarios.listar"], permissoes);
        Assert.True(possuiPermissao);
    }

    [Fact]
    public async Task MecanicoDisponibilidadeService_Deve_Retornar_Mecanico_Disponivel_Ou_Nulo()
    {
        var dbName = Guid.NewGuid().ToString();
        var usuarioId = Guid.Parse("11111111-2222-3333-4444-555555555555");
        var perfil = new Perfil("MECANICO", "Mecânico");
        var usuarioPerfil = new UsuarioPerfil(usuarioId, perfil.Id);
        var pessoa = new Pessoa("Mecânico 1", new Cpf("529.982.247-25"), new Telefone("11999999999"), new Endereco("Rua A", "1", null, "Centro", "São Paulo", "SP", new Cep("01001000")), usuarioId);

        var context = CriarContexto(dbName);
        context.AddRange(perfil, usuarioPerfil, pessoa);
        await context.SaveChangesAsync();

        await using var readContext = CriarContexto(dbName);
        var service = new MecanicoDisponibilidadeService(readContext);

        var disponivel = await service.ObterMecanicoDisponivelAsync();

        Assert.Equal(pessoa.Id, disponivel);

        var ordemServico = new OrdemServico(pessoa.Id, Guid.NewGuid(), Guid.NewGuid(), 0, "Problema", null);
        DefinirPropriedade(ordemServico, "MecanicoId", pessoa.Id);
        DefinirPropriedade(ordemServico, "Status", StatusOrdemServico.EmExecucao);
        readContext.OrdensServico.Add(ordemServico);
        await readContext.SaveChangesAsync();

        var mecanicoOcupado = await service.ObterMecanicoDisponivelAsync();

        Assert.Null(mecanicoOcupado);
    }

    [Fact]
    public async Task MecanicoDisponibilidadeService_Deve_Retornar_Nulo_Quando_Nao_Houver_Mecanico()
    {
        var dbName = Guid.NewGuid().ToString();
        var usuarioId = Guid.NewGuid();
        var perfil = new Perfil("CONSULTOR", "Consultor");
        var usuarioPerfil = new UsuarioPerfil(usuarioId, perfil.Id);
        var pessoa = new Pessoa("Pessoa 1", new Cpf("529.982.247-25"), new Telefone("11999999999"), new Endereco("Rua A", "1", null, "Centro", "São Paulo", "SP", new Cep("01001000")), usuarioId);

        var context = CriarContexto(dbName);
        context.AddRange(perfil, usuarioPerfil, pessoa);
        await context.SaveChangesAsync();

        await using var readContext = CriarContexto(dbName);
        var service = new MecanicoDisponibilidadeService(readContext);

        var disponivel = await service.ObterMecanicoDisponivelAsync();

        Assert.Null(disponivel);
    }

    private static ApplicationDbContext CriarContexto(string dbName)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;

        return new ApplicationDbContext(options);
    }

    private static void DefinirPropriedade<T>(T instancia, string propriedade, object? valor)
        where T : class
    {
        var property = typeof(T).GetProperty(propriedade, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
        Assert.NotNull(property);
        property!.SetValue(instancia, valor);
    }
}