using Ofichina.Contracts.Responses.Agendamento;
using Ofichina.Domain.Aggregates;
using Ofichina.Domain.Common;
using Ofichina.Domain.Entities;

namespace Ofichina.Application.UseCases.Agendamentos.Mappings;

public static class AgendamentoResponseMappingExtensions
{
    public static AgendamentoResponse ToResponse(this Agendamento agendamento)
    {
        ArgumentNullException.ThrowIfNull(agendamento);

        return new AgendamentoResponse
        {
            AgendamentoId = agendamento.Id,
            PessoaId = agendamento.ClientePessoaId,
            ClienteNome = agendamento.Cliente.Nome,
            DiaId = agendamento.AgendaConsultor?.DiaDisponibilidadeId,
            HorarioId = agendamento.AgendaConsultor?.HorarioDisponibilidadeId,
            ConsultorId = agendamento.AgendaConsultor?.ConsultorPessoaId,
            ConsultorNome = agendamento.AgendaConsultor?.Consultor?.Nome ?? string.Empty,
            VeiculoId = agendamento.VeiculoId,
            VeiculoPlaca = agendamento.Veiculo.Placa.Numero,
            VeiculoDescricao = $"{agendamento.Veiculo.Marca} {agendamento.Veiculo.Modelo} {agendamento.Veiculo.AnoFabricacao}",
            Status = agendamento.Status.ToUpperSnakeCase(),
            Descricao = agendamento.Descricao,
            CreatedAt = agendamento.CreatedAt,
            UpdatedAt = agendamento.UpdatedAt,
            DeletedAt = agendamento.DeletedAt
        };
    }

    public static ConsultorDisponibilidadeResponse ToConsultorDisponibilidadeResponse(this Pessoa consultor)
    {
        ArgumentNullException.ThrowIfNull(consultor);

        return new ConsultorDisponibilidadeResponse
        {
            ConsultorId = consultor.Id,
            Nome = consultor.Nome,
            Documento = consultor.Documento?.Numero ?? string.Empty
        };
    }
}
