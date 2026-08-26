using Ofichina.Application.UseCases.Agendamentos.Mappings;
using Ofichina.Domain.Aggregates;
using Ofichina.Domain.Entities;
using Ofichina.Domain.ValueObjects;
using System.Reflection;

namespace Ofichina.UnitTests.Application;

public sealed class ResponseMappingExtensionsTests
{
    [Fact]
    public void ToResponse_Deve_Preencher_Dados_Do_Consultor_Apartir_De_AgendaConsultor()
    {
        var cliente = CriarPessoa("Cliente Teste", "123.456.789-09", "(11) 91234-5678");
        var consultor = CriarPessoa("Consultor Teste", "529.982.247-25", "(11) 92345-6789");
        var veiculo = CriarVeiculo(cliente.Id, "ABC1D23", "Toyota", "Corolla", 2024, "Prata", 12500);
        var dia = new DiaDisponibilidade(new DateOnly(2026, 8, 15));
        var horario = new HorarioDisponibilidade(new TimeOnly(14, 30));
        var agendaConsultor = new AgendaConsultor(dia.Id, horario.Id, consultor.Id);
        DefinirPropriedade(agendaConsultor, nameof(AgendaConsultor.DiaDisponibilidade), dia);
        DefinirPropriedade(agendaConsultor, nameof(AgendaConsultor.HorarioDisponibilidade), horario);
        DefinirPropriedade(agendaConsultor, nameof(AgendaConsultor.Consultor), consultor);

        var agendamento = new Agendamento(cliente.Id, agendaConsultor.Id, veiculo.Id, 12500, "Revisão agendada");
        DefinirPropriedade(agendamento, nameof(Agendamento.Cliente), cliente);
        DefinirPropriedade(agendamento, nameof(Agendamento.Veiculo), veiculo);
        DefinirPropriedade(agendamento, nameof(Agendamento.AgendaConsultor), agendaConsultor);

        var response = agendamento.ToResponse();

        Assert.Equal(cliente.Id, response.PessoaId);
        Assert.Equal("Cliente Teste", response.ClienteNome);
        Assert.Equal(dia.Id, response.DiaId);
        Assert.Equal(horario.Id, response.HorarioId);
        Assert.Equal(consultor.Id, response.ConsultorId);
        Assert.Equal("Consultor Teste", response.ConsultorNome);
        Assert.Equal(veiculo.Id, response.VeiculoId);
        Assert.Equal("ABC1D23", response.VeiculoPlaca);
        Assert.Equal("Toyota Corolla 2024", response.VeiculoDescricao);
    }

    [Fact]
    public void ToUsuarioResponse_Deve_Mapear_View_Expondo_AgendamentosId_VeiculoId_E_PessoaId()
    {
        var agendamentoGuid = Guid.NewGuid();
        var pessoaGuid = Guid.NewGuid();
        var veiculoGuid = Guid.NewGuid();

        var view = new VwAgendamentoPessoa
        {
            AgendamentosId = agendamentoGuid,
            PessoaId = pessoaGuid,
            VeiculoId = veiculoGuid,
            Nome = "João Silva",
            Documento = "12345678909",
            Telefone = "11987654321",
            Placa = "ABC1D23",
            Marca = "Fiat",
            Modelo = "Uno",
            AnoFabricacao = 2020,
            Cor = "Branco",
            Hodometro = 45000,
            Consultor = "Maria Souza",
            DtAgendamento = new DateTime(2026, 8, 15, 0, 0, 0, DateTimeKind.Utc),
            HorarioAgendamento = new TimeOnly(14, 30),
            CreatedAt = new DateTime(2026, 8, 10, 0, 0, 0, DateTimeKind.Utc),
            UpdatedAt = new DateTime(2026, 8, 10, 0, 0, 0, DateTimeKind.Utc),
            DeletedAt = null
        };

        var response = view.ToUsuarioResponse();

        Assert.Equal(agendamentoGuid.ToString(), response.AgendamentoId);
        Assert.Equal(pessoaGuid.ToString(), response.PessoaId);
        Assert.Equal(veiculoGuid.ToString(), response.VeiculoId);
        Assert.Equal("João Silva", response.Nome);
        Assert.Equal("12345678909", response.Documento);
        Assert.Equal("11987654321", response.Telefone);
        Assert.Equal("ABC1D23", response.Placa);
        Assert.Equal("Fiat", response.Marca);
        Assert.Equal("Uno", response.Modelo);
        Assert.Equal(2020, response.AnoFabricacao);
        Assert.Equal("Branco", response.Cor);
        Assert.Equal(45000, response.Hodometro);
        Assert.Equal("Maria Souza", response.Consultor);
        Assert.Equal("15/08/2026", response.DtAgendamento);
        Assert.Equal(new TimeOnly(14, 30), response.HorarioAgendamento);

        var responseType = response.GetType();
        Assert.Null(responseType.GetProperty("UpdatedAt"));
        Assert.Null(responseType.GetProperty("DeletedAt"));
    }

    private static Pessoa CriarPessoa(string nome, string cpf, string telefone)
    {
        return new Pessoa(
            nome,
            new Cpf(cpf),
            new Telefone(telefone),
            new Endereco("Rua Teste", "100", null, "Centro", "São Paulo", "SP", new Cep("01001000")),
            Guid.NewGuid());
    }

    private static Veiculo CriarVeiculo(Guid pessoaId, string placa, string marca, string modelo, int ano, string cor, int hodometro)
    {
        return new Veiculo(pessoaId, new Placa(placa), marca, modelo, ano, cor, new Hodometro(hodometro));
    }

    private static void DefinirPropriedade<T>(T instancia, string propriedade, object? valor)
        where T : class
    {
        var property = typeof(T).GetProperty(propriedade, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        Assert.NotNull(property);
        property!.SetValue(instancia, valor);
    }
}