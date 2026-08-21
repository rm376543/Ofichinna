using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Ofichina.Application.Abstractions.Interfaces.Repository;
using Ofichina.Application.UseCases.Agendamentos.Handlers;
using Ofichina.Application.UseCases.Agendamentos.Queries;
using Ofichina.Domain.Entities;

namespace Ofichina.UnitTests.Application.UseCases.Agendamentos.Handlers;

public sealed class ListarDiasDisponiveisQueryHandlerTests
{
    // ============================================================  
    // SUCESSO - filtra, ordena e mapeia dias  
    // ============================================================  

    [Fact]
    public async Task HandleAsync_Deve_Listar_Dias_Filtrados_E_Ordenados()
    {
        // Arrange  
        // Usa uma data-base futura para garantir que d.Data >= hoje seja verdadeiro.  
        var baseFutura = DateOnly.FromDateTime(DateTime.Today).AddMonths(1);
        var mes = baseFutura.Month;
        var ano = baseFutura.Year;

        var query = new ListarDiasDisponiveisQuery
        {
            Mes = mes,
            Ano = ano
        };

        // Dois dias no mês/ano alvo, fornecidos fora de ordem (para exercitar o OrderBy).  
        var diaMaisTarde = new DiaDisponibilidade(new DateOnly(ano, mes, 20));
        var diaMaisCedo = new DiaDisponibilidade(new DateOnly(ano, mes, 10));

        // Dia de outro mês/ano -> deve ser filtrado (ramo false do Where).  
        var diaForaDoMes = new DiaDisponibilidade(baseFutura.AddMonths(1).ToDateOnlyPrimeiroDia());

        var diaDisponibilidadeRepository = new Mock<IDiaDisponibilidadeRepository>();

        diaDisponibilidadeRepository
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<DiaDisponibilidade>
            {
                diaMaisTarde,
                diaForaDoMes,
                diaMaisCedo
            });

        var handler = CriarHandler(diaDisponibilidadeRepository);

        // Act  
        var result = await handler.HandleAsync(query);

        // Assert  
        Assert.True(result.IsSuccess);

        var dias = result.Value!.ToList();

        // Apenas os dois dias do mês/ano alvo passam no filtro.  
        Assert.Equal(2, dias.Count);

        // Ordenados por data crescente: dia 10 antes do dia 20.  
        Assert.Equal(diaMaisCedo.Id, dias[0].DiaId);
        Assert.Equal(diaMaisTarde.Id, dias[1].DiaId);
    }

    // ============================================================  
    // SUCESSO - lista vazia quando nenhum dia satisfaz o filtro  
    // ============================================================  

    [Fact]
    public async Task HandleAsync_Deve_Retornar_Lista_Vazia_Quando_Nao_Houver_Dias()
    {
        // Arrange  
        var query = new ListarDiasDisponiveisQuery
        {
            Mes = 1,
            Ano = 2000 // ano no passado -> nenhum dia passa no filtro  
        };

        var diaDisponibilidadeRepository = new Mock<IDiaDisponibilidadeRepository>();

        diaDisponibilidadeRepository
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<DiaDisponibilidade>());

        var handler = CriarHandler(diaDisponibilidadeRepository);

        // Act  
        var result = await handler.HandleAsync(query);

        // Assert  
        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value!);
    }

    // ============================================================  
    // FALHA - exceção inesperada  
    // ============================================================  

    [Fact]
    public async Task HandleAsync_Deve_Retornar_Falha_Quando_Ocorrer_Excecao()
    {
        // Arrange  
        var query = new ListarDiasDisponiveisQuery
        {
            Mes = 8,
            Ano = 2026
        };

        var diaDisponibilidadeRepository = new Mock<IDiaDisponibilidadeRepository>();

        diaDisponibilidadeRepository
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Falha inesperada."));

        var handler = CriarHandler(diaDisponibilidadeRepository);

        // Act  
        var result = await handler.HandleAsync(query);

        // Assert  
        Assert.False(result.IsSuccess);
        Assert.Equal("Erro ao listar dias disponíveis.", result.Error);
    }

    // ============================================================  
    // HELPERS  
    // ============================================================  

    private static ListarDiasDisponiveisQueryHandler CriarHandler(
        Mock<IDiaDisponibilidadeRepository> diaDisponibilidadeRepository)
    {
        return new ListarDiasDisponiveisQueryHandler(
            diaDisponibilidadeRepository.Object,
            NullLogger<ListarDiasDisponiveisQueryHandler>.Instance);
    }
}

internal static class DiaDisponibilidadeTestExtensions
{
    // Retorna o primeiro dia do mês/ano da data informada.  
    public static DateOnly ToDateOnlyPrimeiroDia(this DateOnly data)
        => new DateOnly(data.Year, data.Month, 1);
}