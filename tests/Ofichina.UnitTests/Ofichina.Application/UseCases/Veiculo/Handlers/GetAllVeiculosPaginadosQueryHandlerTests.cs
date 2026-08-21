using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Ofichina.Application.Abstractions.Interfaces.Repository;
using Ofichina.Application.UseCases.Veiculos.Handlers;
using Ofichina.Application.UseCases.Veiculos.Queries;
using Ofichina.Contracts.Common;
using Ofichina.Domain.Entities;
using Ofichina.Domain.ValueObjects;

namespace Ofichina.UnitTests.Application.UseCases.Veiculos.Handlers;

public sealed class GetAllVeiculosPaginadosQueryHandlerTests
{
    // ============================================================  
    // SUCESSO - mapeia itens paginados  
    // ============================================================  

    [Fact]
    public async Task HandleAsync_Deve_Mapear_Veiculos_Paginados_Com_Sucesso()
    {
        // Arrange  
        var pagination = new Pagination(2, 10);
        var query = new GetAllVeiculosPaginadosQuery(pagination);

        var veiculo = CriarVeiculo();

        var pagedResponse = new PagedResponse<Veiculo>
        {
            Items = [veiculo],
            PageNumber = 2,
            PageSize = 10,
            TotalCount = 20,
            TotalPages = 2,
            HasNextPage = false,
            HasPreviousPage = true
        };

        var veiculoRepository = new Mock<IVeiculoRepository>();

        veiculoRepository
            .Setup(x => x.GetPagedAsync(
                pagination,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResponse);

        var handler = CriarHandler(veiculoRepository);

        // Act  
        var result = await handler.HandleAsync(query);

        // Assert  
        Assert.True(result.IsSuccess, result.Error);
        Assert.NotNull(result.Value);
        Assert.Equal(2, result.Value!.PageNumber);
        Assert.Equal(10, result.Value.PageSize);
        Assert.Equal(20, result.Value.TotalCount);
        Assert.Equal(2, result.Value.TotalPages);
        Assert.False(result.Value.HasNextPage);
        Assert.True(result.Value.HasPreviousPage);

        var item = Assert.Single(result.Value.Items);
        Assert.Equal(veiculo.Id, item.VeiculoId);
        Assert.Equal(veiculo.Placa.ToString(), item.Placa);
        Assert.Equal(veiculo.Marca, item.Marca);
        Assert.Equal(veiculo.Modelo, item.Modelo);
        Assert.Equal(veiculo.AnoFabricacao, item.AnoFabricacao);
        Assert.Equal(veiculo.Cor, item.Cor);
        Assert.Equal(veiculo.Hodometro.Valor, item.Hodometro);
        Assert.Equal(veiculo.Hodometro.ToString(), item.HodometroFormatado);

        veiculoRepository.Verify(
            x => x.GetPagedAsync(
                pagination,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    // ============================================================  
    // FALHA - repositório retorna null  
    // ============================================================  

    [Fact]
    public async Task HandleAsync_Deve_Retornar_Falha_Quando_Resultado_For_Nulo()
    {
        // Arrange  
        var pagination = new Pagination(1, 10);
        var query = new GetAllVeiculosPaginadosQuery(pagination);

        var veiculoRepository = new Mock<IVeiculoRepository>();

        veiculoRepository
            .Setup(x => x.GetPagedAsync(
                pagination,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((PagedResponse<Veiculo>)null!);

        var handler = CriarHandler(veiculoRepository);

        // Act  
        var result = await handler.HandleAsync(query);

        // Assert  
        Assert.False(result.IsSuccess);
        Assert.Equal("Nenhum veículo encontrado.", result.Error);
    }

    // ============================================================  
    // FALHA - exceção inesperada  
    // ============================================================  

    [Fact]
    public async Task HandleAsync_Deve_Retornar_Falha_Quando_Ocorrer_Excecao()
    {
        // Arrange  
        var pagination = new Pagination(1, 10);
        var query = new GetAllVeiculosPaginadosQuery(pagination);

        var veiculoRepository = new Mock<IVeiculoRepository>();

        veiculoRepository
            .Setup(x => x.GetPagedAsync(
                pagination,
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Falha inesperada."));

        var handler = CriarHandler(veiculoRepository);

        // Act  
        var result = await handler.HandleAsync(query);

        // Assert  
        Assert.False(result.IsSuccess);
        Assert.Equal("Não foi possível obter os veículos.", result.Error);
    }

    // ============================================================  
    // HELPERS  
    // ============================================================  

    private static GetAllVeiculosPaginadosQueryHandler CriarHandler(
        Mock<IVeiculoRepository> veiculoRepository)
    {
        return new GetAllVeiculosPaginadosQueryHandler(
            veiculoRepository.Object,
            NullLogger<GetAllVeiculosPaginadosQueryHandler>.Instance);
    }

    private static Veiculo CriarVeiculo()
    {
        return new Veiculo(
            Guid.NewGuid(),
            new Placa("ABC1D23"),
            "Ford",
            "Ka",
            2022,
            "Prata",
            new Hodometro(10000))
        {
            CreatedAt = new DateTime(2026, 8, 10, 0, 0, 0, DateTimeKind.Utc),
            UpdatedAt = new DateTime(2026, 8, 11, 0, 0, 0, DateTimeKind.Utc)
        };
    }
}