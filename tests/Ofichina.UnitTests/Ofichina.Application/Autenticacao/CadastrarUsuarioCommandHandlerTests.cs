using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Ofichina.Application.Abstractions.Authentication.Repository;
using Ofichina.Application.Abstractions.Authentication.Service;
using Ofichina.Application.Exceptions;
using Ofichina.Application.UseCases.Autenticacao.Commands;
using Ofichina.Application.UseCases.Autenticacao.Handlers;
using Ofichina.Contracts.Responses.Authentication;
using Ofichina.Domain.Entities;
using Ofichina.Domain.ValueObjects;

namespace Ofichina.UnitTests.Application.Autenticacao;

public sealed class CadastrarUsuarioCommandHandlerTests
{
    [Fact]
    public async Task Deve_Cadastrar_Usuario_E_Retornar_Token()
    {
        var usuarios = new List<Usuario>();
        var usuarioRepository = new Mock<IRepository<Usuario>>();
        var usuarioAuthRepository = new Mock<IUserAuthRepository>();
        var jwtTokenService = new Mock<IJwtTokenService>();
        var senhaHasher = new Mock<IPasswordHasherService>();
        var unitOfWork = new Mock<IUnitOfWork>();

        usuarioAuthRepository.Setup(x => x.ObterPorEmailAsync("maria@ofichinna.com", It.IsAny<CancellationToken>())).ReturnsAsync((Usuario?)null);
        senhaHasher.Setup(x => x.GerarHash("123456")).Returns("hash:123456");
        jwtTokenService.Setup(x => x.GerarTokenAsync(It.IsAny<Usuario>(), It.IsAny<IReadOnlyCollection<string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new JwtResponse { AccessToken = "fake-token", ExpiraEm = DateTime.UtcNow.AddHours(1) });

        usuarioRepository.Setup(x => x.AddAsync(It.IsAny<Usuario>(), It.IsAny<CancellationToken>()))
            .Callback<Usuario, CancellationToken>((usuario, _) => usuarios.Add(usuario));

        var handler = new CadastrarUsuarioCommandHandler(
            usuarioRepository.Object,
            unitOfWork.Object,
            usuarioAuthRepository.Object,
            jwtTokenService.Object,
            senhaHasher.Object,
            NullLogger<CadastrarUsuarioCommandHandler>.Instance);

        var result = await handler.HandleAsync(new CadastrarUsuarioCommand("maria@ofichinna.com", "123456"));

        Assert.True(result.IsSuccess, result.Error);
        Assert.NotNull(result.Value);
        Assert.Single(usuarios);
        Assert.Equal("maria@ofichinna.com", usuarios[0].Email.Value);
        Assert.Equal("hash:123456", usuarios[0].SenhaHash);
        Assert.Equal("fake-token", result.Value!.AccessToken);
        Assert.Empty(result.Value.Perfis);
        Assert.Single(usuarios);
        unitOfWork.Verify(x => x.SaveChangesAsync(), Times.Once);
        jwtTokenService.Verify(x => x.GerarTokenAsync(It.IsAny<Usuario>(), It.IsAny<IReadOnlyCollection<string>>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Deve_Falhar_Quando_Email_Ja_Existir()
    {
        var usuarioExistente = new Usuario(new Email("maria@ofichinna.com"), "hash:123456");
        var usuarioRepository = new Mock<IRepository<Usuario>>();
        var usuarioAuthRepository = new Mock<IUserAuthRepository>();
        var jwtTokenService = new Mock<IJwtTokenService>();
        var senhaHasher = new Mock<IPasswordHasherService>();
        var unitOfWork = new Mock<IUnitOfWork>();

        usuarioAuthRepository.Setup(x => x.ObterPorEmailAsync("maria@ofichinna.com", It.IsAny<CancellationToken>())).ReturnsAsync(usuarioExistente);

        var handler = new CadastrarUsuarioCommandHandler(
            usuarioRepository.Object,
            unitOfWork.Object,
            usuarioAuthRepository.Object,
            jwtTokenService.Object,
            senhaHasher.Object,
            NullLogger<CadastrarUsuarioCommandHandler>.Instance);

        var result = await handler.HandleAsync(new CadastrarUsuarioCommand("maria@ofichinna.com", "123456"));

        Assert.False(result.IsSuccess);
        Assert.Equal("Já existe um usuário cadastrado com este e-mail.", result.Error);
        usuarioRepository.Verify(x => x.AddAsync(It.IsAny<Usuario>(), It.IsAny<CancellationToken>()), Times.Never);
        unitOfWork.Verify(x => x.SaveChangesAsync(), Times.Never);
        jwtTokenService.Verify(x => x.GerarTokenAsync(It.IsAny<Usuario>(), It.IsAny<IReadOnlyCollection<string>>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Deve_Falhar_Com_Erro_Desconhecido_Quando_Email_For_Invalido()
    {
        var usuarioRepository = new Mock<IRepository<Usuario>>();
        var usuarioAuthRepository = new Mock<IUserAuthRepository>();
        var jwtTokenService = new Mock<IJwtTokenService>();
        var senhaHasher = new Mock<IPasswordHasherService>();
        var unitOfWork = new Mock<IUnitOfWork>();

        var handler = new CadastrarUsuarioCommandHandler(
            usuarioRepository.Object,
            unitOfWork.Object,
            usuarioAuthRepository.Object,
            jwtTokenService.Object,
            senhaHasher.Object,
            NullLogger<CadastrarUsuarioCommandHandler>.Instance);

        var result = await handler.HandleAsync(new CadastrarUsuarioCommand("email-invalido", "123456"));

        Assert.False(result.IsSuccess);
        Assert.Equal("Erro desconhecido. - E-mail inválido.", result.Error);
        usuarioRepository.Verify(x => x.AddAsync(It.IsAny<Usuario>(), It.IsAny<CancellationToken>()), Times.Never);
        unitOfWork.Verify(x => x.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task Deve_Falhar_Com_Mensagem_De_Negocio_Quando_Hash_Senha_For_Invalido()
    {
        var usuarioRepository = new Mock<IRepository<Usuario>>();
        var usuarioAuthRepository = new Mock<IUserAuthRepository>();
        var jwtTokenService = new Mock<IJwtTokenService>();
        var senhaHasher = new Mock<IPasswordHasherService>();
        var unitOfWork = new Mock<IUnitOfWork>();

        usuarioAuthRepository.Setup(x => x.ObterPorEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync((Usuario?)null);
        senhaHasher.Setup(x => x.GerarHash(It.IsAny<string>())).Throws(new BusinessException("Senha inválida."));

        var handler = new CadastrarUsuarioCommandHandler(
            usuarioRepository.Object,
            unitOfWork.Object,
            usuarioAuthRepository.Object,
            jwtTokenService.Object,
            senhaHasher.Object,
            NullLogger<CadastrarUsuarioCommandHandler>.Instance);

        var result = await handler.HandleAsync(new CadastrarUsuarioCommand("maria@ofichinna.com", "123456"));

        Assert.False(result.IsSuccess);
        Assert.Equal("Senha inválida.", result.Error);
        usuarioRepository.Verify(x => x.AddAsync(It.IsAny<Usuario>(), It.IsAny<CancellationToken>()), Times.Never);
        unitOfWork.Verify(x => x.SaveChangesAsync(), Times.Never);
        jwtTokenService.Verify(x => x.GerarTokenAsync(It.IsAny<Usuario>(), It.IsAny<IReadOnlyCollection<string>>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}