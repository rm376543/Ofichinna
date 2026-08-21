using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Ofichina.Application.UseCases.Pecas.Commands;
using Ofichina.Application.UseCases.Pecas.Handlers;
using Ofichina.Contracts.Requests.Pecas;
using Ofichina.Domain.Entities;
using System.Reflection;

namespace Ofichina.UnitTests.Ofichina.Application.UseCases.Pecas.Handlers;

public sealed class UpdatePecaCommandHandlerTests
{
    // ============================================================
    // SUCESSO
    // ============================================================

    [Fact]
    public async Task Deve_Atualizar_Peca_Quando_Dados_Forem_Validos()
    {
        // Arrange
        var peca = CriarPeca();

        var command = CriarCommand(
            peca.Id,
            nome: "Pastilha de Freio Atualizada",
            descricao: "Descrição atualizada",
            codigo: "PF-002",
            valor: 250.50m,
            quantidadeEstoque: 15);

        var pecaRepository = new Mock<IRepository<Peca>>();
        var unitOfWork = new Mock<IUnitOfWork>();

        pecaRepository
            .Setup(x => x.GetByIdAsync(
                command.PecaId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(peca);

        var handler = CriarHandler(
            pecaRepository,
            unitOfWork);

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        Assert.True(result.IsSuccess, result.Error);

        Assert.Equal("Pastilha de Freio Atualizada", peca.Nome);
        Assert.Equal("Descrição atualizada", peca.Descricao);
        Assert.Equal("PF-002", peca.Codigo);
        Assert.Equal(250.50m, peca.Valor);
        Assert.Equal(15, peca.QuantidadeEstoque);

        pecaRepository.Verify(
            x => x.GetByIdAsync(
                command.PecaId,
                It.IsAny<CancellationToken>()),
            Times.Once);

        pecaRepository.Verify(
            x => x.UpdateAsync(
                peca,
                It.IsAny<CancellationToken>()),
            Times.Once);

        unitOfWork.Verify(
            x => x.SaveChangesAsync(),
            Times.Once);
    }

    // ============================================================
    // PEÇA NÃO ENCONTRADA
    // ============================================================

    [Fact]
    public async Task Deve_Retornar_Falha_Quando_Peca_Nao_For_Encontrada()
    {
        // Arrange
        var command = CriarCommand();

        var pecaRepository = new Mock<IRepository<Peca>>();
        var unitOfWork = new Mock<IUnitOfWork>();

        pecaRepository
            .Setup(x => x.GetByIdAsync(
                command.PecaId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Peca?)null);

        var handler = CriarHandler(
            pecaRepository,
            unitOfWork);

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Peça não encontrada.", result.Error);

        pecaRepository.Verify(
            x => x.UpdateAsync(
                It.IsAny<Peca>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        unitOfWork.Verify(
            x => x.SaveChangesAsync(),
            Times.Never);
    }

    // ============================================================
    // PEÇA EXCLUÍDA
    // ============================================================

    [Fact]
    public async Task Deve_Retornar_Falha_Quando_Peca_Estiver_Excluida()
    {
        // Arrange
        var peca = CriarPeca();
        MarcarComoExcluida(peca);

        var command = CriarCommand(peca.Id);

        var pecaRepository = new Mock<IRepository<Peca>>();
        var unitOfWork = new Mock<IUnitOfWork>();

        pecaRepository
            .Setup(x => x.GetByIdAsync(
                command.PecaId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(peca);

        var handler = CriarHandler(
            pecaRepository,
            unitOfWork);

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Peça não encontrada.", result.Error);

        pecaRepository.Verify(
            x => x.UpdateAsync(
                It.IsAny<Peca>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        unitOfWork.Verify(
            x => x.SaveChangesAsync(),
            Times.Never);
    }

    // ============================================================
    // DOMAIN EXCEPTION
    // ============================================================

    [Fact]
    public async Task Deve_Retornar_Falha_Quando_Atualizacao_Lancar_DomainException()
    {
        // Arrange
        var peca = CriarPeca();

        var command = CriarCommand(
            peca.Id,
            nome: "",
            descricao: "Descrição",
            codigo: "COD-001",
            valor: 100m,
            quantidadeEstoque: 10);

        var pecaRepository = new Mock<IRepository<Peca>>();
        var unitOfWork = new Mock<IUnitOfWork>();

        pecaRepository
            .Setup(x => x.GetByIdAsync(
                command.PecaId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(peca);

        var handler = CriarHandler(
            pecaRepository,
            unitOfWork);

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(
            "O nome da peça deve ser informado.",
            result.Error);

        pecaRepository.Verify(
            x => x.UpdateAsync(
                It.IsAny<Peca>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        unitOfWork.Verify(
            x => x.SaveChangesAsync(),
            Times.Never);
    }

    // ============================================================
    // EXCEPTION INESPERADA NO UPDATE
    // ============================================================

    [Fact]
    public async Task Deve_Retornar_Falha_Quando_Update_Lancar_Excecao()
    {
        // Arrange
        var peca = CriarPeca();

        var command = CriarCommand(peca.Id);

        var pecaRepository = new Mock<IRepository<Peca>>();
        var unitOfWork = new Mock<IUnitOfWork>();

        pecaRepository
            .Setup(x => x.GetByIdAsync(
                command.PecaId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(peca);

        pecaRepository
            .Setup(x => x.UpdateAsync(
                peca,
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(
                new InvalidOperationException(
                    "Erro inesperado de infraestrutura."));

        var handler = CriarHandler(
            pecaRepository,
            unitOfWork);

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(
            "Não foi possível atualizar a peça.",
            result.Error);

        unitOfWork.Verify(
            x => x.SaveChangesAsync(),
            Times.Never);
    }

    // ============================================================
    // EXCEPTION INESPERADA NO SAVE
    // ============================================================

    [Fact]
    public async Task Deve_Retornar_Falha_Quando_SaveChanges_Lancar_Excecao()
    {
        // Arrange
        var peca = CriarPeca();

        var command = CriarCommand(peca.Id);

        var pecaRepository = new Mock<IRepository<Peca>>();
        var unitOfWork = new Mock<IUnitOfWork>();

        pecaRepository
            .Setup(x => x.GetByIdAsync(
                command.PecaId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(peca);

        pecaRepository
            .Setup(x => x.UpdateAsync(
                peca,
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        unitOfWork
            .Setup(x => x.SaveChangesAsync())
            .ThrowsAsync(
                new InvalidOperationException(
                    "Erro inesperado ao salvar."));

        var handler = CriarHandler(
            pecaRepository,
            unitOfWork);

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(
            "Não foi possível atualizar a peça.",
            result.Error);

        pecaRepository.Verify(
            x => x.UpdateAsync(
                peca,
                It.IsAny<CancellationToken>()),
            Times.Once);

        unitOfWork.Verify(
            x => x.SaveChangesAsync(),
            Times.Once);
    }

    // ============================================================
    // DOMAIN EXCEPTION — CÓDIGO
    // ============================================================

    [Fact]
    public async Task Deve_Retornar_Falha_Quando_Codigo_For_Invalido()
    {
        // Arrange
        var peca = CriarPeca();

        var command = CriarCommand(
            peca.Id,
            nome: "Pastilha",
            descricao: "Descrição",
            codigo: "",
            valor: 100m,
            quantidadeEstoque: 10);

        var pecaRepository = new Mock<IRepository<Peca>>();
        var unitOfWork = new Mock<IUnitOfWork>();

        pecaRepository
            .Setup(x => x.GetByIdAsync(
                command.PecaId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(peca);

        var handler = CriarHandler(
            pecaRepository,
            unitOfWork);

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(
            "O código da peça deve ser informado.",
            result.Error);

        pecaRepository.Verify(
            x => x.UpdateAsync(
                It.IsAny<Peca>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        unitOfWork.Verify(
            x => x.SaveChangesAsync(),
            Times.Never);
    }

    // ============================================================
    // DOMAIN EXCEPTION — VALOR
    // ============================================================

    [Fact]
    public async Task Deve_Retornar_Falha_Quando_Valor_For_Invalido()
    {
        // Arrange
        var peca = CriarPeca();

        var command = CriarCommand(
            peca.Id,
            nome: "Pastilha",
            descricao: "Descrição",
            codigo: "COD-001",
            valor: 0m,
            quantidadeEstoque: 10);

        var pecaRepository = new Mock<IRepository<Peca>>();
        var unitOfWork = new Mock<IUnitOfWork>();

        pecaRepository
            .Setup(x => x.GetByIdAsync(
                command.PecaId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(peca);

        var handler = CriarHandler(
            pecaRepository,
            unitOfWork);

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(
            "O valor da peça deve ser maior que zero.",
            result.Error);

        pecaRepository.Verify(
            x => x.UpdateAsync(
                It.IsAny<Peca>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        unitOfWork.Verify(
            x => x.SaveChangesAsync(),
            Times.Never);
    }

    // ============================================================
    // DOMAIN EXCEPTION — ESTOQUE
    // ============================================================

    [Fact]
    public async Task Deve_Retornar_Falha_Quando_Quantidade_Estoque_For_Negativa()
    {
        // Arrange
        var peca = CriarPeca();

        var command = CriarCommand(
            peca.Id,
            nome: "Pastilha",
            descricao: "Descrição",
            codigo: "COD-001",
            valor: 100m,
            quantidadeEstoque: -1);

        var pecaRepository = new Mock<IRepository<Peca>>();
        var unitOfWork = new Mock<IUnitOfWork>();

        pecaRepository
            .Setup(x => x.GetByIdAsync(
                command.PecaId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(peca);

        var handler = CriarHandler(
            pecaRepository,
            unitOfWork);

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(
            "A quantidade em estoque não pode ser negativa.",
            result.Error);

        pecaRepository.Verify(
            x => x.UpdateAsync(
                It.IsAny<Peca>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        unitOfWork.Verify(
            x => x.SaveChangesAsync(),
            Times.Never);
    }

    // ============================================================
    // HELPERS
    // ============================================================

    private static UpdatePecaCommandHandler CriarHandler(
        Mock<IRepository<Peca>> pecaRepository,
        Mock<IUnitOfWork> unitOfWork)
    {
        return new UpdatePecaCommandHandler(
            pecaRepository.Object,
            unitOfWork.Object,
            NullLogger<UpdatePecaCommandHandler>.Instance);
    }

    private static Peca CriarPeca()
    {
        return new Peca(
            "Pastilha de Freio",
            "Pastilha dianteira",
            "PF-001",
            150m,
            10);
    }

    private static UpdatePecaCommand CriarCommand(
        Guid? pecaId = null,
        string nome = "Pastilha de Freio Atualizada",
        string? descricao = "Descrição atualizada",
        string codigo = "PF-002",
        decimal valor = 200m,
        int quantidadeEstoque = 20)
    {
        var request = new UpdatePecaRequest
        {
            PecaId = pecaId ?? Guid.NewGuid(),
            Nome = nome,
            Descricao = descricao,
            Codigo = codigo,
            Valor = valor,
            QuantidadeEstoque = quantidadeEstoque
        };

        return new UpdatePecaCommand(request);
    }

    private static void MarcarComoExcluida(Peca peca)
    {
        var metodo = typeof(Entity)
            .GetMethod(
                "Excluir",
                BindingFlags.Instance |
                BindingFlags.Public |
                BindingFlags.NonPublic);

        Assert.NotNull(metodo);

        metodo!.Invoke(peca, null);
    }
}