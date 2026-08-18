using Microsoft.EntityFrameworkCore;
using Ofichina.Domain.Entities;
using Ofichina.Domain.ValueObjects;
using Ofichina.Infrastructure.Persistence;
using Ofichina.Infrastructure.Repositories;

namespace Ofichina.UnitTests.Infrastructure.Repositories;

public sealed class HorarioConsultorDisponibilidadeRepositoryTests
{
    [Fact]
    public void Constructor_ContextoValido_Deve_CriarRepositorio()
    {
        // Arrange
        using var context = CriarContexto(Guid.NewGuid().ToString());

        // Act
        var repository = new AgendaConsultorRepository(context);

        // Assert
        Assert.NotNull(repository);
    }

    [Fact]
    public async Task GetByIdWithConsultorAsync_AgendaAtivaEDesativada_Deve_RetornarSomenteAtivaComConsultor()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        var consultorAtivo = CriarPessoaConsultor("Consultor Ativo");
        var consultorExcluido = CriarPessoaConsultor("Consultor Excluido");
        var diaAtivo = new DiaDisponibilidade(new DateOnly(2026, 8, 15));
        var horarioAtivo = new HorarioDisponibilidade(new TimeOnly(9, 0));
        var agendaAtiva = new AgendaConsultor(diaAtivo.Id, horarioAtivo.Id, consultorAtivo.Id);

        var diaExcluido = new DiaDisponibilidade(new DateOnly(2026, 8, 16));
        var horarioExcluido = new HorarioDisponibilidade(new TimeOnly(10, 0));
        var agendaExcluida = new AgendaConsultor(diaExcluido.Id, horarioExcluido.Id, consultorExcluido.Id);
        agendaExcluida.Excluir();

        await using (var seedContext = CriarContexto(dbName))
        {
            seedContext.AddRange(
                consultorAtivo,
                consultorExcluido,
                diaAtivo,
                horarioAtivo,
                agendaAtiva,
                diaExcluido,
                horarioExcluido,
                agendaExcluida);

            await seedContext.SaveChangesAsync();
        }

        await using var readContext = CriarContexto(dbName);
        var repository = new AgendaConsultorRepository(readContext);

        // Act
        var resultadoAtivo = await repository.GetByIdWithConsultorAsync(agendaAtiva.Id);
        var resultadoExcluido = await repository.GetByIdWithConsultorAsync(agendaExcluida.Id);

        // Assert
        Assert.NotNull(resultadoAtivo);
        Assert.Equal(agendaAtiva.Id, resultadoAtivo!.Id);
        Assert.NotNull(resultadoAtivo.Consultor);
        Assert.Equal(consultorAtivo.Id, resultadoAtivo.Consultor.Id);
        Assert.Null(resultadoExcluido);
        Assert.Empty(readContext.ChangeTracker.Entries());
    }

    [Fact]
    public async Task GetByDiaHorarioConsultorAsync_AgendaAtivaEDesativada_Deve_Respeitar_FiltroDeExclusaoLogica()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        var consultorAtivo = CriarPessoaConsultor("Consultor Ativo");
        var consultorExcluido = CriarPessoaConsultor("Consultor Excluido");
        var diaAtivo = new DiaDisponibilidade(new DateOnly(2026, 8, 17));
        var horarioAtivo = new HorarioDisponibilidade(new TimeOnly(11, 0));
        var agendaAtiva = new AgendaConsultor(diaAtivo.Id, horarioAtivo.Id, consultorAtivo.Id);

        var diaExcluido = new DiaDisponibilidade(new DateOnly(2026, 8, 18));
        var horarioExcluido = new HorarioDisponibilidade(new TimeOnly(12, 0));
        var agendaExcluida = new AgendaConsultor(diaExcluido.Id, horarioExcluido.Id, consultorExcluido.Id);
        agendaExcluida.Excluir();

        await using (var seedContext = CriarContexto(dbName))
        {
            seedContext.AddRange(
                consultorAtivo,
                consultorExcluido,
                diaAtivo,
                horarioAtivo,
                agendaAtiva,
                diaExcluido,
                horarioExcluido,
                agendaExcluida);

            await seedContext.SaveChangesAsync();
        }

        await using var readContext = CriarContexto(dbName);
        var repository = new AgendaConsultorRepository(readContext);

        // Act
        var resultadoAtivo = await repository.GetByDiaHorarioConsultorAsync(diaAtivo.Id, horarioAtivo.Id, consultorAtivo.Id);
        var resultadoExcluido = await repository.GetByDiaHorarioConsultorAsync(diaExcluido.Id, horarioExcluido.Id, consultorExcluido.Id);

        // Assert
        Assert.NotNull(resultadoAtivo);
        Assert.Equal(agendaAtiva.Id, resultadoAtivo!.Id);
        Assert.Null(resultadoExcluido);
        Assert.Empty(readContext.ChangeTracker.Entries());
    }

    [Fact]
    public async Task GetByConsultorAndDiaAsync_ComHorariosDiversos_Deve_RetornarOrdenadoESemItensDeOutrosConsultoresOuExcluidos()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        var consultorPrincipal = CriarPessoaConsultor("Consultor Principal");
        var consultorSecundario = CriarPessoaConsultor("Consultor Secundario");
        var diaPrincipal = new DiaDisponibilidade(new DateOnly(2026, 8, 19));
        var diaOutro = new DiaDisponibilidade(new DateOnly(2026, 8, 20));

        var horarioManha = new HorarioDisponibilidade(new TimeOnly(8, 0));
        var horarioMeio = new HorarioDisponibilidade(new TimeOnly(11, 0));
        var horarioTarde = new HorarioDisponibilidade(new TimeOnly(14, 0));
        var horarioExcluido = new HorarioDisponibilidade(new TimeOnly(10, 0));
        var horarioOutroDia = new HorarioDisponibilidade(new TimeOnly(6, 0));
        var horarioOutroConsultor = new HorarioDisponibilidade(new TimeOnly(7, 0));

        var agendaManha = new AgendaConsultor(diaPrincipal.Id, horarioManha.Id, consultorPrincipal.Id);
        var agendaMeio = new AgendaConsultor(diaPrincipal.Id, horarioMeio.Id, consultorPrincipal.Id);
        var agendaTarde = new AgendaConsultor(diaPrincipal.Id, horarioTarde.Id, consultorPrincipal.Id);
        var agendaExcluida = new AgendaConsultor(diaPrincipal.Id, horarioExcluido.Id, consultorPrincipal.Id);
        agendaExcluida.Excluir();
        var agendaOutroDia = new AgendaConsultor(diaOutro.Id, horarioOutroDia.Id, consultorPrincipal.Id);
        var agendaOutroConsultor = new AgendaConsultor(diaPrincipal.Id, horarioOutroConsultor.Id, consultorSecundario.Id);

        await using (var seedContext = CriarContexto(dbName))
        {
            seedContext.AddRange(
                consultorPrincipal,
                consultorSecundario,
                diaPrincipal,
                diaOutro,
                horarioManha,
                horarioMeio,
                horarioTarde,
                horarioExcluido,
                horarioOutroDia,
                horarioOutroConsultor,
                agendaManha,
                agendaMeio,
                agendaTarde,
                agendaExcluida,
                agendaOutroDia,
                agendaOutroConsultor);

            await seedContext.SaveChangesAsync();
        }

        await using var readContext = CriarContexto(dbName);
        var repository = new AgendaConsultorRepository(readContext);

        // Act
        var resultado = await repository.GetByConsultorAndDiaAsync(consultorPrincipal.Id, diaPrincipal.Id);

        // Assert
        Assert.Equal(3, resultado.Count);
        Assert.Equal(
            new[] { horarioManha.Id, horarioMeio.Id, horarioTarde.Id },
            resultado.Select(x => x.HorarioDisponibilidadeId));
        Assert.All(resultado, item =>
        {
            Assert.Equal(consultorPrincipal.Id, item.ConsultorPessoaId);
            Assert.Equal(diaPrincipal.Id, item.DiaDisponibilidadeId);
        });
        Assert.Empty(readContext.ChangeTracker.Entries());
    }

    [Fact]
    public async Task GetConsultoresByDiaAndHorarioAsync_ComRegistrosVariados_Deve_RetornarSomenteCorrespondentesAtivos()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        var diaPrincipal = new DiaDisponibilidade(new DateOnly(2026, 8, 21));
        var diaOutro = new DiaDisponibilidade(new DateOnly(2026, 8, 22));
        var horarioPrincipal = new HorarioDisponibilidade(new TimeOnly(15, 0));
        var horarioOutro = new HorarioDisponibilidade(new TimeOnly(16, 0));

        var consultorUm = CriarPessoaConsultor("Consultor Um");
        var consultorDois = CriarPessoaConsultor("Consultor Dois");
        var consultorExcluido = CriarPessoaConsultor("Consultor Excluido");

        var agendaUm = new AgendaConsultor(diaPrincipal.Id, horarioPrincipal.Id, consultorUm.Id);
        var agendaDois = new AgendaConsultor(diaPrincipal.Id, horarioPrincipal.Id, consultorDois.Id);
        var agendaExcluida = new AgendaConsultor(diaPrincipal.Id, horarioPrincipal.Id, consultorExcluido.Id);
        agendaExcluida.Excluir();
        var agendaOutroHorario = new AgendaConsultor(diaPrincipal.Id, horarioOutro.Id, consultorUm.Id);
        var agendaOutroDia = new AgendaConsultor(diaOutro.Id, horarioPrincipal.Id, consultorDois.Id);

        await using (var seedContext = CriarContexto(dbName))
        {
            seedContext.AddRange(
                diaPrincipal,
                diaOutro,
                horarioPrincipal,
                horarioOutro,
                consultorUm,
                consultorDois,
                consultorExcluido,
                agendaUm,
                agendaDois,
                agendaExcluida,
                agendaOutroHorario,
                agendaOutroDia);

            await seedContext.SaveChangesAsync();
        }

        await using var readContext = CriarContexto(dbName);
        var repository = new AgendaConsultorRepository(readContext);

        // Act
        var resultado = await repository.GetConsultoresByDiaAndHorarioAsync(diaPrincipal.Id, horarioPrincipal.Id);

        // Assert
        Assert.Equal(2, resultado.Count);
        Assert.Contains(resultado, item => item.ConsultorPessoaId == consultorUm.Id);
        Assert.Contains(resultado, item => item.ConsultorPessoaId == consultorDois.Id);
        Assert.DoesNotContain(resultado, item => item.ConsultorPessoaId == consultorExcluido.Id);
        Assert.DoesNotContain(resultado, item => item.DiaDisponibilidadeId == diaOutro.Id);
        Assert.DoesNotContain(resultado, item => item.HorarioDisponibilidadeId == horarioOutro.Id);
        Assert.Empty(readContext.ChangeTracker.Entries());
    }

    [Fact]
    public async Task GetByDiaAsync_ComRegistrosDiversos_Deve_RetornarOrdenadoESemExcluidos()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        var consultorUm = CriarPessoaConsultor("Consultor Um");
        var consultorDois = CriarPessoaConsultor("Consultor Dois");
        var diaPrincipal = new DiaDisponibilidade(new DateOnly(2026, 8, 23));
        var diaOutro = new DiaDisponibilidade(new DateOnly(2026, 8, 24));

        var horarioTarde = new HorarioDisponibilidade(new TimeOnly(14, 0));
        var horarioManha = new HorarioDisponibilidade(new TimeOnly(8, 0));
        var horarioNoite = new HorarioDisponibilidade(new TimeOnly(18, 0));
        var horarioOutroDia = new HorarioDisponibilidade(new TimeOnly(6, 0));

        var agendaTarde = new AgendaConsultor(diaPrincipal.Id, horarioTarde.Id, consultorUm.Id);
        var agendaManha = new AgendaConsultor(diaPrincipal.Id, horarioManha.Id, consultorDois.Id);
        var agendaNoiteExcluida = new AgendaConsultor(diaPrincipal.Id, horarioNoite.Id, consultorUm.Id);
        agendaNoiteExcluida.Excluir();
        var agendaOutroDia = new AgendaConsultor(diaOutro.Id, horarioOutroDia.Id, consultorDois.Id);

        await using (var seedContext = CriarContexto(dbName))
        {
            seedContext.AddRange(
                consultorUm,
                consultorDois,
                diaPrincipal,
                diaOutro,
                horarioTarde,
                horarioManha,
                horarioNoite,
                horarioOutroDia,
                agendaTarde,
                agendaManha,
                agendaNoiteExcluida,
                agendaOutroDia);

            await seedContext.SaveChangesAsync();
        }

        await using var readContext = CriarContexto(dbName);
        var repository = new AgendaConsultorRepository(readContext);

        // Act
        var resultado = await repository.GetByDiaAsync(diaPrincipal.Id);

        // Assert
        Assert.Equal(2, resultado.Count);
        Assert.Equal(
            new[] { horarioManha.Id, horarioTarde.Id },
            resultado.Select(x => x.HorarioDisponibilidadeId));
        Assert.All(resultado, item =>
        {
            Assert.Equal(diaPrincipal.Id, item.DiaDisponibilidadeId);
            Assert.NotNull(item.HorarioDisponibilidade);
            Assert.Equal(item.HorarioDisponibilidadeId, item.HorarioDisponibilidade.Id);
        });
        Assert.DoesNotContain(resultado, item => item.DiaDisponibilidadeId == diaOutro.Id);
        Assert.DoesNotContain(resultado, item => item.HorarioDisponibilidadeId == horarioNoite.Id);
        Assert.Empty(readContext.ChangeTracker.Entries());
    }

    [Fact]
    public async Task GetAllWithIncludesAsync_ComRegistrosAtivosEDesativados_Deve_RetornarSomenteAtivosComRelacionamentos()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        var consultorAtivo = CriarPessoaConsultor("Consultor Ativo");
        var consultorExcluido = CriarPessoaConsultor("Consultor Excluido");
        var diaAtivo = new DiaDisponibilidade(new DateOnly(2026, 8, 25));
        var diaExcluido = new DiaDisponibilidade(new DateOnly(2026, 8, 26));
        var horarioAtivo = new HorarioDisponibilidade(new TimeOnly(9, 30));
        var horarioExcluido = new HorarioDisponibilidade(new TimeOnly(10, 30));

        var agendaAtiva = new AgendaConsultor(diaAtivo.Id, horarioAtivo.Id, consultorAtivo.Id);
        var agendaExcluida = new AgendaConsultor(diaExcluido.Id, horarioExcluido.Id, consultorExcluido.Id);
        agendaExcluida.Excluir();

        await using (var seedContext = CriarContexto(dbName))
        {
            seedContext.AddRange(
                consultorAtivo,
                consultorExcluido,
                diaAtivo,
                diaExcluido,
                horarioAtivo,
                horarioExcluido,
                agendaAtiva,
                agendaExcluida);

            await seedContext.SaveChangesAsync();
        }

        await using var readContext = CriarContexto(dbName);
        var repository = new AgendaConsultorRepository(readContext);

        // Act
        var resultado = await repository.GetAllWithIncludesAsync();

        // Assert
        Assert.Single(resultado);
        var item = Assert.Single(resultado);
        Assert.Equal(agendaAtiva.Id, item.Id);
        Assert.NotNull(item.DiaDisponibilidade);
        Assert.NotNull(item.HorarioDisponibilidade);
        Assert.NotNull(item.Consultor);
        Assert.Equal(diaAtivo.Id, item.DiaDisponibilidade.Id);
        Assert.Equal(horarioAtivo.Id, item.HorarioDisponibilidade.Id);
        Assert.Equal(consultorAtivo.Id, item.Consultor.Id);
        Assert.DoesNotContain(resultado, x => x.Id == agendaExcluida.Id);
        Assert.Empty(readContext.ChangeTracker.Entries());
    }


    private static ApplicationDbContext CriarContexto(string dbName)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;

        return new ApplicationDbContext(options);
    }

    private static Pessoa CriarPessoaConsultor(string nome)
    {
        return new Pessoa(
            nome,
            new Cpf("12345678909"),
            new Telefone("11999999999"),
            new Endereco("Rua A", "1", null, "Centro", "São Paulo", "SP", new Cep("01001000")),
            Guid.NewGuid());
    }
}
