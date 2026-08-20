using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Ofichina.Application.Abstractions.Interfaces.Repository;
using Ofichina.Application.UseCases.ItensServico.Commands;
using Ofichina.Application.UseCases.ItensServico.Handlers;
using Ofichina.Contracts.Requests.ItensServico;
using Ofichina.Domain.Aggregates;
using Ofichina.Domain.Entities;
using Ofichina.Domain.Enums;

namespace Ofichina.UnitTests.Application.Orcamentos;

public sealed class UpdateItemOrcamentoCommandHandlerTests
{
    private static Orcamento CriarOrcamento(StatusOrcamento status = StatusOrcamento.EmDiagnostico)
    {
        var orcamento = new Orcamento(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTime.UtcNow.AddDays(7),
            0m,
            "Observação de teste");

        if (status == StatusOrcamento.EmDiagnostico)
            orcamento.IniciarDiagnostico();

        return orcamento;
    }

    private static Servico CriarServico() => new("Troca de óleo", "Descrição", 100m);

    private static Peca CriarPeca() => new("Filtro de óleo", "Descrição", "COD-001", 50m, 10);

    private static UpdateItemOrcamentoCommand CriarCommand(
        Guid orcamentoId,
        Guid itemServicoId,
        Guid servicoId,
        Guid? pecaId,
        int quantidade)
    {
        var request = new UpdateItemOrcamentoRequest
        {
            OrcamentoId = orcamentoId,
            ItemServicoId = itemServicoId,
            ServicoId = servicoId,
            PecaId = pecaId,
            Quantidade = quantidade
        };

        return new UpdateItemOrcamentoCommand(request);
    }

    private static (
        Mock<IOrcamentoRepository> orcamentoRepository,
        Mock<IItemServicoRepository> itemServicoRepository,
        Mock<IRepository<Servico>> servicoRepository,
        Mock<IRepository<Peca>> pecaRepository,
        Mock<IUnitOfWork> unitOfWork,
        UpdateItemOrcamentoCommandHandler handler) CriarHandler()
    {
        var orcamentoRepository = new Mock<IOrcamentoRepository>();
        var itemServicoRepository = new Mock<IItemServicoRepository>();
        var servicoRepository = new Mock<IRepository<Servico>>();
        var pecaRepository = new Mock<IRepository<Peca>>();
        var unitOfWork = new Mock<IUnitOfWork>();

        var handler = new UpdateItemOrcamentoCommandHandler(
            orcamentoRepository.Object,
            itemServicoRepository.Object,
            servicoRepository.Object,
            pecaRepository.Object,
            unitOfWork.Object,
            NullLogger<UpdateItemOrcamentoCommandHandler>.Instance);

        return (orcamentoRepository, itemServicoRepository, servicoRepository, pecaRepository, unitOfWork, handler);
    }

    [Fact]
    public async Task HandleAsync_Deve_Recusar_Quando_Item_Nao_For_Encontrado()
    {
        var (orcamentoRepository, itemServicoRepository, servicoRepository, pecaRepository, unitOfWork, handler) = CriarHandler();

        var orcamentoId = Guid.NewGuid();
        var itemServicoId = Guid.NewGuid();
        var servicoId = Guid.NewGuid();

        itemServicoRepository
            .Setup(x => x.GetByOrcamentoIdAndItemServicoIdAsync(orcamentoId, itemServicoId, It.IsAny<CancellationToken>(), false, false))
            .ReturnsAsync((ItemServico?)null);

        var command = CriarCommand(orcamentoId, itemServicoId, servicoId, null, 1);

        var result = await handler.HandleAsync(command);

        Assert.False(result.IsSuccess);
        Assert.Equal("Item de serviço não encontrado.", result.Error);
        unitOfWork.Verify(x => x.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task HandleAsync_Deve_Recusar_Quando_Item_Estiver_Excluido()
    {
        var (orcamentoRepository, itemServicoRepository, servicoRepository, pecaRepository, unitOfWork, handler) = CriarHandler();

        var orcamentoId = Guid.NewGuid();
        var servicoId = Guid.NewGuid();
        var item = ItemServico.ParaOrcamento(orcamentoId, Guid.NewGuid(), null, 1);
        item.Excluir();

        itemServicoRepository
            .Setup(x => x.GetByOrcamentoIdAndItemServicoIdAsync(orcamentoId, item.Id, It.IsAny<CancellationToken>(), false, false))
            .ReturnsAsync(item);

        var command = CriarCommand(orcamentoId, item.Id, servicoId, null, 1);

        var result = await handler.HandleAsync(command);

        Assert.False(result.IsSuccess);
        Assert.Equal("Item de serviço não encontrado.", result.Error);
    }

    [Fact]
    public async Task HandleAsync_Deve_Recusar_Quando_Orcamento_Nao_For_Encontrado()
    {
        var (orcamentoRepository, itemServicoRepository, servicoRepository, pecaRepository, unitOfWork, handler) = CriarHandler();

        var orcamentoId = Guid.NewGuid();
        var servicoId = Guid.NewGuid();
        var item = ItemServico.ParaOrcamento(orcamentoId, Guid.NewGuid(), null, 1);

        itemServicoRepository
            .Setup(x => x.GetByOrcamentoIdAndItemServicoIdAsync(orcamentoId, item.Id, It.IsAny<CancellationToken>(), true, true))
            .ReturnsAsync(item);

        orcamentoRepository
            .Setup(x => x.GetByIdAsync(orcamentoId, true, It.IsAny<CancellationToken>(), true))
            .ReturnsAsync((Orcamento?)null);

        var command = CriarCommand(orcamentoId, item.Id, servicoId, null, 1);

        var result = await handler.HandleAsync(command);

        Assert.False(result.IsSuccess);
        Assert.Equal("Orçamento não encontrado.", result.Error);
    }

    [Fact]
    public async Task HandleAsync_Deve_Recusar_Quando_Orcamento_Estiver_Excluido()
    {
        var (orcamentoRepository, itemServicoRepository, servicoRepository, pecaRepository, unitOfWork, handler) = CriarHandler();

        var orcamento = CriarOrcamento();
        orcamento.Excluir();
        var servicoId = Guid.NewGuid();
        var item = ItemServico.ParaOrcamento(orcamento.Id, Guid.NewGuid(), null, 1);

        itemServicoRepository
            .Setup(x => x.GetByOrcamentoIdAndItemServicoIdAsync(orcamento.Id, item.Id, It.IsAny<CancellationToken>(), true, true))
            .ReturnsAsync(item);

        orcamentoRepository
            .Setup(x => x.GetByIdAsync(orcamento.Id, true, It.IsAny<CancellationToken>(), true))
            .ReturnsAsync(orcamento);

        var command = CriarCommand(orcamento.Id, item.Id, servicoId, null, 1);

        var result = await handler.HandleAsync(command);

        Assert.False(result.IsSuccess);
        Assert.Equal("Orçamento não encontrado.", result.Error);
    }

    [Fact]
    public async Task HandleAsync_Deve_Recusar_Quando_Orcamento_Nao_Estiver_Em_Diagnostico()
    {
        var (orcamentoRepository, itemServicoRepository, servicoRepository, pecaRepository, unitOfWork, handler) = CriarHandler();

        var orcamento = CriarOrcamento(StatusOrcamento.Criado);
        var servicoId = Guid.NewGuid();
        var item = ItemServico.ParaOrcamento(orcamento.Id, Guid.NewGuid(), null, 1);

        itemServicoRepository
            .Setup(x => x.GetByOrcamentoIdAndItemServicoIdAsync(orcamento.Id, item.Id, It.IsAny<CancellationToken>(), true, true))
            .ReturnsAsync(item);

        orcamentoRepository
            .Setup(x => x.GetByIdAsync(orcamento.Id, true, It.IsAny<CancellationToken>(), true))
            .ReturnsAsync(orcamento);

        var command = CriarCommand(orcamento.Id, item.Id, servicoId, null, 1);

        var result = await handler.HandleAsync(command);

        Assert.False(result.IsSuccess);
        Assert.Equal("Não é possível alterar itens nesta etapa do orçamento.", result.Error);
    }

    [Fact]
    public async Task HandleAsync_Deve_Recusar_Quando_Servico_Nao_For_Encontrado()
    {
        var (orcamentoRepository, itemServicoRepository, servicoRepository, pecaRepository, unitOfWork, handler) = CriarHandler();

        var orcamento = CriarOrcamento();
        var servicoId = Guid.NewGuid();
        var item = ItemServico.ParaOrcamento(orcamento.Id, Guid.NewGuid(), null, 1);

        itemServicoRepository
            .Setup(x => x.GetByOrcamentoIdAndItemServicoIdAsync(orcamento.Id, item.Id, It.IsAny<CancellationToken>(), true, true))
            .ReturnsAsync(item);

        orcamentoRepository
            .Setup(x => x.GetByIdAsync(orcamento.Id, true, It.IsAny<CancellationToken>(), true))
            .ReturnsAsync(orcamento);

        servicoRepository
            .Setup(x => x.GetByIdAsync(servicoId, It.IsAny<CancellationToken>(), true))
            .ReturnsAsync((Servico?)null);

        var command = CriarCommand(orcamento.Id, item.Id, servicoId, null, 1);

        var result = await handler.HandleAsync(command);

        Assert.False(result.IsSuccess);
        Assert.Equal("Serviço não encontrado.", result.Error);
    }

    [Fact]
    public async Task HandleAsync_Deve_Recusar_Quando_Servico_Estiver_Excluido()
    {
        var (orcamentoRepository, itemServicoRepository, servicoRepository, pecaRepository, unitOfWork, handler) = CriarHandler();

        var orcamento = CriarOrcamento();
        var servicoId = Guid.NewGuid();
        var item = ItemServico.ParaOrcamento(orcamento.Id, Guid.NewGuid(), null, 1);
        var servico = CriarServico();
        servico.Desativar();

        itemServicoRepository
            .Setup(x => x.GetByOrcamentoIdAndItemServicoIdAsync(orcamento.Id, item.Id, It.IsAny<CancellationToken>(), true, true))
            .ReturnsAsync(item);

        orcamentoRepository
            .Setup(x => x.GetByIdAsync(orcamento.Id, true, It.IsAny<CancellationToken>(), true))
            .ReturnsAsync(orcamento);

        servicoRepository
            .Setup(x => x.GetByIdAsync(servicoId, It.IsAny<CancellationToken>(), true))
            .ReturnsAsync(servico);

        var command = CriarCommand(orcamento.Id, item.Id, servicoId, null, 1);

        var result = await handler.HandleAsync(command);

        Assert.False(result.IsSuccess);
        Assert.Equal("Serviço não encontrado.", result.Error);
    }

    [Fact]
    public async Task HandleAsync_Deve_Recusar_Quando_Peca_Informada_Nao_For_Encontrada()
    {
        var (orcamentoRepository, itemServicoRepository, servicoRepository, pecaRepository, unitOfWork, handler) = CriarHandler();

        var orcamento = CriarOrcamento();
        var servicoId = Guid.NewGuid();
        var pecaId = Guid.NewGuid();
        var item = ItemServico.ParaOrcamento(orcamento.Id, Guid.NewGuid(), null, 1);
        var servico = CriarServico();

        itemServicoRepository
            .Setup(x => x.GetByOrcamentoIdAndItemServicoIdAsync(orcamento.Id, item.Id, It.IsAny<CancellationToken>(), true, true))
            .ReturnsAsync(item);

        orcamentoRepository
            .Setup(x => x.GetByIdAsync(orcamento.Id, true, It.IsAny<CancellationToken>(), true))
            .ReturnsAsync(orcamento);

        servicoRepository
            .Setup(x => x.GetByIdAsync(servicoId, It.IsAny<CancellationToken>(), true))
            .ReturnsAsync(servico);

        pecaRepository
            .Setup(x => x.GetByIdAsync(pecaId, It.IsAny<CancellationToken>(), true))
            .ReturnsAsync((Peca?)null);

        var command = CriarCommand(orcamento.Id, item.Id, servicoId, pecaId, 1);

        var result = await handler.HandleAsync(command);

        Assert.False(result.IsSuccess);
        Assert.Equal("Peça não encontrada.", result.Error);
    }

    [Fact]
    public async Task HandleAsync_Deve_Recusar_Quando_Peca_Informada_Estiver_Excluida()
    {
        var (orcamentoRepository, itemServicoRepository, servicoRepository, pecaRepository, unitOfWork, handler) = CriarHandler();

        var orcamento = CriarOrcamento();
        var servicoId = Guid.NewGuid();
        var item = ItemServico.ParaOrcamento(orcamento.Id, Guid.NewGuid(), null, 1);
        var servico = CriarServico();
        var peca = CriarPeca();
        peca.ExcluirLogicamente();

        itemServicoRepository
            .Setup(x => x.GetByOrcamentoIdAndItemServicoIdAsync(orcamento.Id, item.Id, It.IsAny<CancellationToken>(), true, true))
            .ReturnsAsync(item);

        orcamentoRepository
            .Setup(x => x.GetByIdAsync(orcamento.Id, true, It.IsAny<CancellationToken>(), true))
            .ReturnsAsync(orcamento);

        servicoRepository
            .Setup(x => x.GetByIdAsync(servicoId, It.IsAny<CancellationToken>(), true))
            .ReturnsAsync(servico);

        pecaRepository
            .Setup(x => x.GetByIdAsync(peca.Id, It.IsAny<CancellationToken>(), true))
            .ReturnsAsync(peca);

        var command = CriarCommand(orcamento.Id, item.Id, servicoId, peca.Id, 1);

        var result = await handler.HandleAsync(command);

        Assert.False(result.IsSuccess);
        Assert.Equal("Peça não encontrada.", result.Error);
    }

    [Fact]
    public async Task HandleAsync_Deve_Recusar_Quando_Existir_Duplicado_Com_Outro_Id()
    {
        var (orcamentoRepository, itemServicoRepository, servicoRepository, pecaRepository, unitOfWork, handler) = CriarHandler();

        var orcamento = CriarOrcamento();
        var servicoId = Guid.NewGuid();
        var item = ItemServico.ParaOrcamento(orcamento.Id, Guid.NewGuid(), null, 1);
        var servico = CriarServico();
        var duplicado = ItemServico.ParaOrcamento(orcamento.Id, servicoId, null, 1);

        itemServicoRepository
            .Setup(x => x.GetByOrcamentoIdAndItemServicoIdAsync(orcamento.Id, item.Id, It.IsAny<CancellationToken>(), true, true))
            .ReturnsAsync(item);

        orcamentoRepository
            .Setup(x => x.GetByIdAsync(orcamento.Id, true, It.IsAny<CancellationToken>(), true))
            .ReturnsAsync(orcamento);

        servicoRepository
            .Setup(x => x.GetByIdAsync(servicoId, It.IsAny<CancellationToken>(), true))
            .ReturnsAsync(servico);

        itemServicoRepository
            .Setup(x => x.GetByOrcamentoServicoPecaIdAsync(orcamento.Id, servicoId, (Guid?)null, It.IsAny<CancellationToken>(), true))
            .ReturnsAsync(duplicado);

        var command = CriarCommand(orcamento.Id, item.Id, servicoId, null, 1);

        var result = await handler.HandleAsync(command);

        Assert.False(result.IsSuccess);
        Assert.Equal("Já existe um item de serviço com este serviço e esta peça no orçamento.", result.Error);
        unitOfWork.Verify(x => x.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task HandleAsync_Deve_Permitir_Quando_Duplicado_For_O_Proprio_Item()
    {
        var (orcamentoRepository, itemServicoRepository, servicoRepository, pecaRepository, unitOfWork, handler) = CriarHandler();

        var orcamento = CriarOrcamento();
        var servicoId = Guid.NewGuid();
        var item = ItemServico.ParaOrcamento(orcamento.Id, Guid.NewGuid(), null, 1);
        var servico = CriarServico();

        itemServicoRepository
            .Setup(x => x.GetByOrcamentoIdAndItemServicoIdAsync(orcamento.Id, item.Id, It.IsAny<CancellationToken>(), true, true))
            .ReturnsAsync(item);

        orcamentoRepository
            .Setup(x => x.GetByIdAsync(orcamento.Id, true, It.IsAny<CancellationToken>(), true))
            .ReturnsAsync(orcamento);

        servicoRepository
            .Setup(x => x.GetByIdAsync(servicoId, It.IsAny<CancellationToken>(), true))
            .ReturnsAsync(servico);

        // duplicado retornado é o próprio item (mesmo Id)
        itemServicoRepository
            .Setup(x => x.GetByOrcamentoServicoPecaIdAsync(orcamento.Id, servicoId, (Guid?)null, It.IsAny<CancellationToken>(), true))
            .ReturnsAsync(item);

        var command = CriarCommand(orcamento.Id, item.Id, servicoId, null, 2);

        var result = await handler.HandleAsync(command);

        Assert.True(result.IsSuccess, result.Error);
        Assert.Equal(servicoId, item.ServicoId);
        Assert.Equal(2, item.Quantidade);
        unitOfWork.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_Deve_Permitir_Quando_Duplicado_Estiver_Excluido()
    {
        var (orcamentoRepository, itemServicoRepository, servicoRepository, pecaRepository, unitOfWork, handler) = CriarHandler();

        var orcamento = CriarOrcamento();
        var servicoId = Guid.NewGuid();
        var item = ItemServico.ParaOrcamento(orcamento.Id, Guid.NewGuid(), null, 1);
        var servico = CriarServico();
        var duplicado = ItemServico.ParaOrcamento(orcamento.Id, servicoId, null, 1);
        duplicado.Excluir();

        itemServicoRepository
            .Setup(x => x.GetByOrcamentoIdAndItemServicoIdAsync(orcamento.Id, item.Id, It.IsAny<CancellationToken>(), true, true))
            .ReturnsAsync(item);

        orcamentoRepository
            .Setup(x => x.GetByIdAsync(orcamento.Id, true, It.IsAny<CancellationToken>(), true))
            .ReturnsAsync(orcamento);

        servicoRepository
            .Setup(x => x.GetByIdAsync(servicoId, It.IsAny<CancellationToken>(), true))
            .ReturnsAsync(servico);

        itemServicoRepository
            .Setup(x => x.GetByOrcamentoServicoPecaIdAsync(orcamento.Id, servicoId, (Guid?)null, It.IsAny<CancellationToken>(), true))
            .ReturnsAsync(duplicado);

        var command = CriarCommand(orcamento.Id, item.Id, servicoId, null, 1);

        var result = await handler.HandleAsync(command);

        Assert.True(result.IsSuccess, result.Error);
        unitOfWork.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_Deve_Atualizar_Item_Com_Sucesso_Com_Peca()
    {
        var (orcamentoRepository, itemServicoRepository, servicoRepository, pecaRepository, unitOfWork, handler) = CriarHandler();

        var orcamento = CriarOrcamento();
        var servicoId = Guid.NewGuid();
        var item = ItemServico.ParaOrcamento(orcamento.Id, Guid.NewGuid(), null, 1);
        var servico = CriarServico();
        var peca = CriarPeca();

        itemServicoRepository
            .Setup(x => x.GetByOrcamentoIdAndItemServicoIdAsync(orcamento.Id, item.Id, It.IsAny<CancellationToken>(), true, true))
            .ReturnsAsync(item);

        orcamentoRepository
            .Setup(x => x.GetByIdAsync(orcamento.Id, true, It.IsAny<CancellationToken>(), true))
            .ReturnsAsync(orcamento);

        servicoRepository
            .Setup(x => x.GetByIdAsync(servicoId, It.IsAny<CancellationToken>(), true))
            .ReturnsAsync(servico);

        pecaRepository
            .Setup(x => x.GetByIdAsync(peca.Id, It.IsAny<CancellationToken>(), true))
            .ReturnsAsync(peca);

        itemServicoRepository
            .Setup(x => x.GetByOrcamentoServicoPecaIdAsync(orcamento.Id, servicoId, peca.Id, It.IsAny<CancellationToken>(), true))
            .ReturnsAsync((ItemServico?)null);

        var command = CriarCommand(orcamento.Id, item.Id, servicoId, peca.Id, 3);

        var result = await handler.HandleAsync(command);

        Assert.True(result.IsSuccess, result.Error);
        Assert.Equal(servicoId, item.ServicoId);
        Assert.Equal(peca.Id, item.PecaId);
        Assert.Equal(3, item.Quantidade);
        unitOfWork.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_Deve_Retornar_Falha_Quando_AtualizarDados_Lancar_DomainException()
    {
        var (orcamentoRepository, itemServicoRepository, servicoRepository, pecaRepository, unitOfWork, handler) = CriarHandler();

        var orcamento = CriarOrcamento();
        var servicoId = Guid.NewGuid();
        var item = ItemServico.ParaOrcamento(orcamento.Id, Guid.NewGuid(), null, 1);
        var servico = CriarServico();

        itemServicoRepository
            .Setup(x => x.GetByOrcamentoIdAndItemServicoIdAsync(orcamento.Id, item.Id, It.IsAny<CancellationToken>(), true, true))
            .ReturnsAsync(item);

        orcamentoRepository
            .Setup(x => x.GetByIdAsync(orcamento.Id, true, It.IsAny<CancellationToken>(), true))
            .ReturnsAsync(orcamento);

        servicoRepository
            .Setup(x => x.GetByIdAsync(servicoId, It.IsAny<CancellationToken>(), true))
            .ReturnsAsync(servico);

        itemServicoRepository
            .Setup(x => x.GetByOrcamentoServicoPecaIdAsync(orcamento.Id, servicoId, (Guid?)null, It.IsAny<CancellationToken>(), true))
            .ReturnsAsync((ItemServico?)null);

        // Quantidade <= 0 faz ItemServico.AtualizarDados lançar DomainException
        var command = CriarCommand(orcamento.Id, item.Id, servicoId, null, 0);

        var result = await handler.HandleAsync(command);

        Assert.False(result.IsSuccess);
        Assert.Equal("Quantidade inválida.", result.Error);
        unitOfWork.Verify(x => x.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task HandleAsync_Deve_Retornar_Falha_Generica_Quando_Ocorrer_Excecao_Inesperada()
    {
        var (orcamentoRepository, itemServicoRepository, servicoRepository, pecaRepository, unitOfWork, handler) = CriarHandler();

        var orcamentoId = Guid.NewGuid();
        var itemServicoId = Guid.NewGuid();
        var servicoId = Guid.NewGuid();

        itemServicoRepository
            .Setup(x => x.GetByOrcamentoIdAndItemServicoIdAsync(orcamentoId, itemServicoId, It.IsAny<CancellationToken>(), true, true))
            .ThrowsAsync(new InvalidOperationException("Falha inesperada."));

        var command = CriarCommand(orcamentoId, itemServicoId, servicoId, null, 1);

        var result = await handler.HandleAsync(command);

        Assert.False(result.IsSuccess);
        Assert.Equal("Não foi possível atualizar o item de serviço do orçamento.", result.Error);
    }
}