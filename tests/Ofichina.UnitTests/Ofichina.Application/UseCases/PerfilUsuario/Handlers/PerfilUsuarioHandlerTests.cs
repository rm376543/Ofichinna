using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Ofichina.Application.Abstractions.Authentication.Service;
using Ofichina.Application.Abstractions.Common;
using Ofichina.Application.Abstractions.Interfaces.Repository;
using Ofichina.Application.Abstractions.Interfaces.Service;
using Ofichina.Application.UseCases.PerfilUsuario.Commands;
using Ofichina.Application.UseCases.PerfilUsuario.Handlers;
using Ofichina.Application.UseCases.PerfilUsuario.Queries;
using Ofichina.Contracts.Requests.PerfilUsuario;
using Ofichina.Domain.Entities;
using Ofichina.Domain.ValueObjects;

namespace Ofichina.UnitTests.Application.UseCases.PerfilUsuario.Handlers;

public sealed class PerfilUsuarioHandlerTests
{
    [Fact]
    public async Task VincularPerfilUsuario_Deve_Retornar_Sucesso_Quando_Tudo_Estiver_Valido()
    {
        var usuario = new Usuario(new Email("usuario@ofichina.com.br"), "hash");
        var perfil = new Perfil("CONSULTOR", "Consultor");
        var usuarioRepository = new Mock<IRepository<Usuario>>();
        var perfilRepository = new Mock<IPerfilRepository>();
        var usuarioPerfilRepository = new Mock<IPerfilUsuarioRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();

        usuarioRepository.Setup(x => x.GetByIdAsync(usuario.Id, It.IsAny<CancellationToken>(), false)).ReturnsAsync(usuario);
        perfilRepository.Setup(x => x.GetByIdAsync(perfil.Id, It.IsAny<CancellationToken>(), false)).ReturnsAsync(perfil);
        usuarioPerfilRepository.Setup(x => x.GetByUsuarioIdPerfilIdAsync(usuario.Id, perfil.Id, It.IsAny<CancellationToken>())).ReturnsAsync((UsuarioPerfil?)null);

        var handler = new VincularPerfilUsuarioCommandHandler(
            usuarioRepository.Object,
            perfilRepository.Object,
            usuarioPerfilRepository.Object,
            unitOfWork.Object,
            NullLogger<VincularPerfilUsuarioCommandHandler>.Instance);

        var result = await handler.HandleAsync(new VincularPerfilUsuarioCommand(new VincularPerfilUsuarioRequest
        {
            UsuarioId = usuario.Id,
            PerfilId = perfil.Id
        }));

        Assert.True(result.IsSuccess, result.Error);
        Assert.Equal(usuario.Id, result.Value.UsuarioId);
        Assert.Equal(perfil.Id, result.Value.PerfilId);
        usuarioPerfilRepository.Verify(x => x.AddAsync(It.Is<UsuarioPerfil>(v => v.UsuarioId == usuario.Id && v.PerfilId == perfil.Id), It.IsAny<CancellationToken>()), Times.Once);
        unitOfWork.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task VincularPerfilUsuario_Deve_Recusar_Quando_Usuario_Nao_Existir()
    {
        var usuarioRepository = new Mock<IRepository<Usuario>>();
        var perfilRepository = new Mock<IPerfilRepository>();
        var usuarioPerfilRepository = new Mock<IPerfilUsuarioRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();

        usuarioRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>(), false)).ReturnsAsync((Usuario?)null);

        var handler = new VincularPerfilUsuarioCommandHandler(
            usuarioRepository.Object,
            perfilRepository.Object,
            usuarioPerfilRepository.Object,
            unitOfWork.Object,
            NullLogger<VincularPerfilUsuarioCommandHandler>.Instance);

        var result = await handler.HandleAsync(new VincularPerfilUsuarioCommand(new VincularPerfilUsuarioRequest
        {
            UsuarioId = Guid.NewGuid(),
            PerfilId = Guid.NewGuid()
        }));

        Assert.False(result.IsSuccess);
        Assert.Equal("Usuário não encontrado.", result.Error);
        usuarioPerfilRepository.Verify(x => x.AddAsync(It.IsAny<UsuarioPerfil>(), It.IsAny<CancellationToken>()), Times.Never);
        unitOfWork.Verify(x => x.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task VincularPerfilUsuario_Deve_Recusar_Quando_Perfil_Estiver_Inativo()
    {
        var usuario = new Usuario(new Email("usuario@ofichina.com.br"), "hash");
        var perfil = new Perfil("CONSULTOR", "Consultor");
        perfil.Desativar();
        var usuarioRepository = new Mock<IRepository<Usuario>>();
        var perfilRepository = new Mock<IPerfilRepository>();
        var usuarioPerfilRepository = new Mock<IPerfilUsuarioRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();

        usuarioRepository.Setup(x => x.GetByIdAsync(usuario.Id, It.IsAny<CancellationToken>(), false)).ReturnsAsync(usuario);
        perfilRepository.Setup(x => x.GetByIdAsync(perfil.Id, It.IsAny<CancellationToken>(), false)).ReturnsAsync(perfil);

        var handler = new VincularPerfilUsuarioCommandHandler(
            usuarioRepository.Object,
            perfilRepository.Object,
            usuarioPerfilRepository.Object,
            unitOfWork.Object,
            NullLogger<VincularPerfilUsuarioCommandHandler>.Instance);

        var result = await handler.HandleAsync(new VincularPerfilUsuarioCommand(new VincularPerfilUsuarioRequest
        {
            UsuarioId = usuario.Id,
            PerfilId = perfil.Id
        }));

        Assert.False(result.IsSuccess);
        Assert.Equal("Perfil inativo.", result.Error);
        usuarioPerfilRepository.Verify(x => x.AddAsync(It.IsAny<UsuarioPerfil>(), It.IsAny<CancellationToken>()), Times.Never);
        unitOfWork.Verify(x => x.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task VincularPerfilUsuario_Deve_Recusar_Quando_Vinculo_Ja_Existir()
    {
        var usuario = new Usuario(new Email("usuario@ofichina.com.br"), "hash");
        var perfil = new Perfil("CONSULTOR", "Consultor");
        var usuarioRepository = new Mock<IRepository<Usuario>>();
        var perfilRepository = new Mock<IPerfilRepository>();
        var usuarioPerfilRepository = new Mock<IPerfilUsuarioRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();

        usuarioRepository.Setup(x => x.GetByIdAsync(usuario.Id, It.IsAny<CancellationToken>(), false)).ReturnsAsync(usuario);
        perfilRepository.Setup(x => x.GetByIdAsync(perfil.Id, It.IsAny<CancellationToken>(), false)).ReturnsAsync(perfil);
        usuarioPerfilRepository.Setup(x => x.GetByUsuarioIdPerfilIdAsync(usuario.Id, perfil.Id, It.IsAny<CancellationToken>())).ReturnsAsync(new UsuarioPerfil(usuario.Id, perfil.Id));

        var handler = new VincularPerfilUsuarioCommandHandler(
            usuarioRepository.Object,
            perfilRepository.Object,
            usuarioPerfilRepository.Object,
            unitOfWork.Object,
            NullLogger<VincularPerfilUsuarioCommandHandler>.Instance);

        var result = await handler.HandleAsync(new VincularPerfilUsuarioCommand(new VincularPerfilUsuarioRequest
        {
            UsuarioId = usuario.Id,
            PerfilId = perfil.Id
        }));

        Assert.False(result.IsSuccess);
        Assert.Equal("O vínculo entre usuário e perfil já existe.", result.Error);
        usuarioPerfilRepository.Verify(x => x.AddAsync(It.IsAny<UsuarioPerfil>(), It.IsAny<CancellationToken>()), Times.Never);
        unitOfWork.Verify(x => x.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task DesvincularPerfilUsuario_Deve_Desvincular_Quando_Vinculo_Existir()
    {
        var vinculo = new UsuarioPerfil(Guid.NewGuid(), Guid.NewGuid());
        var usuarioPerfilRepository = new Mock<IPerfilUsuarioRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();

        usuarioPerfilRepository
            .Setup(x => x.GetByUsuarioIdPerfilIdAsync(vinculo.UsuarioId, vinculo.PerfilId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(vinculo);

        var handler = new DesvincularPerfilUsuarioCommandHandler(
            usuarioPerfilRepository.Object,
            unitOfWork.Object,
            NullLogger<DesvincularPerfilUsuarioCommandHandler>.Instance);

        var result = await handler.HandleAsync(new DesvincularPerfilUsuarioCommand(vinculo.UsuarioId, vinculo.PerfilId));

        Assert.True(result.IsSuccess, result.Error);
        Assert.Equal(vinculo.UsuarioId, result.Value.UsuarioId);
        Assert.Equal(vinculo.PerfilId, result.Value.PerfilId);
        usuarioPerfilRepository.Verify(x => x.DeleteAsync(vinculo, It.IsAny<CancellationToken>()), Times.Once);
        unitOfWork.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DesvincularPerfilUsuario_Deve_Recusar_Quando_Vinculo_Nao_Existir()
    {
        var usuarioPerfilRepository = new Mock<IPerfilUsuarioRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();

        usuarioPerfilRepository
            .Setup(x => x.GetByUsuarioIdPerfilIdAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((UsuarioPerfil?)null);

        var handler = new DesvincularPerfilUsuarioCommandHandler(
            usuarioPerfilRepository.Object,
            unitOfWork.Object,
            NullLogger<DesvincularPerfilUsuarioCommandHandler>.Instance);

        var result = await handler.HandleAsync(new DesvincularPerfilUsuarioCommand(Guid.NewGuid(), Guid.NewGuid()));

        Assert.False(result.IsSuccess);
        Assert.Equal("Vínculo entre usuário e perfil não encontrado.", result.Error);
        usuarioPerfilRepository.Verify(x => x.DeleteAsync(It.IsAny<UsuarioPerfil>(), It.IsAny<CancellationToken>()), Times.Never);
        unitOfWork.Verify(x => x.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task ObterPerfisDoUsuarioQueryHandler_Deve_Retornar_Perfis_Do_Usuario()
    {
        var perfilService = new Mock<IProfileAuthService>();
        var usuarioId = Guid.NewGuid();

        perfilService
            .Setup(x => x.ObterPerfisAsync(usuarioId))
            .ReturnsAsync((IReadOnlyCollection<string>)["ADMIN", "CONSULTOR"]);

        var handler = new ObterPerfisDoUsuarioQueryHandler(perfilService.Object);

        var result = await handler.HandleAsync(new ObterPerfisDoUsuarioQuery(usuarioId));

        Assert.Equal(["ADMIN", "CONSULTOR"], result);
        perfilService.Verify(x => x.ObterPerfisAsync(usuarioId), Times.Once);
    }
}