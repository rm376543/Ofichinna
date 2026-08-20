using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Ofichina.Application.Abstractions.Interfaces.Repository;
using Ofichina.Application.UseCases.Pessoas.Commands;
using Ofichina.Application.UseCases.Pessoas.Handlers;
using Ofichina.Domain.Entities;
using Ofichina.Domain.Exceptions;
using System.Reflection;
using System.Runtime.Serialization;

namespace Ofichina.UnitTests.Application.UseCases.Pessoas.Handlers;

public sealed class CreatePessoaCommandHandlerTests
{
    // ============================================================
    // USUÁRIO
    // ============================================================

    [Fact]
    public async Task HandleAsync_Deve_Retornar_Falha_Quando_Usuario_Nao_Existir()
    {
        var command = CriarCommand();

        var usuarioRepository =
            new Mock<IRepository<Usuario>>();

        usuarioRepository
            .Setup(x => x.GetByIdAsync(
                command.UsuarioId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Usuario?)null);

        var handler = CriarHandler(
            usuarioRepository: usuarioRepository);

        var result = await handler.HandleAsync(
            command,
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(
            "Usuário não encontrado.",
            result.Error);
    }

    // ============================================================
    // DOCUMENTO
    // ============================================================

    [Fact]
    public async Task HandleAsync_Deve_Criar_Pessoa_Com_Cpf_Com_Sucesso()
    {
        var command = CriarCommand(
            documento: "12345678909");

        var repository =
            new Mock<IPessoaRepository>();

        var usuarioRepository =
            CriarUsuarioRepositoryValido(command);

        var unitOfWork =
            new Mock<IUnitOfWork>();

        var handler = CriarHandler(
            repository: repository,
            usuarioRepository: usuarioRepository,
            unitOfWork: unitOfWork);

        var result = await handler.HandleAsync(
            command,
            CancellationToken.None);

        Assert.True(result.IsSuccess);

        repository.Verify(
            x => x.AddAsync(
                It.Is<Pessoa>(p =>
                    p.UsuarioId == command.UsuarioId),
                It.IsAny<CancellationToken>()),
            Times.Once);

        unitOfWork.Verify(
            x => x.SaveChangesAsync(),
            Times.Once);
    }

    [Fact]
    public async Task HandleAsync_Deve_Criar_Pessoa_Com_Cnpj_Com_Sucesso()
    {
        var command = CriarCommand(
            documento: "11222333000181");

        var repository =
            new Mock<IPessoaRepository>();

        var usuarioRepository =
            CriarUsuarioRepositoryValido(command);

        var unitOfWork =
            new Mock<IUnitOfWork>();

        var handler = CriarHandler(
            repository: repository,
            usuarioRepository: usuarioRepository,
            unitOfWork: unitOfWork);

        var result = await handler.HandleAsync(
            command,
            CancellationToken.None);

        Assert.True(result.IsSuccess);

        repository.Verify(
            x => x.AddAsync(
                It.Is<Pessoa>(p =>
                    p.UsuarioId == command.UsuarioId),
                It.IsAny<CancellationToken>()),
            Times.Once);

        unitOfWork.Verify(
            x => x.SaveChangesAsync(),
            Times.Once);
    }

    [Fact]
    public async Task HandleAsync_Deve_Retornar_Falha_Quando_Documento_For_Invalido()
    {
        var command = CriarCommand(
            documento: "123");

        var repository =
            new Mock<IPessoaRepository>();

        var usuarioRepository =
            CriarUsuarioRepositoryValido(command);

        var unitOfWork =
            new Mock<IUnitOfWork>();

        var handler = CriarHandler(
            repository: repository,
            usuarioRepository: usuarioRepository,
            unitOfWork: unitOfWork);

        var result = await handler.HandleAsync(
            command,
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(
            "Documento inválido.",
            result.Error);

        repository.Verify(
            x => x.AddAsync(
                It.IsAny<Pessoa>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        unitOfWork.Verify(
            x => x.SaveChangesAsync(),
            Times.Never);
    }

    [Fact]
    public async Task HandleAsync_Deve_Criar_Pessoa_Quando_Documento_Contiver_Mascara()
    {
        var command = CriarCommand(
            documento: "123.456.789-09");

        var repository =
            new Mock<IPessoaRepository>();

        var usuarioRepository =
            CriarUsuarioRepositoryValido(command);

        var unitOfWork =
            new Mock<IUnitOfWork>();

        var handler = CriarHandler(
            repository: repository,
            usuarioRepository: usuarioRepository,
            unitOfWork: unitOfWork);

        var result = await handler.HandleAsync(
            command,
            CancellationToken.None);

        Assert.True(result.IsSuccess);

        repository.Verify(
            x => x.AddAsync(
                It.IsAny<Pessoa>(),
                It.IsAny<CancellationToken>()),
            Times.Once);

        unitOfWork.Verify(
            x => x.SaveChangesAsync(),
            Times.Once);
    }

    // ============================================================
    // DOMAIN EXCEPTION
    // ============================================================

    [Fact]
    public async Task HandleAsync_Deve_Retornar_Falha_Quando_Ocorrer_DomainException()
    {
        var command = CriarCommand();

        var usuarioRepository =
            CriarUsuarioRepositoryValido(command);

        var repository =
            new Mock<IPessoaRepository>();

        repository
            .Setup(x => x.AddAsync(
                It.IsAny<Pessoa>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(
                new DomainException(
                    "Erro de domínio."));

        var handler = CriarHandler(
            repository: repository,
            usuarioRepository: usuarioRepository);

        var result = await handler.HandleAsync(
            command,
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(
            "Erro de domínio.",
            result.Error);
    }

    // ============================================================
    // EXCEPTION
    // ============================================================

    [Fact]
    public async Task HandleAsync_Deve_Retornar_Falha_Quando_Ocorrer_Excecao()
    {
        var command = CriarCommand();

        var usuarioRepository =
            CriarUsuarioRepositoryValido(command);

        var repository =
            new Mock<IPessoaRepository>();

        repository
            .Setup(x => x.AddAsync(
                It.IsAny<Pessoa>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(
                new InvalidOperationException(
                    "Erro inesperado."));

        var handler = CriarHandler(
            repository: repository,
            usuarioRepository: usuarioRepository);

        var result = await handler.HandleAsync(
            command,
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(
            "Ocorreu um erro ao criar a pessoa.",
            result.Error);
    }

    // ============================================================
    // FACTORY
    // ============================================================

    private static CreatePessoaCommandHandler CriarHandler(
        Mock<IPessoaRepository>? repository = null,
        Mock<IRepository<Usuario>>? usuarioRepository = null,
        Mock<IUnitOfWork>? unitOfWork = null)
    {
        return new CreatePessoaCommandHandler(
            (repository ??
                new Mock<IPessoaRepository>()).Object,

            (usuarioRepository ??
                new Mock<IRepository<Usuario>>()).Object,

            (unitOfWork ??
                new Mock<IUnitOfWork>()).Object,

            NullLogger<CreatePessoaCommandHandler>.Instance);
    }

    // ============================================================
    // REPOSITORIES
    // ============================================================

    private static Mock<IRepository<Usuario>>
        CriarUsuarioRepositoryValido(
            CreatePessoaCommand command)
    {
        var repository =
            new Mock<IRepository<Usuario>>();

        var usuario = CriarUsuario();

        DefinirPropriedade(
            usuario,
            nameof(Usuario.Id),
            command.UsuarioId);

        repository
            .Setup(x => x.GetByIdAsync(
                command.UsuarioId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(usuario);

        return repository;
    }

    // ============================================================
    // COMMAND
    // ============================================================

    private static CreatePessoaCommand CriarCommand(
        string documento = "12345678909")
    {
        var command =
            (CreatePessoaCommand)
                FormatterServices.GetUninitializedObject(
                    typeof(CreatePessoaCommand));

        DefinirPropriedade(
            command,
            nameof(CreatePessoaCommand.Nome),
            "João da Silva");

        DefinirPropriedade(
            command,
            nameof(CreatePessoaCommand.Documento),
            documento);

        DefinirPropriedade(
            command,
            nameof(CreatePessoaCommand.Telefone),
            "11999999999");

        DefinirPropriedade(
            command,
            nameof(CreatePessoaCommand.Logradouro),
            "Rua das Flores");

        DefinirPropriedade(
            command,
            nameof(CreatePessoaCommand.Numero),
            "100");

        DefinirPropriedade(
            command,
            nameof(CreatePessoaCommand.Complemento),
            "Apto 10");

        DefinirPropriedade(
            command,
            nameof(CreatePessoaCommand.Bairro),
            "Centro");

        DefinirPropriedade(
            command,
            nameof(CreatePessoaCommand.Cidade),
            "São Paulo");

        DefinirPropriedade(
            command,
            nameof(CreatePessoaCommand.Estado),
            "SP");

        DefinirPropriedade(
            command,
            nameof(CreatePessoaCommand.Cep),
            "01001000");

        DefinirPropriedade(
            command,
            nameof(CreatePessoaCommand.UsuarioId),
            Guid.NewGuid());

        return command;
    }

    // ============================================================
    // ENTITIES
    // ============================================================

    private static Usuario CriarUsuario()
    {
        return (Usuario)
            FormatterServices.GetUninitializedObject(
                typeof(Usuario));
    }

    // ============================================================
    // REFLECTION HELPERS
    // ============================================================

    private static void DefinirPropriedade(
        object objeto,
        string nome,
        object? valor)
    {
        var property = objeto
            .GetType()
            .GetProperty(
                nome,
                BindingFlags.Instance |
                BindingFlags.Public |
                BindingFlags.NonPublic);

        if (property is null)
        {
            throw new InvalidOperationException(
                $"A propriedade '{nome}' não foi encontrada " +
                $"em '{objeto.GetType().Name}'.");
        }

        property.SetValue(
            objeto,
            valor);
    }
}