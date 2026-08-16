using Ofichina.Application.UseCases.Servicos.Commands;
using Ofichina.Application.UseCases.Servicos.Queries;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Requests.Servicos;
using Ofichina.Contracts.Responses.Servicos;

namespace Ofichina.UnitTests.Application.Servicos;

public sealed class ServicoCommandAndQueryTests
{
    [Fact]
    public void CreateServicoCommand_Deve_Mapear_Requisicao()
    {
        var request = new CreateServicoRequest
        {
            Nome = "Troca de óleo",
            Descricao = "Serviço completo",
            Valor = 149.90m,
            Ativo = true
        };

        var command = new CreateServicoCommand(request);

        Assert.Equal(request.Nome, command.Nome);
        Assert.Equal(request.Descricao, command.Descricao);
        Assert.Equal(request.Valor, command.Valor);
    }

    [Fact]
    public void UpdateServicoCommand_Deve_Mapear_Requisicao()
    {
        var request = new UpdateServicoRequest
        {
            ServicoId = Guid.NewGuid(),
            Nome = "Alinhamento",
            Descricao = "Ajuste de direção",
            Valor = 129.90m,
            Ativo = false
        };

        var command = new UpdateServicoCommand(request);

        Assert.Equal(request.ServicoId, command.ServicoId);
        Assert.Equal(request.Nome, command.Nome);
        Assert.Equal(request.Descricao, command.Descricao);
        Assert.Equal(request.Valor, command.Valor);
        Assert.False(command.Ativo);
    }

    [Fact]
    public void DeleteServicoCommand_Deve_Mapear_Identificador()
    {
        var request = new RemoveServicoRequest { ServicoId = Guid.NewGuid() };

        var command = new DeleteServicoCommand(request);

        Assert.Equal(request.ServicoId, command.Id);
    }

    [Fact]
    public void GetServicoByIdQuery_Deve_Receber_Identificador()
    {
        var id = Guid.NewGuid();

        var query = new GetServicoByIdQuery(id);

        Assert.Equal(id, query.Id);
    }

    [Fact]
    public void GetAllServicosPaginadosQuery_Deve_Preservar_Paginacao()
    {
        var pagination = new Pagination(2, 15);

        var query = new GetAllServicosPaginadosQuery(pagination);

        Assert.Same(pagination, query.Pagination);
    }
}