using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Ofichina.Application.Abstractions.Interfaces.Repository;
using Ofichina.Application.UseCases.Pecas.Handlers;
using Ofichina.Application.UseCases.Pecas.Queries;
using Ofichina.Contracts.Common;
using Ofichina.Domain.Entities;
using Ofichina.UnitTests.TestInfrastructure;

namespace Ofichina.UnitTests.Application.UseCases.Pecas.Handlers;

public sealed class GetAllPecasPaginadasQueryHandlerTests
{
    // ============================================================  
    // SUCESSO  
    // ============================================================  

    [Fact]
    public async Task HandleAsync_Deve_Retornar_Pecas_Paginadas_Com_Sucesso()
    {
        // Arrange  
        var pagination = new Pagination(1, 10);
        var query = new GetAllPecasPaginadasQuery(pagination);

        var peca = CriarPeca();

        var pagedPecas = new PagedResponse<Peca>
        {
            Items = [peca],
            PageNumber = 1,
            PageSize = 10,
            TotalCount = 1,
            TotalPages = 1,
            HasNextPage = false,
            HasPreviousPage = false
        };

        var pecaRepository = new Mock<IPecaRepository>();

        pecaRepository
            .Setup(x => x.GetPagedAsync(
                pagination,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedPecas);

        var handler = CriarHandler(pecaRepository);

        // Act  
        var result = await handler.HandleAsync(query);

        // Assert  
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Single(result.Value!.Items);

        var item = result.Value!.Items.First();
        Assert.Equal(peca.Id, item.PecaId);
        Assert.Equal(peca.Nome, item.Nome);
        Assert.Equal(peca.Descricao, item.Descricao);
        Assert.Equal(peca.Codigo, item.Codigo);
        Assert.Equal(peca.Valor, item.Valor);
        Assert.Equal(peca.QuantidadeEstoque, item.QuantidadeEstoque);
        Assert.NotNull(item.UpdatedAt); // ramo não-nulo de UpdatedAt?.ToDateString()  
        Assert.Null(item.DeletedAt);    // ramo nulo de DeletedAt?.ToDateString()  
    }

    // ============================================================  
    // NULO  
    // ============================================================  

    [Fact]
    public async Task HandleAsync_Deve_Retornar_Falha_Quando_Resultado_For_Nulo()
    {
        // Arrange  
        var pagination = new Pagination(1, 10);
        var query = new GetAllPecasPaginadasQuery(pagination);

        var pecaRepository = new Mock<IPecaRepository>();

        pecaRepository
            .Setup(x => x.GetPagedAsync(
                pagination,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((PagedResponse<Peca>)null!);

        var handler = CriarHandler(pecaRepository);

        // Act  
        var result = await handler.HandleAsync(query);

        // Assert  
        Assert.False(result.IsSuccess);
        Assert.Equal("Nenhuma peça encontrada.", result.Error);
    }

    // ============================================================  
    // EXCEÇÃO  
    // ============================================================  

    [Fact]
    public async Task HandleAsync_Deve_Retornar_Falha_Quando_Ocorrer_Excecao()
    {
        // Arrange  
        var pagination = new Pagination(1, 10);
        var query = new GetAllPecasPaginadasQuery(pagination);

        var pecaRepository = new Mock<IPecaRepository>();

        pecaRepository
            .Setup(x => x.GetPagedAsync(
                pagination,
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Falha inesperada."));

        var handler = CriarHandler(pecaRepository);

        // Act  
        var result = await handler.HandleAsync(query);

        // Assert  
        Assert.False(result.IsSuccess);
        Assert.Equal("Não foi possível obter as peças.", result.Error);
    }

    // ============================================================  
    // HELPERS  
    // ============================================================  

    private static GetAllPecasPaginadasQueryHandler CriarHandler(
        Mock<IPecaRepository> pecaRepository)
    {
        return new GetAllPecasPaginadasQueryHandler(
            pecaRepository.Object,
            NullLogger<GetAllPecasPaginadasQueryHandler>.Instance);
    }

    private static Peca CriarPeca()
    {
        var peca = TestDataFactory.Pecas.Criar(p => p.AtualizarDados("Pastilha de freio", "Pastilha dianteira", "PF-1234", 149.90m, 10));
        peca.CreatedAt = new DateTime(2026, 8, 10, 0, 0, 0, DateTimeKind.Utc);
        peca.UpdatedAt = new DateTime(2026, 8, 11, 0, 0, 0, DateTimeKind.Utc);
        return peca;
    }
}