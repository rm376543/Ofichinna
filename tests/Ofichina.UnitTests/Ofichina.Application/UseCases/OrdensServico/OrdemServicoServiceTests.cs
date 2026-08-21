using Moq;
using Ofichina.Application.Abstractions.Interfaces.Repository;
using Ofichina.Application.UseCases.OrdensServico.Services;
using Ofichina.Contracts.Common;
using Ofichina.Domain.Aggregates;
using Ofichina.Domain.Entities;
using Ofichina.Domain.ValueObjects;
using System.Reflection;

namespace Ofichina.UnitTests.Application.UseCases.OrdensServico;

public sealed class OrdemServicoServiceTests
{
    // ============================================================  
    // SUCESSO - mapeia todos os campos, nomes encontrados e não  
    // encontrados, e os ramos não-nulos de UpdatedAt/DeletedAt/  
    // DataFinalizacao.  
    // ============================================================  

    [Fact]
    public async Task GetAllPagedAsync_Deve_Mapear_Ordens_Com_Nomes_E_Formatacoes()
    {
        // Arrange  
        var pagination = new Pagination(1, 10);

        var cliente = CriarPessoa("João Cliente");
        var consultor = CriarPessoa("Maria Consultora");

        // Ordem A: PessoaId e ConsultorId presentes no dicionário  
        // (cobre o ramo "encontrado" de ObterNome). Também com  
        // UpdatedAt, DeletedAt e DataFinalizacao preenchidos  
        // (cobre os ramos não-nulos dos null-conditional).  
        var ordemA = CriarOrdemServico(cliente.Id, consultor.Id);
        ordemA.CreatedAt = new DateTime(2026, 8, 10, 0, 0, 0, DateTimeKind.Utc);
        ordemA.UpdatedAt = new DateTime(2026, 8, 11, 0, 0, 0, DateTimeKind.Utc);
        ordemA.DeletedAt = new DateTime(2026, 8, 12, 0, 0, 0, DateTimeKind.Utc);
        DefinirDataFinalizacao(ordemA, new DateTime(2026, 8, 13, 0, 0, 0, DateTimeKind.Utc));

        // Ordem B: PessoaId e ConsultorId NÃO presentes no dicionário  
        // (cobre o ramo "Nome não encontrado" de ObterNome) e mantém  
        // UpdatedAt/DeletedAt/DataFinalizacao nulos (ramos nulos).  
        var ordemB = CriarOrdemServico(Guid.NewGuid(), Guid.NewGuid());
        ordemB.CreatedAt = new DateTime(2026, 8, 14, 0, 0, 0, DateTimeKind.Utc);

        var paged = new PagedResponse<OrdemServico>
        {
            Items = [ordemA, ordemB],
            PageNumber = 1,
            PageSize = 10,
            TotalCount = 2,
            TotalPages = 1,
            HasNextPage = false,
            HasPreviousPage = false
        };

        var ordemServicoRepository = new Mock<IOrdemServicoRepository>();
        var pessoaRepository = new Mock<IPessoaRepository>();

        ordemServicoRepository
            .Setup(x => x.GetPagedAsync(pagination, It.IsAny<CancellationToken>()))
            .ReturnsAsync(paged);

        pessoaRepository
            .Setup(x => x.GetByIdsAsync(
                It.IsAny<IEnumerable<Guid>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Pessoa> { cliente, consultor });

        var service = CriarService(ordemServicoRepository, pessoaRepository);

        // Act  
        var result = await service.GetAllPagedAsync(pagination);

        // Assert  
        Assert.Equal(1, result.PageNumber);
        Assert.Equal(10, result.PageSize);
        Assert.Equal(2, result.TotalCount);

        var itens = result.Items.ToList();
        Assert.Equal(2, itens.Count);

        var itemA = itens[0];
        Assert.Equal(ordemA.Id, itemA.OrdemServicoId);
        Assert.Equal("João Cliente", itemA.Cliente);
        Assert.Equal("Maria Consultora", itemA.Consultor);
        Assert.Equal(ordemA.ProblemaRelatado, itemA.ProblemaRelatado);
        Assert.Equal("13/08/2026", itemA.DataFinalizacao);
        Assert.NotNull(itemA.UpdatedAt);
        Assert.NotNull(itemA.DeletedAt);

        var itemB = itens[1];
        Assert.Equal("Nome não encontrado", itemB.Cliente);
        Assert.Equal("Nome não encontrado", itemB.Consultor);
        Assert.Equal("", itemB.DataFinalizacao);
        Assert.Null(itemB.UpdatedAt);
        Assert.Null(itemB.DeletedAt);

        ordemServicoRepository.Verify(
            x => x.GetPagedAsync(pagination, It.IsAny<CancellationToken>()),
            Times.Once);
        pessoaRepository.Verify(
            x => x.GetByIdsAsync(
                It.IsAny<IEnumerable<Guid>>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    // ============================================================  
    // SUCESSO - lista vazia (nenhuma ordem, nenhum id, dicionário vazio)  
    // ============================================================  

    [Fact]
    public async Task GetAllPagedAsync_Deve_Retornar_Vazio_Quando_Nao_Houver_Ordens()
    {
        // Arrange  
        var pagination = new Pagination(1, 10);

        var paged = new PagedResponse<OrdemServico>
        {
            Items = [],
            PageNumber = 1,
            PageSize = 10,
            TotalCount = 0,
            TotalPages = 0,
            HasNextPage = false,
            HasPreviousPage = false
        };

        var ordemServicoRepository = new Mock<IOrdemServicoRepository>();
        var pessoaRepository = new Mock<IPessoaRepository>();

        ordemServicoRepository
            .Setup(x => x.GetPagedAsync(pagination, It.IsAny<CancellationToken>()))
            .ReturnsAsync(paged);

        pessoaRepository
            .Setup(x => x.GetByIdsAsync(
                It.IsAny<IEnumerable<Guid>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Pessoa>());

        var service = CriarService(ordemServicoRepository, pessoaRepository);

        // Act  
        var result = await service.GetAllPagedAsync(pagination);

        // Assert  
        Assert.Empty(result.Items);
        Assert.Equal(0, result.TotalCount);
    }

    // ============================================================  
    // HELPERS  
    // ============================================================  

    private static OrdemServicoService CriarService(
        Mock<IOrdemServicoRepository> ordemServicoRepository,
        Mock<IPessoaRepository> pessoaRepository)
    {
        return new OrdemServicoService(
            ordemServicoRepository.Object,
            pessoaRepository.Object);
    }

    private static Pessoa CriarPessoa(string nome)
    {
        return new Pessoa(
            nome,
            new Cpf("12345678909"),
            new Telefone("11999999999"),
            new Endereco(
                "Rua Exemplo",
                "123",
                "",
                "Bairro Exemplo",
                "Cidade Exemplo",
                "Estado Exemplo",
                new Cep("12345-678")),
            Guid.NewGuid());
    }

    private static OrdemServico CriarOrdemServico(Guid pessoaId, Guid consultorId)
    {
        return new OrdemServico(
            pessoaId,
            Guid.NewGuid(),
            consultorId,
            10000,
            "Barulho no motor",
            "Observação da OS");
    }

    private static void DefinirDataFinalizacao(OrdemServico ordem, DateTime valor)
    {
        var propriedade = typeof(OrdemServico)
            .GetProperty(
                nameof(OrdemServico.DataFinalizacao),
                BindingFlags.Instance | BindingFlags.Public);

        Assert.NotNull(propriedade);

        propriedade!.SetValue(ordem, valor);
    }
}