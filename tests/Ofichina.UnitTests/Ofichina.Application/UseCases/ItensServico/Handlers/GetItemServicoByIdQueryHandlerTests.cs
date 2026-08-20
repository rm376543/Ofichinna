using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Ofichina.Application.Abstractions.Interfaces.Repository;
using Ofichina.Application.UseCases.ItensServico.Handlers;
using Ofichina.Application.UseCases.ItensServico.Queries;
using Ofichina.Domain.Aggregates;
using Ofichina.Domain.Entities;

namespace Ofichina.UnitTests.Application.UseCases.ItensServico.Handlers;

public sealed class GetItemServicoByIdQueryHandlerTests
{
    private static OrdemServico CriarOrdemServico()
    {
        return new OrdemServico(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            0,
            "Problema relatado de teste",
            "Observação de teste");
    }

    [Fact]
    public async Task HandleAsync_Deve_Retornar_Item_Quando_Ordem_E_Item_Existirem()
    {
        var ordemServico = CriarOrdemServico();
        var servicoId = Guid.NewGuid();
        var pecaId = Guid.NewGuid();
        var item = ItemServico.ParaOrdemServico(ordemServico.Id, servicoId, pecaId, 2);

        var ordemServicoRepository = new Mock<IOrdemServicoRepository>();
        var itemServicoRepository = new Mock<IItemServicoRepository>();

        ordemServicoRepository
            .Setup(x => x.GetByIdAsync(ordemServico.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ordemServico);

        itemServicoRepository
            .Setup(x => x.GetByOrdemServicoIdAndItemServicoIdAsync(
                ordemServico.Id,
                item.Id,
                It.IsAny<CancellationToken>(),
                false,
                true))
            .ReturnsAsync(item);

        var handler = new GetItemServicoByIdQueryHandler(
            ordemServicoRepository.Object,
            itemServicoRepository.Object,
            NullLogger<GetItemServicoByIdQueryHandler>.Instance);

        var result = await handler.HandleAsync(new GetItemServicoByIdQuery(ordemServico.Id, item.Id));

        Assert.True(result.IsSuccess, result.Error);
        Assert.Equal(ordemServico.Id, result.Value.OrdemServicoId);
        Assert.Single(result.Value.Servicos);

        var servicoResponse = result.Value.Servicos[0];
        Assert.Equal(servicoId, servicoResponse.ServicoId);
        Assert.Single(servicoResponse.Pecas);
        Assert.Equal(pecaId, servicoResponse.Pecas[0].PecaId);
        Assert.Equal(2, servicoResponse.Pecas[0].Quantidade);
    }

    [Fact]
    public async Task HandleAsync_Deve_Recusar_Quando_Ordem_De_Servico_Nao_For_Encontrada()
    {
        var ordemServicoId = Guid.NewGuid();
        var itemServicoId = Guid.NewGuid();

        var ordemServicoRepository = new Mock<IOrdemServicoRepository>();
        var itemServicoRepository = new Mock<IItemServicoRepository>();

        ordemServicoRepository
            .Setup(x => x.GetByIdAsync(ordemServicoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((OrdemServico?)null);

        var handler = new GetItemServicoByIdQueryHandler(
            ordemServicoRepository.Object,
            itemServicoRepository.Object,
            NullLogger<GetItemServicoByIdQueryHandler>.Instance);

        var result = await handler.HandleAsync(new GetItemServicoByIdQuery(ordemServicoId, itemServicoId));

        Assert.False(result.IsSuccess);
        Assert.Equal("Ordem de serviço não encontrada.", result.Error);
        itemServicoRepository.Verify(
            x => x.GetByOrdemServicoIdAndItemServicoIdAsync(
                It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>(), It.IsAny<bool>()),
            Times.Never);
    }

    [Fact]
    public async Task HandleAsync_Deve_Recusar_Quando_Ordem_De_Servico_Estiver_Excluida()
    {
        var ordemServico = CriarOrdemServico();
        ordemServico.Excluir();

        var itemServicoId = Guid.NewGuid();

        var ordemServicoRepository = new Mock<IOrdemServicoRepository>();
        var itemServicoRepository = new Mock<IItemServicoRepository>();

        ordemServicoRepository
            .Setup(x => x.GetByIdAsync(ordemServico.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ordemServico);

        var handler = new GetItemServicoByIdQueryHandler(
            ordemServicoRepository.Object,
            itemServicoRepository.Object,
            NullLogger<GetItemServicoByIdQueryHandler>.Instance);

        var result = await handler.HandleAsync(new GetItemServicoByIdQuery(ordemServico.Id, itemServicoId));

        Assert.False(result.IsSuccess);
        Assert.Equal("Ordem de serviço não encontrada.", result.Error);
    }

    [Fact]
    public async Task HandleAsync_Deve_Recusar_Quando_Item_De_Servico_Nao_For_Encontrado()
    {
        var ordemServico = CriarOrdemServico();
        var itemServicoId = Guid.NewGuid();

        var ordemServicoRepository = new Mock<IOrdemServicoRepository>();
        var itemServicoRepository = new Mock<IItemServicoRepository>();

        ordemServicoRepository
            .Setup(x => x.GetByIdAsync(ordemServico.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ordemServico);

        itemServicoRepository
            .Setup(x => x.GetByOrdemServicoIdAndItemServicoIdAsync(
                ordemServico.Id,
                itemServicoId,
                It.IsAny<CancellationToken>(),
                true))
            .ReturnsAsync((ItemServico?)null);

        var handler = new GetItemServicoByIdQueryHandler(
            ordemServicoRepository.Object,
            itemServicoRepository.Object,
            NullLogger<GetItemServicoByIdQueryHandler>.Instance);

        var result = await handler.HandleAsync(new GetItemServicoByIdQuery(ordemServico.Id, itemServicoId));

        Assert.False(result.IsSuccess);
        Assert.Equal("Item de serviço não encontrado.", result.Error);
    }

    [Fact]
    public async Task HandleAsync_Deve_Recusar_Quando_Item_De_Servico_Estiver_Excluido()
    {
        var ordemServico = CriarOrdemServico();
        var item = ItemServico.ParaOrdemServico(ordemServico.Id, Guid.NewGuid(), null, 1);
        item.Excluir();

        var ordemServicoRepository = new Mock<IOrdemServicoRepository>();
        var itemServicoRepository = new Mock<IItemServicoRepository>();

        ordemServicoRepository
            .Setup(x => x.GetByIdAsync(ordemServico.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ordemServico);

        itemServicoRepository
            .Setup(x => x.GetByOrdemServicoIdAndItemServicoIdAsync(
                ordemServico.Id,
                item.Id,
                It.IsAny<CancellationToken>(),
                true))
            .ReturnsAsync(item);

        var handler = new GetItemServicoByIdQueryHandler(
            ordemServicoRepository.Object,
            itemServicoRepository.Object,
            NullLogger<GetItemServicoByIdQueryHandler>.Instance);

        var result = await handler.HandleAsync(new GetItemServicoByIdQuery(ordemServico.Id, item.Id));

        Assert.False(result.IsSuccess);
        Assert.Equal("Item de serviço não encontrado.", result.Error);
    }

    [Fact]
    public async Task HandleAsync_Deve_Retornar_Falha_Quando_Ocorrer_Excecao()
    {
        var ordemServicoId = Guid.NewGuid();
        var itemServicoId = Guid.NewGuid();

        var ordemServicoRepository = new Mock<IOrdemServicoRepository>();
        var itemServicoRepository = new Mock<IItemServicoRepository>();

        ordemServicoRepository
            .Setup(x => x.GetByIdAsync(ordemServicoId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Falha inesperada."));

        var handler = new GetItemServicoByIdQueryHandler(
            ordemServicoRepository.Object,
            itemServicoRepository.Object,
            NullLogger<GetItemServicoByIdQueryHandler>.Instance);

        var result = await handler.HandleAsync(new GetItemServicoByIdQuery(ordemServicoId, itemServicoId));

        Assert.False(result.IsSuccess);
        Assert.Equal("Não foi possível obter o item de serviço.", result.Error);
    }

    [Fact]
    public async Task HandleAsync_Deve_Retornar_Item_Sem_Pecas_Quando_Nao_Houver_Peca_Vinculada()
    {
        var ordemServico = CriarOrdemServico();
        var servicoId = Guid.NewGuid();
        var item = ItemServico.ParaOrdemServico(ordemServico.Id, servicoId, null, 1);

        var ordemServicoRepository = new Mock<IOrdemServicoRepository>();
        var itemServicoRepository = new Mock<IItemServicoRepository>();

        ordemServicoRepository
            .Setup(x => x.GetByIdAsync(ordemServico.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ordemServico);

        itemServicoRepository
            .Setup(x => x.GetByOrdemServicoIdAndItemServicoIdAsync(
                ordemServico.Id,
                item.Id,
                It.IsAny<CancellationToken>(),
                false,
                true))
            .ReturnsAsync(item);

        var handler = new GetItemServicoByIdQueryHandler(
            ordemServicoRepository.Object,
            itemServicoRepository.Object,
            NullLogger<GetItemServicoByIdQueryHandler>.Instance);

        var result = await handler.HandleAsync(new GetItemServicoByIdQuery(ordemServico.Id, item.Id));

        Assert.True(result.IsSuccess, result.Error);
        var servicoResponse = result.Value.Servicos[0];
        Assert.Equal(servicoId, servicoResponse.ServicoId);
        Assert.Empty(servicoResponse.Pecas);
    }
}