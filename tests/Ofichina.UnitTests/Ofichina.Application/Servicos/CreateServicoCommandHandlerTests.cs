using Microsoft.Extensions.Logging.Abstractions;
using Ofichina.Application.UseCases.Servicos.Commands;
using Ofichina.Application.UseCases.Servicos.Handlers;
using Ofichina.Contracts.Requests.Servicos;
using Ofichina.Domain.Entities;

namespace Ofichina.UnitTests.Application.Servicos;

public sealed class CreateServicoCommandHandlerTests
{
    [Fact]
    public async Task Deve_Criar_Servico_E_Persistir_Alteracoes()
    {
        var repository = new ServicoRepositoryTestDouble();
        var unitOfWork = new TestUnitOfWork();
        var handler = new CreateServicoCommandHandler(repository, unitOfWork, NullLogger<CreateServicoCommandHandler>.Instance);

        var result = await handler.HandleAsync(new CreateServicoCommand(new CreateServicoRequest
        {
            Nome = "Troca de óleo",
            Descricao = "Serviço completo",
            Valor = 149.90m,
            Ativo = true
        }));

        Assert.True(result.IsSuccess, result.Error);
        Assert.NotNull(repository.UltimoServicoAdicionado);
        Assert.Equal("Troca de óleo", repository.UltimoServicoAdicionado!.Nome);
        Assert.Equal(1, unitOfWork.SaveChangesCalls);
    }

    [Fact]
    public async Task Deve_Retornar_Falha_Quando_Regra_De_Dominio_For_Violada()
    {
        var repository = new ServicoRepositoryTestDouble();
        var unitOfWork = new TestUnitOfWork();
        var handler = new CreateServicoCommandHandler(repository, unitOfWork, NullLogger<CreateServicoCommandHandler>.Instance);

        var result = await handler.HandleAsync(new CreateServicoCommand(new CreateServicoRequest
        {
            Nome = string.Empty,
            Descricao = null,
            Valor = 0m,
            Ativo = true
        }));

        Assert.False(result.IsSuccess);
        Assert.Equal("O nome do serviço é obrigatório.", result.Error);
        Assert.Null(repository.UltimoServicoAdicionado);
        Assert.Equal(0, unitOfWork.SaveChangesCalls);
    }
}