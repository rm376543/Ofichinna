using Microsoft.Extensions.Logging.Abstractions;
using Ofichina.Application.Abstractions.Authentication.Service;
using Ofichina.Application.Abstractions.Interfaces.Repository;
using Ofichina.Application.UseCases.OrdensServico.Commands;
using Ofichina.Application.UseCases.OrdensServico.Handlers;
using Ofichina.Contracts.Common;
using Ofichina.Domain.Aggregates;
using Ofichina.Domain.Entities;
using Ofichina.Domain.Enums;
using System.Reflection;
using Moq;
using Ofichina.UnitTests.Fixtures;

namespace Ofichina.UnitTests.Application.UseCases.OrdensServico.Handlers;

public sealed class AlterarStatusOrdemServicoCommandHandlerTests
{
    [Fact]
    public async Task Finalizar_Deve_Carregar_Itens_E_Concluir_A_Ordem_De_Servico()
    {
            var ordemServico = TestFakes.CriarOrdemServicoEmExecucaoComItem();

            var ordemServicoRepositoryMock = new Mock<IOrdemServicoRepository>();
            ordemServicoRepositoryMock
                .Setup(r => r.GetByIdAsync(It.Is<Guid>(g => g == ordemServico.Id), It.IsAny<bool>(), It.IsAny<CancellationToken>(), It.IsAny<bool>()))
                .ReturnsAsync(ordemServico);

            var historicoStatusRepositoryMock = new Mock<IRepository<HistoricoStatus>>();
            historicoStatusRepositoryMock
                .Setup(r => r.AddAsync(It.IsAny<HistoricoStatus>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var usuarioAtualServiceMock = new Mock<IUserService>();
            usuarioAtualServiceMock.Setup(s => s.ObterUsuarioId()).Returns(Guid.NewGuid());

            var unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            var handler = new AlterarStatusOrdemServicoCommandHandler(
                ordemServicoRepositoryMock.Object,
                historicoStatusRepositoryMock.Object,
                usuarioAtualServiceMock.Object,
                unitOfWorkMock.Object,
                NullLogger<AlterarStatusOrdemServicoCommandHandler>.Instance);

        var result = await handler.HandleAsync(
            new AlterarStatusOrdemServicoCommand(ordemServico.Id, "Finalizada"));

            Assert.True(result.IsSuccess, result.Error);
            ordemServicoRepositoryMock.Verify(r => r.GetByIdAsync(It.Is<Guid>(g => g == ordemServico.Id), true, It.IsAny<CancellationToken>(), It.IsAny<bool>()), Times.AtLeastOnce());
            Assert.Equal(StatusOrdemServico.Finalizada, ordemServico.Status);
            Assert.NotNull(ordemServico.DataFinalizacao);
            historicoStatusRepositoryMock.Verify(r => r.AddAsync(It.IsAny<HistoricoStatus>(), It.IsAny<CancellationToken>()), Times.Exactly(1));
            unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Exactly(1));
    }

    [Fact]
        public async Task Finalizar_Deve_Falhar_Quando_Nao_Houver_Itens_Ativos_Na_Ordem()
    {
            var ordemServico = TestFakes.CriarOrdemServicoEmExecucaoSemItens();

            var ordemServicoRepositoryMock = new Mock<IOrdemServicoRepository>();
            ordemServicoRepositoryMock
                .Setup(r => r.GetByIdAsync(It.Is<Guid>(g => g == ordemServico.Id), It.IsAny<bool>(), It.IsAny<CancellationToken>(), It.IsAny<bool>()))
                .ReturnsAsync(ordemServico);

            var historicoStatusRepositoryMock = new Mock<IRepository<HistoricoStatus>>();
            historicoStatusRepositoryMock
                .Setup(r => r.AddAsync(It.IsAny<HistoricoStatus>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var usuarioAtualServiceMock = new Mock<IUserService>();
            usuarioAtualServiceMock.Setup(s => s.ObterUsuarioId()).Returns(Guid.NewGuid());

            var unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            var handler = new AlterarStatusOrdemServicoCommandHandler(
                ordemServicoRepositoryMock.Object,
                historicoStatusRepositoryMock.Object,
                usuarioAtualServiceMock.Object,
                unitOfWorkMock.Object,
                NullLogger<AlterarStatusOrdemServicoCommandHandler>.Instance);

            var result = await handler.HandleAsync(
                new AlterarStatusOrdemServicoCommand(ordemServico.Id, "Finalizada"));

            Assert.False(result.IsSuccess);
            Assert.Equal("A ordem de serviço precisa possuir itens cadastrados.", result.Error);
            ordemServicoRepositoryMock.Verify(r => r.GetByIdAsync(It.Is<Guid>(g => g == ordemServico.Id), true, It.IsAny<CancellationToken>(), It.IsAny<bool>()), Times.AtLeastOnce());
            historicoStatusRepositoryMock.Verify(r => r.AddAsync(It.IsAny<HistoricoStatus>(), It.IsAny<CancellationToken>()), Times.Never());
            unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never());
    }

    }

        public Task<OrdemServico?> GetByIdAsync(Guid id, bool includeItens = false, bool tracking = false, CancellationToken cancellationToken = default)
        {
            IncludeItensRecebido = includeItens;
            return Task.FromResult(_ordemServico is not null && _ordemServico.Id == id ? _ordemServico : null);
        }

}
