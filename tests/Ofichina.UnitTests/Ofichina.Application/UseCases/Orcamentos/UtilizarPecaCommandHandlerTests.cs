using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Ofichina.Application.Abstractions.Interfaces.Repository;
using Ofichina.Application.UseCases.OrdensServico.Commands;
using Ofichina.Application.UseCases.OrdensServico.Handlers;
using Ofichina.Contracts.Requests.Pecas;
using Ofichina.Domain.Aggregates;
using Ofichina.Domain.Entities;
using Ofichina.Domain.Exceptions;

namespace Ofichina.UnitTests.Application.UseCases.OrdensServico.Handlers;

public sealed class UtilizarPecaCommandHandlerTests
{
    // ============================================================
    // HandleAsync - Sucesso
    // ============================================================

    [Fact]
    public async Task HandleAsync_Deve_Utilizar_Peca_Com_Sucesso()
    {
        var ordemServicoRepository =
            new Mock<IOrdemServicoRepository>();

        var itemServicoRepository =
            new Mock<IItemServicoRepository>();

        var pecaRepository =
            new Mock<IRepository<Peca>>();

        var unitOfWork =
            new Mock<IUnitOfWork>();

        var command = CriarCommand();

        var ordemServico = CriarOrdemServico();

        var itemServico = CriarItemServicoComPeca(
            command.OrdemServicoId,
            quantidade: 2);

        var peca = CriarPeca(
            quantidadeEstoque: 10);

        ordemServicoRepository
            .Setup(x => x.GetByIdAsync(
                command.OrdemServicoId,
                true,
                true,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(ordemServico);

        itemServicoRepository
            .Setup(x => x.GetByOrdemServicoIdAndItemServicoIdAsync(
                command.OrdemServicoId,
                command.ItemServicoId,
                It.IsAny<CancellationToken>(),
                true,
                true))
            .ReturnsAsync(itemServico);

        pecaRepository
            .Setup(x => x.GetByIdAsync(
                itemServico.PecaId!.Value,
                It.IsAny<CancellationToken>(),
                true))
            .ReturnsAsync(peca);

        var estoqueAntes = peca.QuantidadeEstoque;

        var handler = CriarHandler(
            ordemServicoRepository,
            itemServicoRepository,
            pecaRepository,
            unitOfWork);

        var result = await handler.HandleAsync(
            command,
            CancellationToken.None);

        Assert.True(result.IsSuccess);

        Assert.Equal(
            estoqueAntes - itemServico.Quantidade,
            peca.QuantidadeEstoque);

        ordemServicoRepository.Verify(
            x => x.GetByIdAsync(
                command.OrdemServicoId,
                true,
                true,
                It.IsAny<CancellationToken>()),
            Times.Once);

        itemServicoRepository.Verify(
            x => x.GetByOrdemServicoIdAndItemServicoIdAsync(
                command.OrdemServicoId,
                command.ItemServicoId,
                It.IsAny<CancellationToken>(),
                true,
                true),
            Times.Once);

        pecaRepository.Verify(
            x => x.GetByIdAsync(
                itemServico.PecaId.Value,
                It.IsAny<CancellationToken>(),
                true),
            Times.Once);

        unitOfWork.Verify(
            x => x.SaveChangesAsync(),
            Times.Once);
    }

    // ============================================================
    // HandleAsync - Ordem de serviço não encontrada
    // ============================================================

    [Fact]
    public async Task HandleAsync_Deve_Retornar_Falha_Quando_Ordem_De_Servico_Nao_Existir()
    {
        var ordemServicoRepository =
            new Mock<IOrdemServicoRepository>();

        var itemServicoRepository =
            new Mock<IItemServicoRepository>();

        var pecaRepository =
            new Mock<IRepository<Peca>>();

        var unitOfWork =
            new Mock<IUnitOfWork>();

        var command = CriarCommand();

        ordemServicoRepository
            .Setup(x => x.GetByIdAsync(
                command.OrdemServicoId,
                true,
                true,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((OrdemServico?)null);

        var handler = CriarHandler(
            ordemServicoRepository,
            itemServicoRepository,
            pecaRepository,
            unitOfWork);

        var result = await handler.HandleAsync(
            command,
            CancellationToken.None);

        Assert.False(result.IsSuccess);

        Assert.Equal(
            "Ordem de serviço não encontrada.",
            result.Error);

        ordemServicoRepository.Verify(
            x => x.GetByIdAsync(
                command.OrdemServicoId,
                true,
                true,
                It.IsAny<CancellationToken>()),
            Times.Once);

        itemServicoRepository.Verify(
            x => x.GetByOrdemServicoIdAndItemServicoIdAsync(
                It.IsAny<Guid>(),
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>(),
                true,
                true),
            Times.Never);

        pecaRepository.Verify(
            x => x.GetByIdAsync(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>(),
                true),
            Times.Never);

        unitOfWork.Verify(
            x => x.SaveChangesAsync(),
            Times.Never);
    }

    // ============================================================
    // HandleAsync - Ordem de serviço excluída
    // ============================================================

    [Fact]
    public async Task HandleAsync_Deve_Retornar_Falha_Quando_Ordem_De_Servico_Estiver_Excluida()
    {
        var ordemServicoRepository =
            new Mock<IOrdemServicoRepository>();

        var itemServicoRepository =
            new Mock<IItemServicoRepository>();

        var pecaRepository =
            new Mock<IRepository<Peca>>();

        var unitOfWork =
            new Mock<IUnitOfWork>();

        var command = CriarCommand();

        var ordemServico = CriarOrdemServico();

        ordemServico.Excluir();

        ordemServicoRepository
            .Setup(x => x.GetByIdAsync(
                command.OrdemServicoId,
                true,
                true,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(ordemServico);

        var handler = CriarHandler(
            ordemServicoRepository,
            itemServicoRepository,
            pecaRepository,
            unitOfWork);

        var result = await handler.HandleAsync(
            command,
            CancellationToken.None);

        Assert.False(result.IsSuccess);

        Assert.Equal(
            "Ordem de serviço não encontrada.",
            result.Error);

        itemServicoRepository.Verify(
            x => x.GetByOrdemServicoIdAndItemServicoIdAsync(
                It.IsAny<Guid>(),
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>(),
                true,
                true),
            Times.Never);

        pecaRepository.Verify(
            x => x.GetByIdAsync(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>(),
                true),
            Times.Never);

        unitOfWork.Verify(
            x => x.SaveChangesAsync(),
            Times.Never);
    }

    // ============================================================
    // HandleAsync - Item de serviço não encontrado
    // ============================================================

    [Fact]
    public async Task HandleAsync_Deve_Retornar_Falha_Quando_Item_Servico_Nao_Existir()
    {
        var ordemServicoRepository =
            new Mock<IOrdemServicoRepository>();

        var itemServicoRepository =
            new Mock<IItemServicoRepository>();

        var pecaRepository =
            new Mock<IRepository<Peca>>();

        var unitOfWork =
            new Mock<IUnitOfWork>();

        var command = CriarCommand();

        ordemServicoRepository
            .Setup(x => x.GetByIdAsync(
                command.OrdemServicoId,
                true,
                true,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(CriarOrdemServico());

        itemServicoRepository
            .Setup(x => x.GetByOrdemServicoIdAndItemServicoIdAsync(
                command.OrdemServicoId,
                command.ItemServicoId,
                It.IsAny<CancellationToken>(),
                true,
                true))
            .ReturnsAsync((ItemServico?)null);

        var handler = CriarHandler(
            ordemServicoRepository,
            itemServicoRepository,
            pecaRepository,
            unitOfWork);

        var result = await handler.HandleAsync(
            command,
            CancellationToken.None);

        Assert.False(result.IsSuccess);

        Assert.Equal(
            "Item de serviço não encontrado.",
            result.Error);

        pecaRepository.Verify(
            x => x.GetByIdAsync(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>(),
                true),
            Times.Never);

        unitOfWork.Verify(
            x => x.SaveChangesAsync(),
            Times.Never);
    }

    // ============================================================
    // HandleAsync - Item de serviço excluído
    // ============================================================

    [Fact]
    public async Task HandleAsync_Deve_Retornar_Falha_Quando_Item_Servico_Estiver_Excluido()
    {
        var ordemServicoRepository =
            new Mock<IOrdemServicoRepository>();

        var itemServicoRepository =
            new Mock<IItemServicoRepository>();

        var pecaRepository =
            new Mock<IRepository<Peca>>();

        var unitOfWork =
            new Mock<IUnitOfWork>();

        var command = CriarCommand();

        var itemServico = CriarItemServicoComPeca(
            command.OrdemServicoId,
            quantidade: 2);

        itemServico.Excluir();

        ordemServicoRepository
            .Setup(x => x.GetByIdAsync(
                command.OrdemServicoId,
                true,
                true,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(CriarOrdemServico());

        itemServicoRepository
            .Setup(x => x.GetByOrdemServicoIdAndItemServicoIdAsync(
                command.OrdemServicoId,
                command.ItemServicoId,
                It.IsAny<CancellationToken>(),
                true,
                true))
            .ReturnsAsync(itemServico);

        var handler = CriarHandler(
            ordemServicoRepository,
            itemServicoRepository,
            pecaRepository,
            unitOfWork);

        var result = await handler.HandleAsync(
            command,
            CancellationToken.None);

        Assert.False(result.IsSuccess);

        Assert.Equal(
            "Item de serviço não encontrado.",
            result.Error);

        pecaRepository.Verify(
            x => x.GetByIdAsync(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>(),
                true),
            Times.Never);

        unitOfWork.Verify(
            x => x.SaveChangesAsync(),
            Times.Never);
    }

    // ============================================================
    // HandleAsync - Item sem peça
    // ============================================================

    [Fact]
    public async Task HandleAsync_Deve_Retornar_Falha_Quando_Item_Nao_Possuir_Peca()
    {
        var ordemServicoRepository =
            new Mock<IOrdemServicoRepository>();

        var itemServicoRepository =
            new Mock<IItemServicoRepository>();

        var pecaRepository =
            new Mock<IRepository<Peca>>();

        var unitOfWork =
            new Mock<IUnitOfWork>();

        var command = CriarCommand();

        var itemServico = ItemServico.ParaOrdemServico(
            command.OrdemServicoId,
            Guid.NewGuid(),
            null,
            2);

        ordemServicoRepository
            .Setup(x => x.GetByIdAsync(
                command.OrdemServicoId,
                true,
                true,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(CriarOrdemServico());

        itemServicoRepository
            .Setup(x => x.GetByOrdemServicoIdAndItemServicoIdAsync(
                command.OrdemServicoId,
                command.ItemServicoId,
                It.IsAny<CancellationToken>(),
                true,
                true))
            .ReturnsAsync(itemServico);

        var handler = CriarHandler(
            ordemServicoRepository,
            itemServicoRepository,
            pecaRepository,
            unitOfWork);

        var result = await handler.HandleAsync(
            command,
            CancellationToken.None);

        Assert.False(result.IsSuccess);

        Assert.Equal(
            "O item de serviço não possui peça vinculada.",
            result.Error);

        pecaRepository.Verify(
            x => x.GetByIdAsync(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>(),
                true),
            Times.Never);

        unitOfWork.Verify(
            x => x.SaveChangesAsync(),
            Times.Never);
    }

    // ============================================================
    // HandleAsync - Peça não encontrada
    // ============================================================

    [Fact]
    public async Task HandleAsync_Deve_Retornar_Falha_Quando_Peca_Nao_Existir()
    {
        var ordemServicoRepository =
            new Mock<IOrdemServicoRepository>();

        var itemServicoRepository =
            new Mock<IItemServicoRepository>();

        var pecaRepository =
            new Mock<IRepository<Peca>>();

        var unitOfWork =
            new Mock<IUnitOfWork>();

        var command = CriarCommand();

        var itemServico = CriarItemServicoComPeca(
            command.OrdemServicoId,
            quantidade: 2);

        ordemServicoRepository
            .Setup(x => x.GetByIdAsync(
                command.OrdemServicoId,
                true,
                true,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(CriarOrdemServico());

        itemServicoRepository
            .Setup(x => x.GetByOrdemServicoIdAndItemServicoIdAsync(
                command.OrdemServicoId,
                command.ItemServicoId,
                It.IsAny<CancellationToken>(),
                true,
                true))
            .ReturnsAsync(itemServico);

        pecaRepository
            .Setup(x => x.GetByIdAsync(
                itemServico.PecaId!.Value,
                It.IsAny<CancellationToken>(),
                true))
            .ReturnsAsync((Peca?)null);

        var handler = CriarHandler(
            ordemServicoRepository,
            itemServicoRepository,
            pecaRepository,
            unitOfWork);

        var result = await handler.HandleAsync(
            command,
            CancellationToken.None);

        Assert.False(result.IsSuccess);

        Assert.Equal(
            "Peça de catálogo não encontrada.",
            result.Error);

        unitOfWork.Verify(
            x => x.SaveChangesAsync(),
            Times.Never);
    }

    // ============================================================
    // HandleAsync - Peça excluída
    // ============================================================

    [Fact]
    public async Task HandleAsync_Deve_Retornar_Falha_Quando_Peca_Estiver_Excluida()
    {
        var ordemServicoRepository =
            new Mock<IOrdemServicoRepository>();

        var itemServicoRepository =
            new Mock<IItemServicoRepository>();

        var pecaRepository =
            new Mock<IRepository<Peca>>();

        var unitOfWork =
            new Mock<IUnitOfWork>();

        var command = CriarCommand();

        var itemServico = CriarItemServicoComPeca(
            command.OrdemServicoId,
            quantidade: 2);

        var peca = CriarPeca(10);

        peca.ExcluirLogicamente();

        ordemServicoRepository
            .Setup(x => x.GetByIdAsync(
                command.OrdemServicoId,
                true,
                true,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(CriarOrdemServico());

        itemServicoRepository
            .Setup(x => x.GetByOrdemServicoIdAndItemServicoIdAsync(
                command.OrdemServicoId,
                command.ItemServicoId,
                It.IsAny<CancellationToken>(),
                true,
                true))
            .ReturnsAsync(itemServico);

        pecaRepository
            .Setup(x => x.GetByIdAsync(
                itemServico.PecaId!.Value,
                It.IsAny<CancellationToken>(),
                true))
            .ReturnsAsync(peca);

        var handler = CriarHandler(
            ordemServicoRepository,
            itemServicoRepository,
            pecaRepository,
            unitOfWork);

        var result = await handler.HandleAsync(
            command,
            CancellationToken.None);

        Assert.False(result.IsSuccess);

        Assert.Equal(
            "Peça de catálogo não encontrada.",
            result.Error);

        unitOfWork.Verify(
            x => x.SaveChangesAsync(),
            Times.Never);
    }

    // ============================================================
    // HandleAsync - Estoque insuficiente
    // ============================================================

    [Fact]
    public async Task HandleAsync_Deve_Retornar_Falha_Quando_Estoque_For_Insuficiente()
    {
        var ordemServicoRepository =
            new Mock<IOrdemServicoRepository>();

        var itemServicoRepository =
            new Mock<IItemServicoRepository>();

        var pecaRepository =
            new Mock<IRepository<Peca>>();

        var unitOfWork =
            new Mock<IUnitOfWork>();

        var command = CriarCommand();

        var itemServico = CriarItemServicoComPeca(
            command.OrdemServicoId,
            quantidade: 10);

        var peca = CriarPeca(5);

        ordemServicoRepository
            .Setup(x => x.GetByIdAsync(
                command.OrdemServicoId,
                true,
                true,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(CriarOrdemServico());

        itemServicoRepository
            .Setup(x => x.GetByOrdemServicoIdAndItemServicoIdAsync(
                command.OrdemServicoId,
                command.ItemServicoId,
                It.IsAny<CancellationToken>(),
                true,
                true))
            .ReturnsAsync(itemServico);

        pecaRepository
            .Setup(x => x.GetByIdAsync(
                itemServico.PecaId!.Value,
                It.IsAny<CancellationToken>(),
                true))
            .ReturnsAsync(peca);

        var handler = CriarHandler(
            ordemServicoRepository,
            itemServicoRepository,
            pecaRepository,
            unitOfWork);

        var result = await handler.HandleAsync(
            command,
            CancellationToken.None);

        Assert.False(result.IsSuccess);

        Assert.Equal(
            "Quantidade insuficiente em estoque.",
            result.Error);

        Assert.Equal(
            5,
            peca.QuantidadeEstoque);

        unitOfWork.Verify(
            x => x.SaveChangesAsync(),
            Times.Never);
    }

    // ============================================================
    // HandleAsync - DomainException
    // ============================================================

    [Fact]
    public async Task HandleAsync_Deve_Retornar_Falha_Quando_Ocorrrer_DomainException()
    {
        var ordemServicoRepository =
            new Mock<IOrdemServicoRepository>();

        var itemServicoRepository =
            new Mock<IItemServicoRepository>();

        var pecaRepository =
            new Mock<IRepository<Peca>>();

        var unitOfWork =
            new Mock<IUnitOfWork>();

        var command = CriarCommand();

        var itemServico = CriarItemServicoComPeca(
            command.OrdemServicoId,
            quantidade: 2);

        var peca = CriarPeca(10);

        ordemServicoRepository
            .Setup(x => x.GetByIdAsync(
                command.OrdemServicoId,
                true,
                true,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(CriarOrdemServico());

        itemServicoRepository
            .Setup(x => x.GetByOrdemServicoIdAndItemServicoIdAsync(
                command.OrdemServicoId,
                command.ItemServicoId,
                It.IsAny<CancellationToken>(),
                true,
                true))
            .ReturnsAsync(itemServico);

        pecaRepository
            .Setup(x => x.GetByIdAsync(
                itemServico.PecaId!.Value,
                It.IsAny<CancellationToken>(),
                true))
            .ReturnsAsync(peca);

        unitOfWork
            .Setup(x => x.SaveChangesAsync())
            .ThrowsAsync(
                new DomainException(
                    "Erro de domínio ao salvar a utilização da peça."));

        var handler = CriarHandler(
            ordemServicoRepository,
            itemServicoRepository,
            pecaRepository,
            unitOfWork);

        var result = await handler.HandleAsync(
            command,
            CancellationToken.None);

        Assert.False(result.IsSuccess);

        Assert.Equal(
            "Erro de domínio ao salvar a utilização da peça.",
            result.Error);

        unitOfWork.Verify(
            x => x.SaveChangesAsync(),
            Times.Once);
    }

    // ============================================================
    // HandleAsync - Exception
    // ============================================================

    [Fact]
    public async Task HandleAsync_Deve_Retornar_Falha_Quando_Ocorrrer_Excecao()
    {
        var ordemServicoRepository =
            new Mock<IOrdemServicoRepository>();

        var itemServicoRepository =
            new Mock<IItemServicoRepository>();

        var pecaRepository =
            new Mock<IRepository<Peca>>();

        var unitOfWork =
            new Mock<IUnitOfWork>();

        var command = CriarCommand();

        ordemServicoRepository
            .Setup(x => x.GetByIdAsync(
                command.OrdemServicoId,
                true,
                true,
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(
                new InvalidOperationException(
                    "Erro inesperado."));

        var handler = CriarHandler(
            ordemServicoRepository,
            itemServicoRepository,
            pecaRepository,
            unitOfWork);

        var result = await handler.HandleAsync(
            command,
            CancellationToken.None);

        Assert.False(result.IsSuccess);

        Assert.Equal(
            "Não foi possível utilizar a peça.",
            result.Error);

        ordemServicoRepository.Verify(
            x => x.GetByIdAsync(
                command.OrdemServicoId,
                true,
                true,
                It.IsAny<CancellationToken>()),
            Times.Once);

        itemServicoRepository.Verify(
            x => x.GetByOrdemServicoIdAndItemServicoIdAsync(
                It.IsAny<Guid>(),
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>(),
                true,
                true),
            Times.Never);

        pecaRepository.Verify(
            x => x.GetByIdAsync(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>(),
                true),
            Times.Never);

        unitOfWork.Verify(
            x => x.SaveChangesAsync(),
            Times.Never);
    }

    // ============================================================
    // Helpers
    // ============================================================

    private static UtilizarPecaCommand CriarCommand()
    {
        var request = new UtilizarPecaRequest
        {
            OrdemServicoId = Guid.NewGuid(),
            ItemServicoId = Guid.NewGuid(),
            PecaId = Guid.NewGuid()
        };

        return new UtilizarPecaCommand(request);
    }

    private static UtilizarPecaCommandHandler CriarHandler(
        Mock<IOrdemServicoRepository> ordemServicoRepository,
        Mock<IItemServicoRepository> itemServicoRepository,
        Mock<IRepository<Peca>> pecaRepository,
        Mock<IUnitOfWork> unitOfWork)
        => new(
            ordemServicoRepository.Object,
            itemServicoRepository.Object,
            pecaRepository.Object,
            unitOfWork.Object,
            NullLogger<UtilizarPecaCommandHandler>.Instance);

    private static ItemServico CriarItemServicoComPeca(
        Guid ordemServicoId,
        int quantidade)
        => ItemServico.ParaOrdemServico(
            ordemServicoId,
            Guid.NewGuid(),
            Guid.NewGuid(),
            quantidade);

    private static Peca CriarPeca(
        int quantidadeEstoque)
        => new(
            "Pastilha de freio",
            "Pastilha de freio dianteira.",
            $"PC-{Guid.NewGuid():N}",
            150m,
            quantidadeEstoque);

    private static OrdemServico CriarOrdemServico()
    {
        var ordemServico =
            (OrdemServico)Activator.CreateInstance(
                typeof(OrdemServico),
                nonPublic: true)!;

        return ordemServico;
    }
}