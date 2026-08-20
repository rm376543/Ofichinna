using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Ofichina.Application.Abstractions.Interfaces.Repository;
using Ofichina.Application.UseCases.Pessoas.Commands;
using Ofichina.Application.UseCases.Pessoas.Handlers;
using Ofichina.Contracts.Requests.Pessoa;
using Ofichina.Domain.Entities;
using System.Reflection;

namespace Ofichina.UnitTests.Application.UseCases.Pessoas.Handlers;

public sealed class UpdatePessoaCommandHandlerTests
{
    // ============================================================
    // SUCESSO
    // ============================================================

    [Fact]
    public async Task Deve_Atualizar_Pessoa_Quando_Todos_Os_Dados_Forem_Validos()
    {
        // Arrange
        var pessoaId = Guid.NewGuid();
        var pessoa = CriarPessoa();

        var command = CriarCommand(pessoaId);

        var repository = new Mock<IPessoaRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();

        repository
            .Setup(x => x.GetByIdAsync(
                pessoaId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(pessoa);

        var handler = CriarHandler(
            repository,
            unitOfWork);

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        Assert.True(result.IsSuccess, result.Error);

        repository.Verify(
            x => x.GetByIdAsync(
                pessoaId,
                It.IsAny<CancellationToken>()),
            Times.Once);

        repository.Verify(
            x => x.UpdateAsync(
                pessoa,
                It.IsAny<CancellationToken>()),
            Times.Once);

        unitOfWork.Verify(
            x => x.SaveChangesAsync(),
            Times.Once);
    }

    // ============================================================
    // PESSOA NÃO ENCONTRADA
    // ============================================================

    [Fact]
    public async Task Deve_Retornar_Falha_Quando_Pessoa_Nao_For_Encontrada()
    {
        // Arrange
        var pessoaId = Guid.NewGuid();
        var command = CriarCommand(pessoaId);

        var repository = new Mock<IPessoaRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();

        repository
            .Setup(x => x.GetByIdAsync(
                pessoaId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Pessoa?)null);

        var handler = CriarHandler(
            repository,
            unitOfWork);

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(
            "Pessoa não encontrada.",
            result.Error);

        repository.Verify(
            x => x.GetByIdAsync(
                pessoaId,
                It.IsAny<CancellationToken>()),
            Times.Once);

        repository.Verify(
            x => x.UpdateAsync(
                It.IsAny<Pessoa>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        unitOfWork.Verify(
            x => x.SaveChangesAsync(),
            Times.Never);
    }

    // ============================================================
    // PESSOA EXCLUÍDA
    // ============================================================

    [Fact]
    public async Task Deve_Retornar_Falha_Quando_Pessoa_Estiver_Excluida()
    {
        // Arrange
        var pessoaId = Guid.NewGuid();
        var pessoa = CriarPessoa();

        MarcarComoExcluida(pessoa);

        var command = CriarCommand(pessoaId);

        var repository = new Mock<IPessoaRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();

        repository
            .Setup(x => x.GetByIdAsync(
                pessoaId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(pessoa);

        var handler = CriarHandler(
            repository,
            unitOfWork);

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(
            "Pessoa não encontrada.",
            result.Error);

        repository.Verify(
            x => x.GetByIdAsync(
                pessoaId,
                It.IsAny<CancellationToken>()),
            Times.Once);

        repository.Verify(
            x => x.UpdateAsync(
                It.IsAny<Pessoa>(),
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
        var pessoaId = Guid.NewGuid();
        var pessoa = CriarPessoa();

        // CEP inválido provoca DomainException dentro de
        // new Cep(command.Cep), no próprio handler.
        var command = CriarCommand(
            pessoaId,
            cep: "000");

        var repository = new Mock<IPessoaRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();

        repository
            .Setup(x => x.GetByIdAsync(
                pessoaId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(pessoa);

        var handler = CriarHandler(
            repository,
            unitOfWork);

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        Assert.False(result.IsSuccess);

        Assert.False(
            string.IsNullOrWhiteSpace(result.Error));

        repository.Verify(
            x => x.GetByIdAsync(
                pessoaId,
                It.IsAny<CancellationToken>()),
            Times.Once);

        repository.Verify(
            x => x.UpdateAsync(
                It.IsAny<Pessoa>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        unitOfWork.Verify(
            x => x.SaveChangesAsync(),
            Times.Never);
    }

    // ============================================================
    // EXCEPTION INESPERADA - GET
    // ============================================================

    [Fact]
    public async Task Deve_Retornar_Falha_Quando_Repositorio_Lancar_Excecao()
    {
        // Arrange
        var pessoaId = Guid.NewGuid();
        var command = CriarCommand(pessoaId);

        var repository = new Mock<IPessoaRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();

        repository
            .Setup(x => x.GetByIdAsync(
                pessoaId,
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(
                new InvalidOperationException(
                    "Falha inesperada de infraestrutura."));

        var handler = CriarHandler(
            repository,
            unitOfWork);

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(
            "Ocorreu um erro ao atualizar a pessoa.",
            result.Error);

        repository.Verify(
            x => x.GetByIdAsync(
                pessoaId,
                It.IsAny<CancellationToken>()),
            Times.Once);

        repository.Verify(
            x => x.UpdateAsync(
                It.IsAny<Pessoa>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        unitOfWork.Verify(
            x => x.SaveChangesAsync(),
            Times.Never);
    }

    // ============================================================
    // EXCEPTION INESPERADA - UPDATE
    // ============================================================

    [Fact]
    public async Task Deve_Retornar_Falha_Quando_Atualizacao_Lancar_Excecao()
    {
        // Arrange
        var pessoaId = Guid.NewGuid();
        var pessoa = CriarPessoa();
        var command = CriarCommand(pessoaId);

        var repository = new Mock<IPessoaRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();

        repository
            .Setup(x => x.GetByIdAsync(
                pessoaId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(pessoa);

        repository
            .Setup(x => x.UpdateAsync(
                pessoa,
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(
                new InvalidOperationException(
                    "Falha inesperada ao atualizar."));

        var handler = CriarHandler(
            repository,
            unitOfWork);

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(
            "Ocorreu um erro ao atualizar a pessoa.",
            result.Error);

        repository.Verify(
            x => x.GetByIdAsync(
                pessoaId,
                It.IsAny<CancellationToken>()),
            Times.Once);

        repository.Verify(
            x => x.UpdateAsync(
                pessoa,
                It.IsAny<CancellationToken>()),
            Times.Once);

        unitOfWork.Verify(
            x => x.SaveChangesAsync(),
            Times.Never);
    }

    // ============================================================
    // EXCEPTION INESPERADA - SAVE
    // ============================================================

    [Fact]
    public async Task Deve_Retornar_Falha_Quando_SaveChanges_Lancar_Excecao()
    {
        // Arrange
        var pessoaId = Guid.NewGuid();
        var pessoa = CriarPessoa();
        var command = CriarCommand(pessoaId);

        var repository = new Mock<IPessoaRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();

        repository
            .Setup(x => x.GetByIdAsync(
                pessoaId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(pessoa);

        repository
            .Setup(x => x.UpdateAsync(
                pessoa,
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        unitOfWork
            .Setup(x => x.SaveChangesAsync())
            .ThrowsAsync(
                new InvalidOperationException(
                    "Falha inesperada ao salvar."));

        var handler = CriarHandler(
            repository,
            unitOfWork);

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(
            "Ocorreu um erro ao atualizar a pessoa.",
            result.Error);

        repository.Verify(
            x => x.GetByIdAsync(
                pessoaId,
                It.IsAny<CancellationToken>()),
            Times.Once);

        repository.Verify(
            x => x.UpdateAsync(
                pessoa,
                It.IsAny<CancellationToken>()),
            Times.Once);

        unitOfWork.Verify(
            x => x.SaveChangesAsync(),
            Times.Once);
    }

    // ============================================================
    // HELPERS
    // ============================================================

    private static UpdatePessoaCommand CriarCommand(
        Guid? pessoaId = null,
        string? nome = null,
        string? telefone = null,
        string? logradouro = null,
        string? numero = null,
        string? complemento = null,
        string? bairro = null,
        string? cidade = null,
        string? estado = null,
        string? cep = null)
    {
        return new UpdatePessoaCommand(
            new UpdatePessoaRequest
            {
                PessoaId = pessoaId ?? Guid.NewGuid(),
                Nome = nome ?? "João da Silva",
                Telefone = telefone ?? "(17) 99999-9999",
                Logradouro = logradouro ?? "Rua das Flores",
                Numero = numero ?? "123",
                Complemento = complemento ?? "Casa",
                Bairro = bairro ?? "Centro",
                Cidade = cidade ?? "São José do Rio Preto",
                Estado = estado ?? "SP",
                Cep = cep ?? "15000-000"
            });
    }

    private static Pessoa CriarPessoa()
    {
        return (Pessoa)Activator.CreateInstance(
            typeof(Pessoa),
            BindingFlags.Instance |
            BindingFlags.NonPublic,
            binder: null,
            args: null,
            culture: null)!;
    }

    private static UpdatePessoaCommandHandler CriarHandler(
        Mock<IPessoaRepository> repository,
        Mock<IUnitOfWork> unitOfWork)
    {
        return new UpdatePessoaCommandHandler(
            repository.Object,
            unitOfWork.Object,
            NullLogger<UpdatePessoaCommandHandler>.Instance);
    }

    private static void MarcarComoExcluida(Pessoa pessoa)
    {
        var metodo =
            typeof(Pessoa).GetMethod(
                "Excluir",
                BindingFlags.Instance |
                BindingFlags.Public |
                BindingFlags.NonPublic)
            ??
            typeof(Pessoa)
                .BaseType?
                .GetMethod(
                    "Excluir",
                    BindingFlags.Instance |
                    BindingFlags.Public |
                    BindingFlags.NonPublic);

        Assert.NotNull(metodo);

        metodo!.Invoke(pessoa, null);
    }
}