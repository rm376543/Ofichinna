using Microsoft.EntityFrameworkCore;
using Ofichina.Domain.Entities;
using Ofichina.Domain.ValueObjects;
using Ofichina.Infrastructure.Persistence;
using Ofichina.Infrastructure.Repositories;

namespace Ofichina.UnitTests;

public sealed class HorarioConsultorRepositoryTests
{
    [Fact]
    public async Task HorarioConsultorRepository_ContextoValido_Deve_CriarInstancia()
    {
        await using var context = CriarContexto(Guid.NewGuid().ToString());

        var repository = new HorarioConsultorRepository(context);

        Assert.NotNull(repository);
    }

    [Fact]
    public async Task GetConsultoresPorHorarioAsync_HorarioExistente_Deve_Retornar_Apenas_Consultores_Ordenados_E_Sem_Tracking()
    {
        var dbName = Guid.NewGuid().ToString();
        var horarioAlvo = new HorarioDisponibilidade(new TimeOnly(8, 0));
        var horarioIgnorado = new HorarioDisponibilidade(new TimeOnly(9, 0));
        var pessoaMaria = CriarPessoa("Maria Consultora", Guid.NewGuid());
        var pessoaAna = CriarPessoa("Ana Consultora", Guid.NewGuid());
        var pessoaZara = CriarPessoa("Zara Consultora", Guid.NewGuid());
        var pessoaForaDoHorario = CriarPessoa("Bruno Externo", Guid.NewGuid());
        var vinculoMaria = new HorarioConsultor(horarioAlvo.Id, pessoaMaria.Id);
        var vinculoAna = new HorarioConsultor(horarioAlvo.Id, pessoaAna.Id);
        var vinculoZara = new HorarioConsultor(horarioAlvo.Id, pessoaZara.Id);
        var vinculoIgnorado = new HorarioConsultor(horarioIgnorado.Id, pessoaForaDoHorario.Id);

        await using (var context = CriarContexto(dbName))
        {
            context.HorariosDisponibilidade.AddRange(horarioAlvo, horarioIgnorado);
            context.Pessoas.AddRange(pessoaMaria, pessoaAna, pessoaZara, pessoaForaDoHorario);
            context.HorariosConsultores.AddRange(vinculoMaria, vinculoAna, vinculoZara, vinculoIgnorado);
            await context.SaveChangesAsync();
        }

        await using var readContext = CriarContexto(dbName);
        var repository = new HorarioConsultorRepository(readContext);

        var resultado = await repository.GetConsultoresPorHorarioAsync(horarioAlvo.Id);

        Assert.Equal(3, resultado.Count);
        Assert.Empty(readContext.ChangeTracker.Entries<HorarioConsultor>());
        Assert.Collection(
            resultado,
            item =>
            {
                Assert.Equal(vinculoAna.Id, item.Id);
                Assert.Equal(horarioAlvo.Id, item.HorarioDisponibilidadeId);
                Assert.NotNull(item.Pessoa);
                Assert.Equal("Ana Consultora", item.Pessoa.Nome);
            },
            item =>
            {
                Assert.Equal(vinculoMaria.Id, item.Id);
                Assert.Equal(horarioAlvo.Id, item.HorarioDisponibilidadeId);
                Assert.NotNull(item.Pessoa);
                Assert.Equal("Maria Consultora", item.Pessoa.Nome);
            },
            item =>
            {
                Assert.Equal(vinculoZara.Id, item.Id);
                Assert.Equal(horarioAlvo.Id, item.HorarioDisponibilidadeId);
                Assert.NotNull(item.Pessoa);
                Assert.Equal("Zara Consultora", item.Pessoa.Nome);
            });
    }

    [Fact]
    public async Task GetConsultoresPorHorarioAsync_HorarioSemConsultores_Deve_Retornar_Colecao_Vazia()
    {
        var dbName = Guid.NewGuid().ToString();
        var horarioAlvo = new HorarioDisponibilidade(new TimeOnly(8, 0));
        var outroHorario = new HorarioDisponibilidade(new TimeOnly(9, 0));
        var pessoa = CriarPessoa("Consultora Teste", Guid.NewGuid());
        var vinculo = new HorarioConsultor(outroHorario.Id, pessoa.Id);

        await using (var context = CriarContexto(dbName))
        {
            context.HorariosDisponibilidade.AddRange(horarioAlvo, outroHorario);
            context.Pessoas.Add(pessoa);
            context.HorariosConsultores.Add(vinculo);
            await context.SaveChangesAsync();
        }

        await using var readContext = CriarContexto(dbName);
        var repository = new HorarioConsultorRepository(readContext);

        var resultado = await repository.GetConsultoresPorHorarioAsync(horarioAlvo.Id);

        Assert.Empty(resultado);
        Assert.Empty(readContext.ChangeTracker.Entries<HorarioConsultor>());
    }

    private static ApplicationDbContext CriarContexto(string dbName)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;

        return new ApplicationDbContext(options);
    }

    private static Pessoa CriarPessoa(string nome, Guid usuarioId)
    {
        return new Pessoa(
            nome,
            new Cpf("529.982.247-25"),
            new Telefone("11999999999"),
            new Endereco("Rua A", "1", null, "Centro", "São Paulo", "SP", new Cep("01001000")),
            usuarioId);
    }
}
