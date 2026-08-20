using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Ofichina.Application.Abstractions.Common;
using Ofichina.Application.Abstractions.Interfaces.Repository;
using Ofichina.Application.UseCases.Perfis.Commands;
using Ofichina.Application.UseCases.Perfis.Handlers;
using Ofichina.Application.UseCases.Perfis.Queries;
using Ofichina.Domain.Entities;

namespace Ofichina.UnitTests.Application.UseCases.Perfis.Handlers;

public sealed class PerfilCommandHandlerTests
{
    [Fact]
    public async Task CreatePerfil_Deve_Criar_Perfil_Quando_Nao_Existir_Registro_Com_Mesmo_Nome()
    {
        var perfilRepository = new Mock<IPerfilRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();

        perfilRepository
            .Setup(x => x.GetByNomeAsync("ADMIN", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Perfil?)null);

        var handler = new CreatePerfilCommandHandler(
            perfilRepository.Object,
            unitOfWork.Object,
            NullLogger<CreatePerfilCommandHandler>.Instance);

        var result = await handler.HandleAsync(new CreatePerfilCommand("ADMIN", "Administrador"));

        Assert.True(result.IsSuccess, result.Error);
        perfilRepository.Verify(x => x.AddAsync(It.Is<Perfil>(p => p.NomePerfil == "ADMIN" && p.Descricao == "Administrador"), It.IsAny<CancellationToken>()), Times.Once);
        unitOfWork.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task CreatePerfil_Deve_Recusar_Duplicidade_De_Nome()
    {
        var perfilRepository = new Mock<IPerfilRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();

        perfilRepository
            .Setup(x => x.GetByNomeAsync("ADMIN", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Perfil("ADMIN", "Administrador"));

        var handler = new CreatePerfilCommandHandler(
            perfilRepository.Object,
            unitOfWork.Object,
            NullLogger<CreatePerfilCommandHandler>.Instance);

        var result = await handler.HandleAsync(new CreatePerfilCommand("ADMIN", "Administrador"));

        Assert.False(result.IsSuccess);
        Assert.Equal("Já existe um perfil com este nome.", result.Error);
        perfilRepository.Verify(x => x.AddAsync(It.IsAny<Perfil>(), It.IsAny<CancellationToken>()), Times.Never);
        unitOfWork.Verify(x => x.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task UpdatePerfil_Deve_Atualizar_Descricao_E_Nome_Quando_Perfil_Existir()
    {
        var perfil = new Perfil("ADMIN", "Administrador");
        var perfilRepository = new Mock<IPerfilRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();

        perfilRepository
            .Setup(x => x.GetByIdAsync(perfil.Id, It.IsAny<CancellationToken>(), false))
            .ReturnsAsync(perfil);
        perfilRepository
            .Setup(x => x.GetByNomeAsync("GESTOR", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Perfil?)null);

        var handler = new UpdatePerfilCommandHandler(
            perfilRepository.Object,
            unitOfWork.Object,
            NullLogger<UpdatePerfilCommandHandler>.Instance);

        var result = await handler.HandleAsync(new UpdatePerfilCommand(perfil.Id, "GESTOR", "Gerente da oficina"));

        Assert.True(result.IsSuccess, result.Error);
        Assert.Equal("GESTOR", perfil.NomePerfil);
        Assert.Equal("Gerente da oficina", perfil.Descricao);
        perfilRepository.Verify(x => x.UpdateAsync(perfil, It.IsAny<CancellationToken>()), Times.Once);
        unitOfWork.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdatePerfil_Deve_Recusar_Quando_Perfil_Nao_For_Encontrado()
    {
        var perfilRepository = new Mock<IPerfilRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();

        perfilRepository
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>(), false))
            .ReturnsAsync((Perfil?)null);

        var handler = new UpdatePerfilCommandHandler(
            perfilRepository.Object,
            unitOfWork.Object,
            NullLogger<UpdatePerfilCommandHandler>.Instance);

        var result = await handler.HandleAsync(new UpdatePerfilCommand(Guid.NewGuid(), "GESTOR", "Gerente da oficina"));

        Assert.False(result.IsSuccess);
        Assert.Equal("Perfil não encontrado.", result.Error);
        perfilRepository.Verify(x => x.UpdateAsync(It.IsAny<Perfil>(), It.IsAny<CancellationToken>()), Times.Never);
        unitOfWork.Verify(x => x.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task GetPerfilById_Deve_Retornar_Perfil_Quando_Encontrado()
    {
        var perfil = new Perfil("ADMIN", "Administrador");
        var perfilRepository = new Mock<IPerfilRepository>();

        perfilRepository
            .Setup(x => x.GetByIdAsync(perfil.Id, It.IsAny<CancellationToken>(), false))
            .ReturnsAsync(perfil);

        var handler = new GetPerfilByIdQueryHandler(perfilRepository.Object, NullLogger<GetPerfilByIdQueryHandler>.Instance);

        var result = await handler.HandleAsync(new GetPerfilByIdQuery(perfil.Id));

        Assert.True(result.IsSuccess, result.Error);
        Assert.Equal(perfil.Id, result.Value.PerfilId);
        Assert.Equal("ADMIN", result.Value.Nome);
        Assert.Equal("Administrador", result.Value.Descricao);
    }

    [Fact]
    public async Task GetPerfilById_Deve_Recusar_Quando_Perfil_Nao_For_Encontrado()
    {
        var perfilRepository = new Mock<IPerfilRepository>();

        perfilRepository
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>(), false))
            .ReturnsAsync((Perfil?)null);

        var handler = new GetPerfilByIdQueryHandler(perfilRepository.Object, NullLogger<GetPerfilByIdQueryHandler>.Instance);

        var result = await handler.HandleAsync(new GetPerfilByIdQuery(Guid.NewGuid()));

        Assert.False(result.IsSuccess);
        Assert.Equal("Perfil não encontrado.", result.Error);
    }
}