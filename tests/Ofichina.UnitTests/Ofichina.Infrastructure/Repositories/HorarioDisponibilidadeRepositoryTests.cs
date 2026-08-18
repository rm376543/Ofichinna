using Microsoft.EntityFrameworkCore;
using Ofichina.Contracts.Common;
using Ofichina.Domain.Entities;
using Ofichina.Infrastructure.Persistence;
using Ofichina.Infrastructure.Repositories;

namespace Ofichina.UnitTests.Infrastructure.Repositories;

public sealed class HorarioDisponibilidadeRepositoryTests
{
    [Fact]
    public async Task Constructor_ContextoValido_Deve_CriarRepositorio()
    {
        await using var context = CriarContexto(Guid.NewGuid().ToString());

        var repository = new HorarioDisponibilidadeRepository(context);

        Assert.NotNull(repository);
    }

    [Fact]
    public async Task GetHorariosPorDiaAsync_DiaComVariosHorarios_Deve_Retornar_Ordenado()
    {
        var dbName = Guid.NewGuid().ToString();
        var dia = new DiaDisponibilidade(new DateOnly(2026, 8, 16));
        var horarioManha = new HorarioDisponibilidade(new TimeOnly(8, 0));
        var horarioTarde = new HorarioDisponibilidade(new TimeOnly(12, 0));
        var horarioOutroDia = new HorarioDisponibilidade(new TimeOnly(18, 0));

        await using (var context = CriarContexto(dbName))
        {
            context.AddRange(dia, horarioTarde, horarioManha, horarioOutroDia);
            await context.SaveChangesAsync();

            var outroDia = new DiaDisponibilidade(new DateOnly(2026, 8, 17));
            context.Add(outroDia);
            await context.SaveChangesAsync();

            context.AddRange(
                new DiaHorarioDisponibilidade(dia.Id, horarioTarde.Id),
                new DiaHorarioDisponibilidade(dia.Id, horarioManha.Id),
                new DiaHorarioDisponibilidade(outroDia.Id, horarioOutroDia.Id));

            await context.SaveChangesAsync();
        }

        await using var readContext = CriarContexto(dbName);
        var repository = new HorarioDisponibilidadeRepository(readContext);

        var resultado = await repository.GetHorariosPorDiaAsync(dia.Id);

        Assert.Collection(
            resultado,
            item => Assert.Equal(new TimeOnly(8, 0), item.Hora),
            item => Assert.Equal(new TimeOnly(12, 0), item.Hora));
    }

    [Fact]
    public async Task GetPagedAsync_PaginationNula_Deve_LancarArgumentNullException()
    {
        var dbName = Guid.NewGuid().ToString();
        await using var context = CriarContexto(dbName);
        var repository = new HorarioDisponibilidadeRepository(context);

        await Assert.ThrowsAsync<ArgumentNullException>(() => repository.GetPagedAsync(null!));
    }

    [Fact]
    public async Task GetPagedAsync_PaginacaoInvalida_Deve_Normalizar_E_OrdenarPorCreatedAt()
    {
        var dbName = Guid.NewGuid().ToString();
        var maisAntigo = new HorarioDisponibilidade(new TimeOnly(9, 0))
        {
            CreatedAt = new DateTime(2024, 1, 1, 8, 0, 0, DateTimeKind.Utc)
        };
        var intermediario = new HorarioDisponibilidade(new TimeOnly(10, 0))
        {
            CreatedAt = new DateTime(2024, 1, 2, 8, 0, 0, DateTimeKind.Utc)
        };
        var maisRecente = new HorarioDisponibilidade(new TimeOnly(11, 0))
        {
            CreatedAt = new DateTime(2024, 1, 3, 8, 0, 0, DateTimeKind.Utc)
        };

        await using (var context = CriarContexto(dbName))
        {
            context.AddRange(maisRecente, intermediario, maisAntigo);
            await context.SaveChangesAsync();
        }

        await using var readContext = CriarContexto(dbName);
        var repository = new HorarioDisponibilidadeRepository(readContext);

        var resultado = await repository.GetPagedAsync(new Pagination(0, 0));

        Assert.Equal(3, resultado.TotalCount);
        Assert.Equal(1, resultado.PageNumber);
        Assert.Equal(10, resultado.PageSize);
        Assert.Equal(3, resultado.Items.Count);
        Assert.Collection(
            resultado.Items,
            item => Assert.Equal(new TimeOnly(9, 0), item.Hora),
            item => Assert.Equal(new TimeOnly(10, 0), item.Hora),
            item => Assert.Equal(new TimeOnly(11, 0), item.Hora));
    }

    [Fact]
    public async Task BuscarPorHorarioAsync_HorarioAtivo_Deve_RetornarEntidade()
    {
        var horarioExcluido = new HorarioDisponibilidade(new TimeOnly(7, 15));
        horarioExcluido.Excluir();

        var horarioAtivo = new HorarioDisponibilidade(new TimeOnly(7, 15));

        var dbName = Guid.NewGuid().ToString();
        await using (var context = CriarContexto(dbName))
        {
            context.AddRange(horarioExcluido, horarioAtivo);
            await context.SaveChangesAsync();
        }

        await using var readContext = CriarContexto(dbName);
        var repository = new HorarioDisponibilidadeRepository(readContext);

        var resultado = await repository.BuscarPorHorarioAsync(new TimeOnly(7, 15));

        Assert.NotNull(resultado);
        Assert.Equal(new TimeOnly(7, 15), resultado!.Hora);
        Assert.Null(resultado.DeletedAt);
    }

    [Fact]
    public async Task BuscarPorHorarioAsync_HorarioExcluido_Deve_RetornarNull()
    {
        var horarioExcluido = new HorarioDisponibilidade(new TimeOnly(6, 45));
        horarioExcluido.Excluir();

        var dbName = Guid.NewGuid().ToString();
        await using (var context = CriarContexto(dbName))
        {
            context.Add(horarioExcluido);
            await context.SaveChangesAsync();
        }

        await using var readContext = CriarContexto(dbName);
        var repository = new HorarioDisponibilidadeRepository(readContext);

        var resultado = await repository.BuscarPorHorarioAsync(new TimeOnly(6, 45));

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
