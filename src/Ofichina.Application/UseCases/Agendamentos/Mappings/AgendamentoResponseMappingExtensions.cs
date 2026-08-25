using Ofichina.Contracts.Extension;
using Ofichina.Contracts.Responses.Agendamento;
using Ofichina.Contracts.Responses.Agendamento.Consultor;
using Ofichina.Domain.Aggregates;
using Ofichina.Domain.Common;
using Ofichina.Domain.Entities;

namespace Ofichina.Application.UseCases.Agendamentos.Mappings;

public static class AgendamentoResponseMappingExtensions
{
    /// <summary>
    /// Mapeia uma entidade Agendamento para um DTO de resposta AgendamentoResponse.
    /// </summary>
    /// <param name="agendamento"></param>
    /// <returns>Retorna um DTO de resposta AgendamentoResponse</returns>
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
            CreatedAt = agendamento.CreatedAt.ToDateString(),
            UpdatedAt = agendamento.UpdatedAt.ToDateString(),
            DeletedAt = agendamento.DeletedAt.ToDateString()
        };
    }

    /// <summary>
    /// Mapeia uma entidade VwAgendamentoPessoa para um DTO de resposta AgendamentoUsuarioResponse.
    /// </summary>
    /// <param name="view"></param>
    /// <returns>Retorna um DTO de resposta AgendamentoUsuarioResponse</returns>
    public static AgendamentoUsuarioResponse ToUsuarioResponse(this VwAgendamentoPessoa view)
    {
        ArgumentNullException.ThrowIfNull(view);

        return new AgendamentoUsuarioResponse
        {
            AgendamentoId = view.AgendamentosId.ToString(),
            PessoaId = view.PessoaId.ToString(),
            VeiculoId = view.VeiculoId.ToString(),
            Nome = view.Nome,
            Documento = view.Documento,
            Telefone = view.Telefone,
            Placa = view.Placa,
            Marca = view.Marca,
            Modelo = view.Modelo,
            AnoFabricacao = view.AnoFabricacao,
            Cor = view.Cor,
            Hodometro = view.Hodometro,
            Consultor = view.Consultor,
            DtAgendamento = view.DtAgendamento.ToDateString(),
            HorarioAgendamento = view.HorarioAgendamento
        };
    }

    /// <summary>
    /// Mapeia uma entidade VwAgendamentoPessoa para um DTO de resposta AgendamentoUsuarioDetalheResponse.
    /// </summary>
    /// <param name="view"></param>
    /// <returns>Retorna um DTO de resposta AgendamentoUsuarioDetalheResponse</returns>
    public static AgendamentoUsuarioDetalheResponse ToDetalheResponse(this VwAgendamentoPessoa view)
    {
        ArgumentNullException.ThrowIfNull(view);

        return new AgendamentoUsuarioDetalheResponse
        {
            AgendamentosId = view.AgendamentosId,
            PessoaId = view.PessoaId,
            Nome = view.Nome,
            Documento = view.Documento,
            Telefone = view.Telefone,
            Placa = view.Placa,
            Marca = view.Marca,
            Modelo = view.Modelo,
            AnoFabricacao = view.AnoFabricacao,
            Cor = view.Cor,
            Hodometro = view.Hodometro,
            Consultor = view.Consultor,
            DtAgendamento = view.DtAgendamento.ToDateString(),
            HorarioAgendamento = view.HorarioAgendamento,
            CreatedAt = view.CreatedAt.ToDateString(),
            UpdatedAt = view.UpdatedAt.ToDateString(),
            DeletedAt = view.DeletedAt.ToDateString()
        };
    }

    /// <summary>
    /// Mapeia uma entidade Pessoa para um DTO de resposta ConsultorDisponibilidadeResponse.
    /// </summary>
    /// <param name="consultor"></param>
    /// <returns>Retorna um DTO de resposta ConsultorDisponibilidadeResponse</returns>
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
