using Microsoft.Extensions.Logging.Abstractions;
using Ofichina.Application.UseCases.Servicos.Handlers;
using Ofichina.Application.UseCases.Servicos.Queries;
using Ofichina.Contracts.Common;
using Ofichina.Domain.Entities;
using Ofichina.UnitTests.Application.Servicos;

namespace Ofichina.UnitTests.Application.Servicos;

public sealed class ServicoQueryHandlerTests
{
    [Fact]
    public async Task GetServicoById_Deve_Mapear_Resposta_Quando_Encontrado()
    {
        var servico = new Servico("Troca de óleo", "Serviço completo", 149.90m)
        {
            CreatedAt = new DateTime(2026, 8, 10, 0, 0, 0, DateTimeKind.Utc),
            UpdatedAt = new DateTime(2026, 8, 11, 0, 0, 0, DateTimeKind.Utc)
        };

        var repository = new ServicoRepositoryTestDouble { ServicoPorId = servico };
        var handler = new GetServicoByIdQueryHandler(repository, NullLogger<GetServicoByIdQueryHandler>.Instance);

        var result = await handler.HandleAsync(new GetServicoByIdQuery(servico.Id));

        Assert.True(result.IsSuccess, result.Error);
        Assert.NotNull(result.Value);
        Assert.Equal(servico.Id, result.Value.ServicoId);
        Assert.Equal("Troca de óleo", result.Value.Nome);
        Assert.Equal("10/08/2026", result.Value.CreatedAt);
        Assert.Equal("11/08/2026", result.Value.UpdatedAt);
    }

    [Fact]
    public async Task GetServicoById_Deve_Retornar_Falha_Quando_Nao_Encontrado()
    {
        var repository = new ServicoRepositoryTestDouble();
        var handler = new GetServicoByIdQueryHandler(repository, NullLogger<GetServicoByIdQueryHandler>.Instance);

        var result = await handler.HandleAsync(new GetServicoByIdQuery(Guid.NewGuid()));

        Assert.False(result.IsSuccess);
        Assert.Equal("Serviço não encontrado.", result.Error);
    }

    [Fact]
    public async Task GetAllServicosPaginados_Deve_Mapear_Itens_Paginados()
    {
        var servicoAtivo = new Servico("Troca de óleo", null, 149.90m);
        var servicoInativo = new Servico("Alinhamento", null, 129.90m);
        servicoInativo.Desativar();

        var repository = new ServicoRepositoryTestDouble
        {
            PagedResponse = new PagedResponse<Servico>
            {
                Items = [servicoAtivo, servicoInativo],
                PageNumber = 2,
                PageSize = 10,
                TotalCount = 20,
                TotalPages = 2,
                HasNextPage = false,
                HasPreviousPage = true
            }
        };

        var handler = new GetAllServicosPaginadosQueryHandler(repository, NullLogger<GetAllServicosPaginadosQueryHandler>.Instance);
        var pagination = new Pagination(2, 10);

        var result = await handler.HandleAsync(new GetAllServicosPaginadosQuery(pagination));

        Assert.True(result.IsSuccess, result.Error);
        Assert.NotNull(result.Value);
        Assert.Same(pagination, repository.UltimaPaginacao);
        Assert.Equal(2, result.Value.PageNumber);
        Assert.Equal(10, result.Value.PageSize);
        Assert.Equal(20, result.Value.TotalCount);
        Assert.Equal(2, result.Value.TotalPages);
        Assert.False(result.Value.HasNextPage);
        Assert.True(result.Value.HasPreviousPage);
        Assert.Equal(2, result.Value.Items.Count);
        Assert.Contains(result.Value.Items, x => x.Nome == "Troca de óleo" && x.Ativo);
        Assert.Contains(result.Value.Items, x => x.Nome == "Alinhamento" && !x.Ativo);
    }

    [Fact]
    public async Task GetAllServicosPaginados_Deve_Retornar_Falha_Quando_Ocorre_Excecao()
    {
        var repository = new ServicoRepositoryTestDouble { ThrowOnGetPaged = true };
        var handler = new GetAllServicosPaginadosQueryHandler(repository, NullLogger<GetAllServicosPaginadosQueryHandler>.Instance);

        var result = await handler.HandleAsync(new GetAllServicosPaginadosQuery(new Pagination(1, 10)));

        Assert.False(result.IsSuccess);
        Assert.Equal("Não foi possível obter os serviços.", result.Error);
    }
}