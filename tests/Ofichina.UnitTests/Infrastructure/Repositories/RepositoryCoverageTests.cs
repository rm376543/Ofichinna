using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Ofichina.Application.Abstractions.Authentication.Repository;
using Ofichina.Application.Abstractions.Interfaces.Repository;
using Ofichina.Contracts.Common;
using Ofichina.Domain.Aggregates;
using Ofichina.Domain.Entities;
using Ofichina.Domain.Enums;
using Ofichina.Domain.ValueObjects;
using Ofichina.Infrastructure.Persistence;
using Ofichina.Infrastructure.Repositories;

namespace Ofichina.UnitTests.Infrastructure.Repositories;

public sealed class PessoaRepositoryTests
{
    [Fact]
    public async Task GetByUsuarioIdAsync_Deve_Retornar_Pessoa_Quando_Existir()
    {
        var dbName = Guid.NewGuid().ToString();
        var usuarioId = Guid.NewGuid();
        var pessoa = CriarPessoa(usuarioId);

        await using (var context = CriarContexto(dbName))
        {
            context.Pessoas.Add(pessoa);
            await context.SaveChangesAsync();
        }

        await using var readContext = CriarContexto(dbName);
        var repository = new PessoaRepository(readContext);

        var resultado = await repository.GetByUsuarioIdAsync(usuarioId);

        Assert.NotNull(resultado);
        Assert.Equal(pessoa.Id, resultado!.Id);
    }

    [Fact]
    public async Task GetByUsuarioIdAsync_Deve_Retornar_Nulo_Quando_Nao_Existir()
    {
        await using var context = CriarContexto(Guid.NewGuid().ToString());
        var repository = new PessoaRepository(context);

        var resultado = await repository.GetByUsuarioIdAsync(Guid.NewGuid());

        Assert.Null(resultado);
    }

    [Fact]
    public async Task GetByIdAsync_Deve_Incluir_Veiculos_Quando_Solicitado()
    {
        var dbName = Guid.NewGuid().ToString();
        var usuarioId = Guid.NewGuid();
        var pessoa = CriarPessoa(usuarioId);
        var veiculo = CriarVeiculo(pessoa.Id);

        await using (var context = CriarContexto(dbName))
        {
            context.Pessoas.Add(pessoa);
            context.Veiculos.Add(veiculo);
            await context.SaveChangesAsync();
        }

        await using var readContext = CriarContexto(dbName);
        var repository = new PessoaRepository(readContext);

        var semInclude = await repository.GetByIdAsync(pessoa.Id);
        var comInclude = await repository.GetByIdAsync(pessoa.Id, includeVeiculos: true);

        Assert.NotNull(semInclude);
        Assert.Empty(semInclude!.Veiculos);
        Assert.NotNull(comInclude);
        Assert.Single(comInclude!.Veiculos);
    }

    [Fact]
    public async Task GetByIdsAsync_Deve_Retornar_Colecao_Vazia_Quando_Ids_Invalidos()
    {
        await using var context = CriarContexto(Guid.NewGuid().ToString());
        var repository = new PessoaRepository(context);

        var resultado = await repository.GetByIdsAsync([Guid.Empty, Guid.Empty]);

        Assert.Empty(resultado);
    }

    [Fact]
    public async Task GetPagedAsync_Deve_Normalizar_Paginacao()
    {
        var dbName = Guid.NewGuid().ToString();
        var pessoas = new[]
        {
            CriarPessoa(Guid.NewGuid()),
            CriarPessoa(Guid.NewGuid())
        };

        await using (var context = CriarContexto(dbName))
        {
            context.Pessoas.AddRange(pessoas);
            await context.SaveChangesAsync();
        }

        await using var readContext = CriarContexto(dbName);
        var repository = new PessoaRepository(readContext);

        var resultado = await repository.GetPagedAsync(new Pagination(0, 0));

        Assert.Equal(2, resultado.TotalCount);
        Assert.Equal(1, resultado.PageNumber);
        Assert.Equal(10, resultado.PageSize);
        Assert.Equal(2, resultado.Items.Count);
    }

    private static ApplicationDbContext CriarContexto(string dbName)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;

        return new ApplicationDbContext(options);
    }

    private static Pessoa CriarPessoa(Guid usuarioId)
    {
        return new Pessoa(
            "Pessoa Teste",
            new Cpf("529.982.247-25"),
            new Telefone("11999999999"),
            new Endereco("Rua A", "1", null, "Centro", "São Paulo", "SP", new Cep("01001000")),
            usuarioId);
    }

    private static Veiculo CriarVeiculo(Guid pessoaId)
    {
        return new Veiculo(
            pessoaId,
            new Placa("ABC1234"),
            "Toyota",
            "Corolla",
            2023,
            "Preto",
            new Hodometro(1000));
    }
}

public sealed class VeiculoRepositoryTests
{
    [Fact]
    public async Task GetByIdAsync_Deve_Incluir_Pessoa_Quando_Solicitado()
    {
        var dbName = Guid.NewGuid().ToString();
        var pessoa = CriarPessoa(Guid.NewGuid());
        var veiculo = CriarVeiculo(pessoa.Id);

        await using (var context = CriarContexto(dbName))
        {
            context.Pessoas.Add(pessoa);
            context.Veiculos.Add(veiculo);
            await context.SaveChangesAsync();
        }

        await using var readContext = CriarContexto(dbName);
        var repository = new VeiculoRepository(readContext);

        var semInclude = await repository.GetByIdAsync(veiculo.Id);
        var comInclude = await repository.GetByIdAsync(veiculo.Id, includePessoa: true);

        Assert.NotNull(semInclude);
        Assert.Null(semInclude!.Pessoa);
        Assert.NotNull(comInclude);
        Assert.NotNull(comInclude!.Pessoa);
        Assert.Equal(pessoa.Id, comInclude.Pessoa.Id);
    }

    [Fact]
    public async Task GetByIdAsync_Deve_Respeitar_Tracking_Em_PessoaComInclude()
    {
        var dbName = Guid.NewGuid().ToString();
        var usuarioId = Guid.NewGuid();
        var pessoa = CriarPessoa(usuarioId);
        var veiculo = CriarVeiculo(pessoa.Id);

        await using (var context = CriarContexto(dbName))
        {
            context.Pessoas.Add(pessoa);
            context.Veiculos.Add(veiculo);
            await context.SaveChangesAsync();
        }

        await using var readContext = CriarContexto(dbName);
        var repository = new PessoaRepository(readContext);

        var resultado = await repository.GetByIdAsync(pessoa.Id, includeVeiculos: true);
        var tracked = await readContext.Pessoas.FirstAsync(x => x.Id == pessoa.Id);

        Assert.NotNull(resultado);
        Assert.NotNull(tracked);
    }

    [Fact]
    public async Task GetAllAsync_Deve_Incluir_Pessoa_Quando_Solicitado()
    {
        var dbName = Guid.NewGuid().ToString();
        var pessoa = CriarPessoa(Guid.NewGuid());
        var veiculo = CriarVeiculo(pessoa.Id);

        await using (var context = CriarContexto(dbName))
        {
            context.Pessoas.Add(pessoa);
            context.Veiculos.Add(veiculo);
            await context.SaveChangesAsync();
        }

        await using var readContext = CriarContexto(dbName);
        var repository = new VeiculoRepository(readContext);

        var semInclude = await repository.GetAllAsync();
        var comInclude = await repository.GetAllAsync(includePessoa: true);

        Assert.Single(semInclude);
        Assert.Null(semInclude.First().Pessoa);
        Assert.Single(comInclude);
        Assert.NotNull(comInclude.First().Pessoa);
    }

    [Fact]
    public async Task GetAllVeiculosByPessoaIdAsync_Deve_Filtrar_Excluidos()
    {
        var dbName = Guid.NewGuid().ToString();
        var pessoa = CriarPessoa(Guid.NewGuid());
        var ativo = CriarVeiculo(pessoa.Id);
        var excluido = CriarVeiculo(pessoa.Id);
        excluido.Desativar();

        await using (var context = CriarContexto(dbName))
        {
            context.Pessoas.Add(pessoa);
            context.Veiculos.AddRange(ativo, excluido);
            await context.SaveChangesAsync();
        }

        await using var readContext = CriarContexto(dbName);
        var repository = new VeiculoRepository(readContext);

        var resultado = await repository.GetAllVeiculosByPessoaIdAsync(pessoa.Id);

        Assert.Single(resultado);
        Assert.Equal(ativo.Id, resultado.First().Id);
    }

    [Fact]
    public async Task GetPagedAsync_Deve_Normalizar_Paginacao()
    {
        var dbName = Guid.NewGuid().ToString();
        var pessoa = CriarPessoa(Guid.NewGuid());
        var veiculos = new[] { CriarVeiculo(pessoa.Id), CriarVeiculo(pessoa.Id) };

        await using (var context = CriarContexto(dbName))
        {
            context.Pessoas.Add(pessoa);
            context.Veiculos.AddRange(veiculos);
            await context.SaveChangesAsync();
        }

        await using var readContext = CriarContexto(dbName);
        var repository = new VeiculoRepository(readContext);

        var resultado = await repository.GetPagedAsync(new Pagination(0, 0));

        Assert.Equal(2, resultado.TotalCount);
        Assert.Equal(1, resultado.PageNumber);
        Assert.Equal(10, resultado.PageSize);
        Assert.Equal(2, resultado.Items.Count);
    }

    [Fact]
    public async Task GetByIdAsync_Deve_Respeitar_Tracking_Em_ServicoComTracking()
    {
        var dbName = Guid.NewGuid().ToString();
        var servico = new Servico("Serviço 1", null, 10m);

        await using (var context = CriarContexto(dbName))
        {
            context.Servicos.Add(servico);
            await context.SaveChangesAsync();
        }

        await using var readContext = CriarContexto(dbName);
        var repository = new ServicoRepository(readContext);

        var resultado = await repository.GetByIdAsync(servico.Id, tracking: true);

        Assert.NotNull(resultado);
        Assert.Equal(servico.Id, resultado!.Id);
    }

    private static ApplicationDbContext CriarContexto(string dbName)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;

        return new ApplicationDbContext(options);
    }

    private static Pessoa CriarPessoa(Guid usuarioId)
        => new("Pessoa Teste", new Cpf("529.982.247-25"), new Telefone("11999999999"), new Endereco("Rua A", "1", null, "Centro", "São Paulo", "SP", new Cep("01001000")), usuarioId);

    private static Veiculo CriarVeiculo(Guid pessoaId)
        => new(pessoaId, new Placa("ABC1234"), "Toyota", "Corolla", 2023, "Preto", new Hodometro(1000));
}

public sealed class UsuarioAutenticacaoRepositoryTests
{
    [Fact]
    public async Task ObterPorEmailAsync_Deve_Retornar_Usuario_Com_Perfis()
    {
        var dbName = Guid.NewGuid().ToString();
        var usuario = new Usuario(new Email("admin@ofichinna.com"), "hash-da-senha");
        var perfil = new Perfil("ADMIN", "Perfil administrativo");
        var vinculo = new UsuarioPerfil(usuario.Id, perfil.Id);

        await using (var context = CriarContexto(dbName))
        {
            context.Perfis.Add(perfil);
            context.Usuarios.Add(usuario);
            context.UsuariosPerfis.Add(vinculo);
            await context.SaveChangesAsync();
        }

        await using var readContext = CriarContexto(dbName);
        var repository = new UsuarioAutenticacaoRepository(readContext);

        var resultado = await repository.ObterPorEmailAsync("ADMIN@OFICHINNA.COM ");

        Assert.NotNull(resultado);
        Assert.Equal("admin@ofichinna.com", resultado!.Email.Value);
        Assert.Single(resultado.Perfis);
        Assert.Equal("ADMIN", resultado.Perfis.First().Perfil.NomePerfil);
    }

    [Fact]
    public async Task ObterPorEmailAsync_Deve_Retornar_Nulo_Quando_Nao_Existir()
    {
        await using var context = CriarContexto(Guid.NewGuid().ToString());
        var repository = new UsuarioAutenticacaoRepository(context);

        var resultado = await repository.ObterPorEmailAsync("naoexiste@ofichinna.com");

        Assert.Null(resultado);
    }

    private static ApplicationDbContext CriarContexto(string dbName)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;

        return new ApplicationDbContext(options);
    }
}

public sealed class PerfilRepositoryTests
{
    [Fact]
    public async Task GetByNomeAsync_Deve_Normalizar_Caso_E_Espacos()
    {
        var dbName = Guid.NewGuid().ToString();
        var perfil = new Perfil("ADMIN", "Perfil administrativo");

        await using (var context = CriarContexto(dbName))
        {
            context.Perfis.Add(perfil);
            await context.SaveChangesAsync();
        }

        await using var readContext = CriarContexto(dbName);
        var repository = new PerfilRepository(readContext);

        var encontrado = await repository.GetByNomeAsync(" admin ");
        var ausente = await repository.GetByNomeAsync("mecanico");

        Assert.NotNull(encontrado);
        Assert.Equal(perfil.Id, encontrado!.Id);
        Assert.Null(ausente);
    }

    [Fact]
    public async Task GetAllAtivosAsync_Deve_Ignorar_Perfil_Desativado()
    {
        var dbName = Guid.NewGuid().ToString();
        var ativo = new Perfil("ADMIN", "Perfil administrativo");
        var desativado = new Perfil("MECANICO", "Perfil mecânico");
        desativado.Desativar();

        await using (var context = CriarContexto(dbName))
        {
            context.Perfis.AddRange(ativo, desativado);
            await context.SaveChangesAsync();
        }

        await using var readContext = CriarContexto(dbName);
        var repository = new PerfilRepository(readContext);

        var resultado = await repository.GetAllAtivosAsync();

        Assert.Single(resultado);
        Assert.Equal(ativo.Id, resultado.First().Id);
    }

    private static ApplicationDbContext CriarContexto(string dbName)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;

        return new ApplicationDbContext(options);
    }
}

public sealed class PerfilUsuarioRepositoryTests
{
    [Fact]
    public async Task ExisteAsync_Deve_Retornar_True_Quando_Vinculo_Existir()
    {
        var dbName = Guid.NewGuid().ToString();
        var usuarioId = Guid.NewGuid();
        var perfil = new Perfil("ADMIN", "Perfil administrativo");
        var vinculo = new UsuarioPerfil(usuarioId, perfil.Id);

        await using (var context = CriarContexto(dbName))
        {
            context.Perfis.Add(perfil);
            context.UsuariosPerfis.Add(vinculo);
            await context.SaveChangesAsync();
        }

        await using var readContext = CriarContexto(dbName);
        var repository = new PerfilUsuarioRepository(readContext);

        var existe = await repository.ExisteAsync(usuarioId, perfil.Id);
        var naoExiste = await repository.ExisteAsync(Guid.NewGuid(), perfil.Id);

        Assert.True(existe);
        Assert.False(naoExiste);
    }

    [Fact]
    public async Task GetByUsuarioIdPerfilIdAsync_Deve_Retornar_Vinculo_Quando_Existir()
    {
        var dbName = Guid.NewGuid().ToString();
        var usuarioId = Guid.NewGuid();
        var perfil = new Perfil("ADMIN", "Perfil administrativo");
        var vinculo = new UsuarioPerfil(usuarioId, perfil.Id);

        await using (var context = CriarContexto(dbName))
        {
            context.Perfis.Add(perfil);
            context.UsuariosPerfis.Add(vinculo);
            await context.SaveChangesAsync();
        }

        await using var readContext = CriarContexto(dbName);
        var repository = new PerfilUsuarioRepository(readContext);

        var encontrado = await repository.GetByUsuarioIdPerfilIdAsync(usuarioId, perfil.Id);
        var ausente = await repository.GetByUsuarioIdPerfilIdAsync(Guid.NewGuid(), perfil.Id);

        Assert.NotNull(encontrado);
        Assert.Equal(vinculo.Id, encontrado!.Id);
        Assert.Null(ausente);
    }

    private static ApplicationDbContext CriarContexto(string dbName)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;

        return new ApplicationDbContext(options);
    }
}

public sealed class PerfilPermissaoRepositoryTests
{
    [Fact]
    public async Task GetByPerfilIdPermissaoIdAsync_Deve_Retornar_Vinculo_Quando_Existir()
    {
        var dbName = Guid.NewGuid().ToString();
        var perfil = new Perfil("ADMIN", "Perfil administrativo");
        var permissao = new Permissao("usuarios.listar", "Listar usuários");
        var vinculo = new PerfilPermissao(perfil.Id, permissao.Id);

        await using (var context = CriarContexto(dbName))
        {
            context.Perfis.Add(perfil);
            context.Permissoes.Add(permissao);
            context.PerfisPermissoes.Add(vinculo);
            await context.SaveChangesAsync();
        }

        await using var readContext = CriarContexto(dbName);
        var repository = new PerfilPermissaoRepository(readContext);

        var encontrado = await repository.GetByPerfilIdPermissaoIdAsync(perfil.Id, permissao.Id);
        var ausente = await repository.GetByPerfilIdPermissaoIdAsync(Guid.NewGuid(), permissao.Id);

        Assert.NotNull(encontrado);
        Assert.Equal(vinculo.Id, encontrado!.Id);
        Assert.Null(ausente);
    }

    [Fact]
    public async Task GetByPerfilIdAsync_Deve_Carregar_Permissao()
    {
        var dbName = Guid.NewGuid().ToString();
        var perfil = new Perfil("ADMIN", "Perfil administrativo");
        var permissao = new Permissao("usuarios.listar", "Listar usuários");
        var vinculo = new PerfilPermissao(perfil.Id, permissao.Id);

        await using (var context = CriarContexto(dbName))
        {
            context.Perfis.Add(perfil);
            context.Permissoes.Add(permissao);
            context.PerfisPermissoes.Add(vinculo);
            await context.SaveChangesAsync();
        }

        await using var readContext = CriarContexto(dbName);
        var repository = new PerfilPermissaoRepository(readContext);

        var resultado = await repository.GetByPerfilIdAsync(perfil.Id);

        Assert.Single(resultado);
        Assert.Equal(permissao.Id, resultado.First().Permissao.Id);
    }

    [Fact]
    public async Task GetAllPermissoesAssociadosDeUmPerfil_Deve_Normalizar_Paginacao()
    {
        var dbName = Guid.NewGuid().ToString();
        var perfil = new Perfil("ADMIN", "Perfil administrativo");
        var perm1 = new Permissao("usuarios.listar", "Listar usuários");
        var perm2 = new Permissao("usuarios.criar", "Criar usuários");
        var vinculos = new[] { new PerfilPermissao(perfil.Id, perm1.Id), new PerfilPermissao(perfil.Id, perm2.Id) };

        await using (var context = CriarContexto(dbName))
        {
            context.Perfis.Add(perfil);
            context.Permissoes.AddRange(perm1, perm2);
            context.PerfisPermissoes.AddRange(vinculos);
            await context.SaveChangesAsync();
        }

        await using var readContext = CriarContexto(dbName);
        var repository = new PerfilPermissaoRepository(readContext);

        var resultado = await repository.GetAllPermissoesAssociadosDeUmPerfil(perfil.Id, new Pagination(0, 0));

        Assert.Equal(2, resultado.TotalCount);
        Assert.Equal(1, resultado.PageNumber);
        Assert.Equal(10, resultado.PageSize);
        Assert.Equal(2, resultado.Items.Count);
    }

    private static ApplicationDbContext CriarContexto(string dbName)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;

        return new ApplicationDbContext(options);
    }
}

public sealed class PermissaoRepositoryTests
{
    [Fact]
    public async Task GetByCodigoAsync_Deve_Normalizar_Caso_E_Espacos()
    {
        var dbName = Guid.NewGuid().ToString();
        var permissao = new Permissao("usuarios.listar", "Listar usuários");

        await using (var context = CriarContexto(dbName))
        {
            context.Permissoes.Add(permissao);
            await context.SaveChangesAsync();
        }

        await using var readContext = CriarContexto(dbName);
        var repository = new PermissaoRepository(readContext);

        var encontrado = await repository.GetByCodigoAsync(" usuarios.listar ");
        var ausente = await repository.GetByCodigoAsync("outro.codigo");

        Assert.NotNull(encontrado);
        Assert.Equal(permissao.Id, encontrado!.Id);
        Assert.Null(ausente);
    }

    [Fact]
    public async Task GetPagedAsync_Deve_Normalizar_Paginacao()
    {
        var dbName = Guid.NewGuid().ToString();
        var permissoes = new[]
        {
            new Permissao("usuarios.listar", "Listar usuários"),
            new Permissao("usuarios.criar", "Criar usuários")
        };

        await using (var context = CriarContexto(dbName))
        {
            context.Permissoes.AddRange(permissoes);
            await context.SaveChangesAsync();
        }

        await using var readContext = CriarContexto(dbName);
        var repository = new PermissaoRepository(readContext);

        var resultado = await repository.GetPagedAsync(new Pagination(0, 0));

        Assert.Equal(2, resultado.TotalCount);
        Assert.Equal(1, resultado.PageNumber);
        Assert.Equal(10, resultado.PageSize);
        Assert.Equal(2, resultado.Items.Count);
    }

    private static ApplicationDbContext CriarContexto(string dbName)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;

        return new ApplicationDbContext(options);
    }
}
