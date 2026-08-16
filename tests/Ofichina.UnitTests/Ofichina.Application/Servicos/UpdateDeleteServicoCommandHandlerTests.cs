using Microsoft.Extensions.Logging.Abstractions;
using Ofichina.Application.UseCases.Servicos.Commands;
using Ofichina.Application.UseCases.Servicos.Handlers;
using Ofichina.Contracts.Requests.Servicos;
using Ofichina.Contracts.Responses.Servicos;
using Ofichina.Domain.Entities;

namespace Ofichina.UnitTests.Application.Servicos;

public sealed class UpdateDeleteServicoCommandHandlerTests
{
    [Fact]
    public async Task UpdateDeve_Atualizar_Servico_E_Marcar_Inativo_Quando_Informado()
    {
        var servico = new Servico("Balanceamento", "Ajuste de rodas", 89.90m);
        var repository = new ServicoRepositoryTestDouble { ServicoPorId = servico };
        var unitOfWork = new TestUnitOfWork();
        var handler = new UpdateServicoCommandHandler(repository, unitOfWork, NullLogger<UpdateServicoCommandHandler>.Instance);

        var result = await handler.HandleAsync(new UpdateServicoCommand(new UpdateServicoRequest
        {
            ServicoId = servico.Id,
            Nome = "Alinhamento",
            Descricao = "Ajuste de direção",
            Valor = 129.90m,
            Ativo = false
        }));

        Assert.True(result.IsSuccess, result.Error);
        Assert.Same(servico, repository.UltimoServicoAtualizado);
        Assert.Equal("Alinhamento", servico.Nome);
        Assert.True(servico.EstaExcluida());
        Assert.Equal(1, unitOfWork.SaveChangesCalls);
    }

    [Fact]
    public async Task UpdateDeve_Retornar_Falha_Quando_Servico_Nao_For_Encontrado()
    {
        var repository = new ServicoRepositoryTestDouble();
        var unitOfWork = new TestUnitOfWork();
        var handler = new UpdateServicoCommandHandler(repository, unitOfWork, NullLogger<UpdateServicoCommandHandler>.Instance);

        var result = await handler.HandleAsync(new UpdateServicoCommand(new UpdateServicoRequest
        {
            ServicoId = Guid.NewGuid(),
            Nome = "Alinhamento",
            Descricao = "Ajuste de direção",
            Valor = 129.90m,
            Ativo = true
        }));

        Assert.False(result.IsSuccess);
        Assert.Equal("Serviço não encontrado.", result.Error);
        Assert.Null(repository.UltimoServicoAtualizado);
        Assert.Equal(0, unitOfWork.SaveChangesCalls);
    }

    [Fact]
    public async Task DeleteDeve_Excluir_Servico_E_Persistir_Alteracoes()
    {
        var servico = new Servico("Balanceamento", null, 89.90m);
        var repository = new ServicoRepositoryTestDouble { ServicoPorId = servico };
        var unitOfWork = new TestUnitOfWork();
        var handler = new DeleteServicoCommandHandler(repository, unitOfWork, NullLogger<DeleteServicoCommandHandler>.Instance);

        var result = await handler.HandleAsync(new DeleteServicoCommand(new RemoveServicoRequest { ServicoId = servico.Id }));

        Assert.True(result.IsSuccess, result.Error);
        Assert.Same(servico, repository.UltimoServicoAtualizado);
        Assert.True(servico.EstaExcluida());
        Assert.NotNull(servico.DeletedAt);
        Assert.Equal(1, unitOfWork.SaveChangesCalls);
    }

    [Fact]
    public async Task DeleteDeve_Retornar_Falha_Quando_Servico_Nao_For_Encontrado()
    {
        var repository = new ServicoRepositoryTestDouble();
        var unitOfWork = new TestUnitOfWork();
        var handler = new DeleteServicoCommandHandler(repository, unitOfWork, NullLogger<DeleteServicoCommandHandler>.Instance);

        var result = await handler.HandleAsync(new DeleteServicoCommand(new RemoveServicoRequest { ServicoId = Guid.NewGuid() }));

        Assert.False(result.IsSuccess);
        Assert.Equal("Serviço não encontrado.", result.Error);
        Assert.Null(repository.UltimoServicoAtualizado);
        Assert.Equal(0, unitOfWork.SaveChangesCalls);
    }
}