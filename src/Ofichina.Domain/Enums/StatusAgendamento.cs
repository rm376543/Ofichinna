namespace Ofichina.Domain.Enums;

/// <summary>
/// Define os possíveis estados de um agendamento.
/// Ciclo simplificado: AGENDADO → INICIADO → FINALIZADO/CANCELADO.
/// </summary>
public enum StatusAgendamento
{
    AGENDADO = 1,
    INICIADO = 2,
    FINALIZADO = 3,
    CANCELADO = 4
}