using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Ofichina.Application.Abstractions.Common;
using Ofichina.Application.Abstractions.Interfaces.Repository;
using Ofichina.Application.UseCases.Permissoes.Commands;
using Ofichina.Application.UseCases.Permissoes.Handlers;
using Ofichina.Domain.Entities;

namespace Ofichina.UnitTests.Application.UseCases.Permissoes.Handlers;

public sealed class PermissaoCommandHandlerTests
{
    [Fact]
    public async Task CreatePermissao_Deve_Criar_Permissao_Quando_Codigo_Nao_Existir()
    {
        var permissaoRepository = new Mock<IPermissaoRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();

        permissaoRepository
            .Setup(x => x.GetByCodigoAsync("usuarios.listar", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Permissao?)null);

        var handler = new CreatePermissaoCommandHandler(
            permissaoRepository.Object,
            unitOfWork.Object,
            NullLogger<CreatePermissaoCommandHandler>.Instance);

        var result = await handler.HandleAsync(new CreatePermissaoCommand("usuarios.listar", "Listar usuários"));

        Assert.True(result.IsSuccess, result.Error);
        permissaoRepository.Verify(x => x.AddAsync(It.Is<Permissao>(p => p.Codigo == "usuarios.listar" && p.Descricao == "Listar usuários"), It.IsAny<CancellationToken>()), Times.Once);
        unitOfWork.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task CreatePermissao_Deve_Recusar_Duplicidade_De_Codigo()
    {
        var permissaoRepository = new Mock<IPermissaoRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();

        permissaoRepository
            .Setup(x => x.GetByCodigoAsync("usuarios.listar", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Permissao("usuarios.listar", "Listar usuários"));

        var handler = new CreatePermissaoCommandHandler(
            permissaoRepository.Object,
            unitOfWork.Object,
            NullLogger<CreatePermissaoCommandHandler>.Instance);

        var result = await handler.HandleAsync(new CreatePermissaoCommand("usuarios.listar", "Listar usuários"));

        Assert.False(result.IsSuccess);
        Assert.Equal("Já existe uma permissão com este código.", result.Error);
        permissaoRepository.Verify(x => x.AddAsync(It.IsAny<Permissao>(), It.IsAny<CancellationToken>()), Times.Never);
        unitOfWork.Verify(x => x.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task UpdatePermissao_Deve_Atualizar_Permissao_Existente()
    {
        var permissao = new Permissao("usuarios.listar", "Listar usuários");
        var permissaoRepository = new Mock<IPermissaoRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();

        permissaoRepository
            .Setup(x => x.GetByIdAsync(permissao.Id, It.IsAny<CancellationToken>(), false))
            .ReturnsAsync(permissao);
        permissaoRepository
            .Setup(x => x.GetByCodigoAsync("usuarios.editar", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Permissao?)null);

        var handler = new UpdatePermissaoCommandHandler(
            permissaoRepository.Object,
            unitOfWork.Object,
            NullLogger<UpdatePermissaoCommandHandler>.Instance);

        var result = await handler.HandleAsync(new UpdatePermissaoCommand(permissao.Id, "usuarios.editar", "Editar usuários"));

        Assert.True(result.IsSuccess, result.Error);
        Assert.Equal("usuarios.editar", permissao.Codigo);
        Assert.Equal("Editar usuários", permissao.Descricao);
        permissaoRepository.Verify(x => x.UpdateAsync(permissao, It.IsAny<CancellationToken>()), Times.Once);
        unitOfWork.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdatePermissao_Deve_Recusar_Quando_Permissao_Nao_For_Encontrada()
    {
        var permissaoRepository = new Mock<IPermissaoRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();

        permissaoRepository
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>(), false))
            .ReturnsAsync((Permissao?)null);

        var handler = new UpdatePermissaoCommandHandler(
            permissaoRepository.Object,
            unitOfWork.Object,
            NullLogger<UpdatePermissaoCommandHandler>.Instance);

        var result = await handler.HandleAsync(new UpdatePermissaoCommand(Guid.NewGuid(), "usuarios.editar", "Editar usuários"));

        Assert.False(result.IsSuccess);
        Assert.Equal("Permissão não encontrada.", result.Error);
        permissaoRepository.Verify(x => x.UpdateAsync(It.IsAny<Permissao>(), It.IsAny<CancellationToken>()), Times.Never);
        unitOfWork.Verify(x => x.SaveChangesAsync(), Times.Never);
    }
}