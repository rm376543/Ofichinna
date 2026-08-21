using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Ofichina.Application.Abstractions.Interfaces.Repository;
using Ofichina.Application.UseCases.PerfilPermissoes.Commands;
using Ofichina.Application.UseCases.PerfilPermissoes.Handlers;
using Ofichina.Domain.Entities;

namespace Ofichina.UnitTests.Application.UseCases.PerfilPermissoes.Handlers;

public sealed class VincularPermissaoPerfilCommandHandlerTests
{
    // ============================================================  
    // SUCESSO  
    // ============================================================  

    [Fact]
    public async Task Deve_Vincular_Permissao_Ao_Perfil_Quando_Dados_Forem_Validos()
    {
        // Arrange  
        var command = CriarCommand();

        var perfilRepository = new Mock<IPerfilRepository>();
        var permissaoRepository = new Mock<IPermissaoRepository>();
        var perfilPermissaoRepository = new Mock<IPerfilPermissaoRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();

        perfilRepository
            .Setup(x => x.GetByIdAsync(
                command.PerfilId,
                It.IsAny<CancellationToken>(),
                It.IsAny<bool>()))
            .ReturnsAsync(CriarPerfil());

        permissaoRepository
            .Setup(x => x.GetByIdAsync(
                command.PermissaoId,
                It.IsAny<CancellationToken>(),
                It.IsAny<bool>()))
            .ReturnsAsync(CriarPermissao());

        perfilPermissaoRepository
            .Setup(x => x.GetByPerfilIdPermissaoIdAsync(
                command.PerfilId,
                command.PermissaoId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((PerfilPermissao?)null);

        var handler = CriarHandler(
            perfilRepository,
            permissaoRepository,
            perfilPermissaoRepository,
            unitOfWork);

        // Act  
        var result = await handler.HandleAsync(command);

        // Assert  
        Assert.True(result.IsSuccess, result.Error);

        perfilPermissaoRepository.Verify(
            x => x.AddAsync(
                It.IsAny<PerfilPermissao>(),
                It.IsAny<CancellationToken>()),
            Times.Once);

        unitOfWork.Verify(
            x => x.SaveChangesAsync(),
            Times.Once);
    }

    // ============================================================  
    // FALHA - perfil não encontrado  
    // ============================================================  

    [Fact]
    public async Task Deve_Retornar_Falha_Quando_Perfil_Nao_For_Encontrado()
    {
        // Arrange  
        var command = CriarCommand();

        var perfilRepository = new Mock<IPerfilRepository>();
        var permissaoRepository = new Mock<IPermissaoRepository>();
        var perfilPermissaoRepository = new Mock<IPerfilPermissaoRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();

        perfilRepository
            .Setup(x => x.GetByIdAsync(
                command.PerfilId,
                It.IsAny<CancellationToken>(),
                It.IsAny<bool>()))
            .ReturnsAsync((Perfil?)null);

        var handler = CriarHandler(
            perfilRepository,
            permissaoRepository,
            perfilPermissaoRepository,
            unitOfWork);

        // Act  
        var result = await handler.HandleAsync(command);

        // Assert  
        Assert.False(result.IsSuccess);
        Assert.Equal("Perfil não encontrado.", result.Error);

        perfilPermissaoRepository.Verify(
            x => x.AddAsync(It.IsAny<PerfilPermissao>(), It.IsAny<CancellationToken>()),
            Times.Never);
        unitOfWork.Verify(x => x.SaveChangesAsync(), Times.Never);
    }

    // ============================================================  
    // FALHA - permissão não encontrada  
    // ============================================================  

    [Fact]
    public async Task Deve_Retornar_Falha_Quando_Permissao_Nao_For_Encontrada()
    {
        // Arrange  
        var command = CriarCommand();

        var perfilRepository = new Mock<IPerfilRepository>();
        var permissaoRepository = new Mock<IPermissaoRepository>();
        var perfilPermissaoRepository = new Mock<IPerfilPermissaoRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();

        perfilRepository
            .Setup(x => x.GetByIdAsync(
                command.PerfilId,
                It.IsAny<CancellationToken>(),
                It.IsAny<bool>()))
            .ReturnsAsync(CriarPerfil());

        permissaoRepository
            .Setup(x => x.GetByIdAsync(
                command.PermissaoId,
                It.IsAny<CancellationToken>(),
                It.IsAny<bool>()))
            .ReturnsAsync((Permissao?)null);

        var handler = CriarHandler(
            perfilRepository,
            permissaoRepository,
            perfilPermissaoRepository,
            unitOfWork);

        // Act  
        var result = await handler.HandleAsync(command);

        // Assert  
        Assert.False(result.IsSuccess);
        Assert.Equal("Permissão não encontrada.", result.Error);

        perfilPermissaoRepository.Verify(
            x => x.AddAsync(It.IsAny<PerfilPermissao>(), It.IsAny<CancellationToken>()),
            Times.Never);
        unitOfWork.Verify(x => x.SaveChangesAsync(), Times.Never);
    }

    // ============================================================  
    // FALHA - vínculo já existente  
    // ============================================================  

    [Fact]
    public async Task Deve_Retornar_Falha_Quando_Vinculo_Ja_Existir()
    {
        // Arrange  
        var command = CriarCommand();

        var perfilRepository = new Mock<IPerfilRepository>();
        var permissaoRepository = new Mock<IPermissaoRepository>();
        var perfilPermissaoRepository = new Mock<IPerfilPermissaoRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();

        perfilRepository
            .Setup(x => x.GetByIdAsync(
                command.PerfilId,
                It.IsAny<CancellationToken>(),
                It.IsAny<bool>()))
            .ReturnsAsync(CriarPerfil());

        permissaoRepository
            .Setup(x => x.GetByIdAsync(
                command.PermissaoId,
                It.IsAny<CancellationToken>(),
                It.IsAny<bool>()))
            .ReturnsAsync(CriarPermissao());

        perfilPermissaoRepository
            .Setup(x => x.GetByPerfilIdPermissaoIdAsync(
                command.PerfilId,
                command.PermissaoId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PerfilPermissao(command.PerfilId, command.PermissaoId));

        var handler = CriarHandler(
            perfilRepository,
            permissaoRepository,
            perfilPermissaoRepository,
            unitOfWork);

        // Act  
        var result = await handler.HandleAsync(command);

        // Assert  
        Assert.False(result.IsSuccess);
        Assert.Equal("O vínculo entre perfil e permissão já existe.", result.Error);

        perfilPermissaoRepository.Verify(
            x => x.AddAsync(It.IsAny<PerfilPermissao>(), It.IsAny<CancellationToken>()),
            Times.Never);
        unitOfWork.Verify(x => x.SaveChangesAsync(), Times.Never);
    }

    // ============================================================  
    // FALHA - exceção inesperada  
    // ============================================================  

    [Fact]
    public async Task Deve_Retornar_Falha_Quando_Ocorrer_Excecao()
    {
        // Arrange  
        var command = CriarCommand();

        var perfilRepository = new Mock<IPerfilRepository>();
        var permissaoRepository = new Mock<IPermissaoRepository>();
        var perfilPermissaoRepository = new Mock<IPerfilPermissaoRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();

        perfilRepository
            .Setup(x => x.GetByIdAsync(
                command.PerfilId,
                It.IsAny<CancellationToken>(),
                It.IsAny<bool>()))
            .ThrowsAsync(new InvalidOperationException("Falha inesperada."));

        var handler = CriarHandler(
            perfilRepository,
            permissaoRepository,
            perfilPermissaoRepository,
            unitOfWork);

        // Act  
        var result = await handler.HandleAsync(command);

        // Assert  
        Assert.False(result.IsSuccess);
        Assert.Equal("Não foi possível vincular a permissão ao perfil.", result.Error);

        unitOfWork.Verify(x => x.SaveChangesAsync(), Times.Never);
    }

    // ============================================================  
    // HELPERS  
    // ============================================================  

    private static VincularPermissaoPerfilCommand CriarCommand(
        Guid? perfilId = null,
        Guid? permissaoId = null)
    {
        return new VincularPermissaoPerfilCommand(
            perfilId ?? Guid.NewGuid(),
            permissaoId ?? Guid.NewGuid());
    }

    private static VincularPermissaoPerfilCommandHandler CriarHandler(
        Mock<IPerfilRepository> perfilRepository,
        Mock<IPermissaoRepository> permissaoRepository,
        Mock<IPerfilPermissaoRepository> perfilPermissaoRepository,
        Mock<IUnitOfWork> unitOfWork)
    {
        return new VincularPermissaoPerfilCommandHandler(
            perfilRepository.Object,
            permissaoRepository.Object,
            perfilPermissaoRepository.Object,
            unitOfWork.Object,
            NullLogger<VincularPermissaoPerfilCommandHandler>.Instance);
    }

    private static Perfil CriarPerfil()
    {
        return new Perfil("Administrador", "Perfil com acesso total.");
    }

    private static Permissao CriarPermissao()
    {
        return new Permissao("USUARIO_CRIAR", "Permite criar usuários.");
    }
}