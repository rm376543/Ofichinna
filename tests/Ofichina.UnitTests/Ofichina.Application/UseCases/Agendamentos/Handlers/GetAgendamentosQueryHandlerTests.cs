using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Ofichina.Application.Abstractions.Interfaces.Repository;
using Ofichina.Application.UseCases.Agendamentos.Handlers;
using Ofichina.Application.UseCases.Agendamentos.Queries;
using Ofichina.Contracts.Responses.Agendamento;
using Ofichina.Domain.Aggregates;
using Ofichina.Domain.Entities;
using Ofichina.Domain.ValueObjects;

namespace Ofichina.UnitTests.Application.UseCases.Agendamentos.Handlers;

public sealed class GetAgendamentosQueryHandlerTests
{
    [Fact]
    public async Task Deve_Recusar_Quando_Pessoa_Nao_For_Encontrada()
    {
        var agendamentoRepository = new Mock<IAgendamentoRepository>();
        var pessoaRepository = new Mock<IPessoaRepository>();

        pessoaRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((Pessoa?)null);

        var handler = new GetAgendamentosQueryHandler(
            agendamentoRepository.Object,
            pessoaRepository.Object,
            NullLogger<GetAgendamentosQueryHandler>.Instance);

        var result = await handler.HandleAsync(new GetAgendamentosQuery(Guid.NewGuid()));

        Assert.False(result.IsSuccess);
        Assert.Equal("Pessoa não encontrada.", result.Error);
        Assert.Null(result.Value);
        agendamentoRepository.Verify(x => x.GetAgendamentosUsuarioViewByPessoaAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Deve_Retornar_Agendamentos_Mapeados_Quando_Pessoa_Existir()
    {
        var pessoa = new Pessoa("Cliente Teste", new Cpf("39053344705"), new Telefone("11999999999"), new Endereco("Rua Teste", "100", null, "Centro", "São Paulo", "SP", new Cep("01001000")), Guid.NewGuid());
        var agendamentoRepository = new Mock<IAgendamentoRepository>();
        var pessoaRepository = new Mock<IPessoaRepository>();

        pessoaRepository.Setup(x => x.GetByIdAsync(pessoa.Id, It.IsAny<CancellationToken>())).ReturnsAsync(pessoa);
        agendamentoRepository.Setup(x => x.GetAgendamentosUsuarioViewByPessoaAsync(pessoa.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync([
                CriarView("Cliente Teste", "39053344705", "11999999999", "ABC1D23", "Toyota", "Corolla", 2020, "Preto", 9122, "Consultor 1", new DateTime(2026, 8, 16), new TimeOnly(8, 0)),
                CriarView("Outro Cliente", "11111111111", "11988887777", "XYZ9K87", "Honda", "Civic", 2019, "Prata", 15000, "Consultor 2", new DateTime(2026, 8, 17), new TimeOnly(9, 0))
            ]);

        var handler = new GetAgendamentosQueryHandler(
            agendamentoRepository.Object,
            pessoaRepository.Object,
            NullLogger<GetAgendamentosQueryHandler>.Instance);

        var result = await handler.HandleAsync(new GetAgendamentosQuery(pessoa.Id));

        Assert.True(result.IsSuccess, result.Error);
        Assert.Equal(2, result.Value.Count);
        Assert.IsType<AgendamentoUsuarioResponse>(result.Value.First());
        Assert.Equal("Cliente Teste", result.Value.First().Nome);
        Assert.Equal("ABC1D23", result.Value.First().Placa);
        Assert.Equal(new TimeOnly(8, 0), result.Value.First().HorarioAgendamento);
        Assert.Equal("16/08/2026", result.Value.First().DtAgendamento);
    }

    private static VwAgendamentoPessoa CriarView(
        string nome,
        string documento,
        string telefone,
        string placa,
        string marca,
        string modelo,
        int ano,
        string cor,
        int hodometro,
        string consultor,
        DateTime data,
        TimeOnly horario)
        => new()
        {
            AgendamentosId = Guid.NewGuid(),
            PessoaId = Guid.NewGuid(),
            Nome = nome,
            Documento = documento,
            Telefone = telefone,
            Placa = placa,
            Marca = marca,
            Modelo = modelo,
            AnoFabricacao = ano,
            Cor = cor,
            Hodometro = hodometro,
            Consultor = consultor,
            DtAgendamento = data,
            HorarioAgendamento = horario
        };
}