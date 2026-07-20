using Ofichina.Contracts.Requests;

namespace Ofichina.Contracts.Requests.Agendamento;

/// <summary>
/// Requisição para vincular um consultor a um horário disponível.
/// </summary>
public sealed class VincularConsultorHorarioRequest : CreateRequest
{
    /// <summary>
    /// Identificador do horário disponível.
    /// </summary>
    public Guid HorarioDisponibilidadeId { get; init; }

    /// <summary>
    /// Identificador da pessoa consultora.
    /// </summary>
    public Guid ConsultorPessoaId { get; init; }
}