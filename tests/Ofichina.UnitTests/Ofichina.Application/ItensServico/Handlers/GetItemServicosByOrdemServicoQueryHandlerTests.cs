using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Ofichina.Application.Abstractions.Interfaces.Repository;
using Ofichina.Application.UseCases.ItensServico.Handlers;
using Ofichina.Application.UseCases.ItensServico.Queries;
using Ofichina.Domain.Aggregates;
using Ofichina.Domain.Entities;
using System.Reflection;

namespace Ofichina.UnitTests.Application.ItensServico.Handlers;

public sealed class GetItemServicosByOrdemServicoQueryHandlerTests
{
    // ============================================================
    // ORDEM DE SERVIÇO
    // ============================================================

    [Fact]
    public async Task HandleAsync_Deve_Retornar_Falha_Quando_OrdemServico_Nao_Existir()
    {
        var ordemServicoId = Guid.NewGuid();

        var ordemServicoRepository =
            new Mock<IOrdemServicoRepository>();

        ordemServicoRepository
            .Setup(x => x.GetByIdAsync(
                ordemServicoId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((OrdemServico?)null);

        var itemServicoRepository =
            new Mock<IItemServicoRepository>();

        var handler = CriarHandler(
            ordemServicoRepository,
            itemServicoRepository);

        var result = await handler.HandleAsync(
            CriarQuery(ordemServicoId));

        Assert.False(result.IsSuccess);

        Assert.Equal(
            "Ordem de serviço não encontrada.",
            result.Error);

        itemServicoRepository.Verify(
            x => x.GetByOrdemServicoIdAsync(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>(),
                true),
            Times.Never);
    }

    [Fact]
    public async Task HandleAsync_Deve_Retornar_Falha_Quando_OrdemServico_Estiver_Excluida()
    {
        var ordemServicoId = Guid.NewGuid();

        var ordemServico =
            CriarEntidade<OrdemServico>();

        ExcluirEntidade(ordemServico);

        var ordemServicoRepository =
            new Mock<IOrdemServicoRepository>();

        ordemServicoRepository
            .Setup(x => x.GetByIdAsync(
                ordemServicoId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(ordemServico);

        var itemServicoRepository =
            new Mock<IItemServicoRepository>();

        var handler = CriarHandler(
            ordemServicoRepository,
            itemServicoRepository);

        var result = await handler.HandleAsync(
            CriarQuery(ordemServicoId));

        Assert.False(result.IsSuccess);

        Assert.Equal(
            "Ordem de serviço não encontrada.",
            result.Error);

        itemServicoRepository.Verify(
            x => x.GetByOrdemServicoIdAsync(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>(),
                true),
            Times.Never);
    }

    // ============================================================
    // NENHUM ITEM
    // ============================================================

    [Fact]
    public async Task HandleAsync_Deve_Retornar_Lista_Vazia_Quando_Nao_Houver_Itens()
    {
        var ordemServicoId = Guid.NewGuid();

        var ordemServico =
            CriarEntidade<OrdemServico>();

        var ordemServicoRepository =
            CriarOrdemServicoRepository(ordemServicoId, ordemServico);

        var itemServicoRepository =
            CriarItemServicoRepository(
                ordemServicoId,
                Array.Empty<ItemServico>());

        var handler = CriarHandler(
            ordemServicoRepository,
            itemServicoRepository);

        var result = await handler.HandleAsync(
            CriarQuery(ordemServicoId));

        Assert.True(result.IsSuccess);

        Assert.NotNull(result.Value);

        var response =
            Assert.Single(result.Value!);

        Assert.Equal(
            ordemServicoId,
            response.OrdemServicoId);

        Assert.Empty(response.Servicos);
    }

    // ============================================================
    // ITEM EXCLUÍDO
    // ============================================================

    [Fact]
    public async Task HandleAsync_Deve_Ignorar_Itens_Excluidos()
    {
        var ordemServicoId = Guid.NewGuid();

        var ordemServico =
            CriarEntidade<OrdemServico>();

        var servicoId = Guid.NewGuid();

        var servico =
            CriarServico(
                servicoId,
                "Troca de óleo",
                150m);

        var itemExcluido =
            CriarItemServico(
                ordemServicoId,
                servicoId,
                servico,
                null,
                null,
                1);

        ExcluirEntidade(itemExcluido);

        var ordemServicoRepository =
            CriarOrdemServicoRepository(
                ordemServicoId,
                ordemServico);

        var itemServicoRepository =
            CriarItemServicoRepository(
                ordemServicoId,
                new[] { itemExcluido });

        var handler = CriarHandler(
            ordemServicoRepository,
            itemServicoRepository);

        var result = await handler.HandleAsync(
            CriarQuery(ordemServicoId));

        Assert.True(result.IsSuccess);

        var response =
            Assert.Single(result.Value!);

        Assert.Empty(response.Servicos);
    }

    // ============================================================
    // SERVIÇO SEM PEÇA
    // ============================================================

    [Fact]
    public async Task HandleAsync_Deve_Retornar_Servico_Sem_Pecas()
    {
        var ordemServicoId = Guid.NewGuid();

        var servicoId = Guid.NewGuid();

        var servico =
            CriarServico(
                servicoId,
                "Diagnóstico",
                200m);

        var item =
            CriarItemServico(
                ordemServicoId,
                servicoId,
                servico,
                null,
                null,
                1);

        var ordemServicoRepository =
            CriarOrdemServicoRepository(
                ordemServicoId,
                CriarEntidade<OrdemServico>());

        var itemServicoRepository =
            CriarItemServicoRepository(
                ordemServicoId,
                new[] { item });

        var handler = CriarHandler(
            ordemServicoRepository,
            itemServicoRepository);

        var result = await handler.HandleAsync(
            CriarQuery(ordemServicoId));

        Assert.True(result.IsSuccess);

        var response =
            Assert.Single(result.Value!);

        var servicoResponse =
            Assert.Single(response.Servicos);

        Assert.Equal(
            servicoId,
            servicoResponse.ServicoId);

        Assert.Equal(
            "Diagnóstico",
            servicoResponse.Descricao);

        Assert.Equal(
            200m,
            servicoResponse.ValorServico);

        Assert.Empty(
            servicoResponse.Pecas);

        Assert.Equal(
            200m,
            servicoResponse.ValorTotal);
    }

    // ============================================================
    // SERVIÇO COM PEÇA
    // ============================================================

    [Fact]
    public async Task HandleAsync_Deve_Retornar_Servico_Com_Peca()
    {
        var ordemServicoId = Guid.NewGuid();

        var servicoId = Guid.NewGuid();
        var pecaId = Guid.NewGuid();

        var servico =
            CriarServico(
                servicoId,
                "Revisão",
                300m);

        var peca =
            CriarPeca(
                pecaId,
                "Filtro de óleo",
                50m);

        var item =
            CriarItemServico(
                ordemServicoId,
                servicoId,
                servico,
                pecaId,
                peca,
                2);

        var ordemServicoRepository =
            CriarOrdemServicoRepository(
                ordemServicoId,
                CriarEntidade<OrdemServico>());

        var itemServicoRepository =
            CriarItemServicoRepository(
                ordemServicoId,
                new[] { item });

        var handler = CriarHandler(
            ordemServicoRepository,
            itemServicoRepository);

        var result = await handler.HandleAsync(
            CriarQuery(ordemServicoId));

        Assert.True(result.IsSuccess);

        var response =
            Assert.Single(result.Value!);

        var servicoResponse =
            Assert.Single(response.Servicos);

        var pecaResponse =
            Assert.Single(servicoResponse.Pecas);

        Assert.Equal(
            pecaId,
            pecaResponse.PecaId);

        Assert.Equal(
            "Filtro de óleo",
            pecaResponse.Descricao);

        Assert.Equal(
            2,
            pecaResponse.Quantidade);

        Assert.Equal(
            50m,
            pecaResponse.ValorUnitario);

        Assert.Equal(
            100m,
            pecaResponse.ValorTotal);

        Assert.Equal(
            400m,
            servicoResponse.ValorTotal);
    }

    // ============================================================
    // PEÇA SEM NAVEGAÇÃO
    // ============================================================

    [Fact]
    public async Task HandleAsync_Deve_Usar_Valores_Padrao_Quando_Peca_For_Null()
    {
        var ordemServicoId = Guid.NewGuid();

        var servicoId = Guid.NewGuid();
        var pecaId = Guid.NewGuid();

        var servico =
            CriarServico(
                servicoId,
                "Serviço",
                100m);

        var item =
            CriarItemServico(
                ordemServicoId,
                servicoId,
                servico,
                pecaId,
                null,
                3);

        var ordemServicoRepository =
            CriarOrdemServicoRepository(
                ordemServicoId,
                CriarEntidade<OrdemServico>());

        var itemServicoRepository =
            CriarItemServicoRepository(
                ordemServicoId,
                new[] { item });

        var handler = CriarHandler(
            ordemServicoRepository,
            itemServicoRepository);

        var result = await handler.HandleAsync(
            CriarQuery(ordemServicoId));

        Assert.True(result.IsSuccess);

        var response =
            Assert.Single(result.Value!);

        var servicoResponse =
            Assert.Single(response.Servicos);

        var pecaResponse =
            Assert.Single(servicoResponse.Pecas);

        Assert.Equal(
            pecaId,
            pecaResponse.PecaId);

        Assert.Equal(
            string.Empty,
            pecaResponse.Descricao);

        Assert.Equal(
            3,
            pecaResponse.Quantidade);

        Assert.Equal(
            0m,
            pecaResponse.ValorUnitario);

        Assert.Equal(
            0m,
            pecaResponse.ValorTotal);

        Assert.Equal(
            100m,
            servicoResponse.ValorTotal);
    }

    // ============================================================
    // ITEM SEM PECA ID
    // ============================================================

    [Fact]
    public async Task HandleAsync_Deve_Ignorar_Item_Que_Nao_Possuir_PecaId()
    {
        var ordemServicoId = Guid.NewGuid();

        var servicoId = Guid.NewGuid();

        var servico =
            CriarServico(
                servicoId,
                "Diagnóstico",
                120m);

        var item =
            CriarItemServico(
                ordemServicoId,
                servicoId,
                servico,
                null,
                null,
                5);

        var ordemServicoRepository =
            CriarOrdemServicoRepository(
                ordemServicoId,
                CriarEntidade<OrdemServico>());

        var itemServicoRepository =
            CriarItemServicoRepository(
                ordemServicoId,
                new[] { item });

        var handler = CriarHandler(
            ordemServicoRepository,
            itemServicoRepository);

        var result = await handler.HandleAsync(
            CriarQuery(ordemServicoId));

        Assert.True(result.IsSuccess);

        var response =
            Assert.Single(result.Value!);

        var servicoResponse =
            Assert.Single(response.Servicos);

        Assert.Empty(
            servicoResponse.Pecas);

        Assert.Equal(
            120m,
            servicoResponse.ValorTotal);
    }

    // ============================================================
    // AGRUPAMENTO DE SERVIÇOS
    // ============================================================

    [Fact]
    public async Task HandleAsync_Deve_Agrupar_Itens_Do_Mesmo_Servico()
    {
        var ordemServicoId = Guid.NewGuid();

        var servicoId = Guid.NewGuid();

        var servico =
            CriarServico(
                servicoId,
                "Revisão",
                250m);

        var peca1Id = Guid.NewGuid();
        var peca2Id = Guid.NewGuid();

        var peca1 =
            CriarPeca(
                peca1Id,
                "Filtro",
                30m);

        var peca2 =
            CriarPeca(
                peca2Id,
                "Óleo",
                40m);

        var item1 =
            CriarItemServico(
                ordemServicoId,
                servicoId,
                servico,
                peca1Id,
                peca1,
                2);

        var item2 =
            CriarItemServico(
                ordemServicoId,
                servicoId,
                servico,
                peca2Id,
                peca2,
                1);

        var ordemServicoRepository =
            CriarOrdemServicoRepository(
                ordemServicoId,
                CriarEntidade<OrdemServico>());

        var itemServicoRepository =
            CriarItemServicoRepository(
                ordemServicoId,
                new[] { item1, item2 });

        var handler = CriarHandler(
            ordemServicoRepository,
            itemServicoRepository);

        var result = await handler.HandleAsync(
            CriarQuery(ordemServicoId));

        Assert.True(result.IsSuccess);

        var response =
            Assert.Single(result.Value!);

        var servicos =
            response.Servicos.ToList();

        var servicoResponse =
            Assert.Single(servicos);

        Assert.Equal(
            servicoId,
            servicoResponse.ServicoId);

        Assert.Equal(
            2,
            servicoResponse.Pecas.Count);

        // Serviço = 250
        // Filtro = 30 * 2 = 60
        // Óleo = 40 * 1 = 40
        // Total = 350
        Assert.Equal(
            350m,
            servicoResponse.ValorTotal);
    }

    // ============================================================
    // MAIS DE UM SERVIÇO
    // ============================================================

    [Fact]
    public async Task HandleAsync_Deve_Retornar_Multiplos_Servicos()
    {
        var ordemServicoId = Guid.NewGuid();

        var servico1Id = Guid.NewGuid();
        var servico2Id = Guid.NewGuid();

        var servico1 =
            CriarServico(
                servico1Id,
                "Alinhamento",
                100m);

        var servico2 =
            CriarServico(
                servico2Id,
                "Balanceamento",
                150m);

        var item1 =
            CriarItemServico(
                ordemServicoId,
                servico1Id,
                servico1,
                null,
                null,
                1);

        var item2 =
            CriarItemServico(
                ordemServicoId,
                servico2Id,
                servico2,
                null,
                null,
                1);

        var ordemServicoRepository =
            CriarOrdemServicoRepository(
                ordemServicoId,
                CriarEntidade<OrdemServico>());

        var itemServicoRepository =
            CriarItemServicoRepository(
                ordemServicoId,
                new[] { item1, item2 });

        var handler = CriarHandler(
            ordemServicoRepository,
            itemServicoRepository);

        var result = await handler.HandleAsync(
            CriarQuery(ordemServicoId));

        Assert.True(result.IsSuccess);

        var response =
            Assert.Single(result.Value!);

        Assert.Equal(
            2,
            response.Servicos.Count);

        Assert.Contains(
            response.Servicos,
            x => x.ServicoId == servico1Id);

        Assert.Contains(
            response.Servicos,
            x => x.ServicoId == servico2Id);
    }

    // ============================================================
    // EXCEPTION
    // ============================================================

    [Fact]
    public async Task HandleAsync_Deve_Retornar_Falha_Quando_Ocorre_Excecao()
    {
        var ordemServicoId = Guid.NewGuid();

        var ordemServicoRepository =
            new Mock<IOrdemServicoRepository>();

        ordemServicoRepository
            .Setup(x => x.GetByIdAsync(
                ordemServicoId,
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(
                new InvalidOperationException(
                    "Erro inesperado."));

        var itemServicoRepository =
            new Mock<IItemServicoRepository>();

        var handler = CriarHandler(
            ordemServicoRepository,
            itemServicoRepository);

        var result = await handler.HandleAsync(
            CriarQuery(ordemServicoId));

        Assert.False(result.IsSuccess);

        Assert.Equal(
            "Não foi possível obter os itens de serviço.",
            result.Error);
    }

    // ============================================================
    // CANCELLATION TOKEN
    // ============================================================

    [Fact]
    public async Task HandleAsync_Deve_Propagar_CancellationToken()
    {
        var ordemServicoId = Guid.NewGuid();

        var cancellationTokenSource =
            new CancellationTokenSource();

        var cancellationToken =
            cancellationTokenSource.Token;

        var ordemServico =
            CriarEntidade<OrdemServico>();

        var ordemServicoRepository =
            new Mock<IOrdemServicoRepository>();

        ordemServicoRepository
            .Setup(x => x.GetByIdAsync(
                ordemServicoId,
                cancellationToken))
            .ReturnsAsync(ordemServico);

        var itemServicoRepository =
            CriarItemServicoRepository(
                ordemServicoId,
                Array.Empty<ItemServico>());

        var handler = CriarHandler(
            ordemServicoRepository,
            itemServicoRepository);

        var result = await handler.HandleAsync(
            CriarQuery(ordemServicoId),
            cancellationToken);

        Assert.True(result.IsSuccess);

        ordemServicoRepository.Verify(
            x => x.GetByIdAsync(
                ordemServicoId,
                cancellationToken),
            Times.Once);

        itemServicoRepository.Verify(
            x => x.GetByOrdemServicoIdAsync(
                ordemServicoId,
                cancellationToken,
                true),
            Times.Once);
    }

    // ============================================================
    // FACTORIES
    // ============================================================

    private static GetItemServicosByOrdemServicoQueryHandler CriarHandler(
        Mock<IOrdemServicoRepository> ordemServicoRepository,
        Mock<IItemServicoRepository> itemServicoRepository)
    {
        return new GetItemServicosByOrdemServicoQueryHandler(
            ordemServicoRepository.Object,
            itemServicoRepository.Object,
            NullLogger<GetItemServicosByOrdemServicoQueryHandler>.Instance);
    }

    private static GetItemServicosByOrdemServicoQuery CriarQuery(
        Guid ordemServicoId)
    {
        return new GetItemServicosByOrdemServicoQuery
        {
            OrdemServicoId = ordemServicoId
        };
    }

    private static Mock<IOrdemServicoRepository>
        CriarOrdemServicoRepository(
            Guid ordemServicoId,
            OrdemServico ordemServico)
    {
        var repository =
            new Mock<IOrdemServicoRepository>();

        repository
            .Setup(x => x.GetByIdAsync(
                ordemServicoId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(ordemServico);

        return repository;
    }

    private static Mock<IItemServicoRepository>
        CriarItemServicoRepository(
            Guid ordemServicoId,
            IEnumerable<ItemServico> itens)
    {
        var repository =
            new Mock<IItemServicoRepository>();

        repository
            .Setup(x => x.GetByOrdemServicoIdAsync(
                ordemServicoId,
                It.IsAny<CancellationToken>(),
                includeRelacionados: true))
            .ReturnsAsync(itens.ToList());

        return repository;
    }

    // ============================================================
    // ENTIDADES
    // ============================================================

    private static Servico CriarServico(
        Guid id,
        string nome,
        decimal valor)
    {
        var servico =
            CriarEntidade<Servico>();

        DefinirPropriedade(
            servico,
            nameof(Servico.Id),
            id);

        DefinirPropriedade(
            servico,
            nameof(Servico.Nome),
            nome);

        DefinirPropriedade(
            servico,
            nameof(Servico.Valor),
            valor);

        return servico;
    }

    private static Peca CriarPeca(
        Guid id,
        string nome,
        decimal valor)
    {
        var peca =
            CriarEntidade<Peca>();

        DefinirPropriedade(
            peca,
            nameof(Peca.Id),
            id);

        DefinirPropriedade(
            peca,
            nameof(Peca.Nome),
            nome);

        DefinirPropriedade(
            peca,
            nameof(Peca.Valor),
            valor);

        return peca;
    }

    private static ItemServico CriarItemServico(
        Guid ordemServicoId,
        Guid servicoId,
        Servico servico,
        Guid? pecaId,
        Peca? peca,
        int quantidade)
    {
        var item =
            CriarEntidade<ItemServico>();

        DefinirPropriedade(
            item,
            nameof(ItemServico.Id),
            Guid.NewGuid());

        DefinirPropriedade(
            item,
            nameof(ItemServico.OrdemServicoId),
            ordemServicoId);

        DefinirPropriedade(
            item,
            nameof(ItemServico.ServicoId),
            servicoId);

        DefinirPropriedade(
            item,
            nameof(ItemServico.Servico),
            servico);

        DefinirPropriedade(
            item,
            nameof(ItemServico.PecaId),
            pecaId);

        DefinirPropriedade(
            item,
            nameof(ItemServico.Peca),
            peca);

        DefinirPropriedade(
            item,
            nameof(ItemServico.Quantidade),
            quantidade);

        return item;
    }

    // ============================================================
    // CRIAÇÃO SEM CONSTRUTOR PÚBLICO
    // ============================================================

    private static T CriarEntidade<T>()
        where T : class
    {
        return (T)Activator.CreateInstance(
            typeof(T),
            BindingFlags.Instance |
            BindingFlags.Public |
            BindingFlags.NonPublic,
            binder: null,
            args: null,
            culture: null)!;
    }

    // ============================================================
    // EXCLUSÃO LÓGICA
    // ============================================================

    private static void ExcluirEntidade(
        object entidade)
    {
        var metodo =
            entidade.GetType().GetMethod(
                "Excluir",
                BindingFlags.Instance |
                BindingFlags.Public |
                BindingFlags.NonPublic);

        if (metodo is null)
        {
            throw new InvalidOperationException(
                $"O método 'Excluir' não foi encontrado em " +
                $"{entidade.GetType().Name}.");
        }

        metodo.Invoke(
            entidade,
            null);
    }

    // ============================================================
    // REFLECTION
    // ============================================================

    private static void DefinirPropriedade(
        object objeto,
        string nome,
        object? valor)
    {
        var propriedade =
            objeto.GetType().GetProperty(
                nome,
                BindingFlags.Instance |
                BindingFlags.Public |
                BindingFlags.NonPublic);

        if (propriedade is null)
        {
            throw new InvalidOperationException(
                $"A propriedade '{nome}' não foi encontrada " +
                $"em '{objeto.GetType().Name}'.");
        }

        propriedade.SetValue(
            objeto,
            valor);
    }
}