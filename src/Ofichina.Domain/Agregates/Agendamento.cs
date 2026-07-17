using Ofichina.Domain.Entities;
using Ofichina.Domain.Enums;
using Ofichina.Domain.Exceptions;

namespace Ofichina.Domain.Aggregates;

/// <summary>
/// Representa um agendamento realizado por um usuário autenticado.
/// </summary>
public class Agendamento : Entity
{
    /// <summary>
    /// Pessoa vinculada ao agendamento.
    /// </summary>
    public Guid PessoaId { get; private set; }

    /// <summary>
    /// Veículo informado para o agendamento.
    /// </summary>
    public Guid VeiculoId { get; private set; }

    /// <summary>
    /// Data e hora solicitadas para atendimento.
    /// </summary>
    public DateTime DataHoraAgendada { get; private set; }

    /// <summary>
    /// Motivo principal do agendamento.
    /// </summary>
    public string Motivo { get; private set; } = string.Empty;

    /// <summary>
    /// Observações adicionais informadas pelo cliente.
    /// </summary>
    public string? Observacoes { get; private set; }

    /// <summary>
    /// Status atual do agendamento.
    /// </summary>
    public StatusAgendamento Status { get; private set; }

    /// <summary>
    /// Canal de atendimento utilizado.
    /// </summary>
    public CanalAtendimento CanalAtendimento { get; private set; }

    private Agendamento()
    {
    }

    public Agendamento(
        Guid pessoaId,
        Guid veiculoId,
        DateTime dataHoraAgendada,
        string motivo,
        string? observacoes,
        CanalAtendimento canalAtendimento = CanalAtendimento.Aplicativo)
    {
        if (pessoaId == Guid.Empty)
            throw new DomainException("A pessoa é obrigatória.");

        if (veiculoId == Guid.Empty)
            throw new DomainException("O veículo é obrigatório.");

        if (dataHoraAgendada <= DateTime.UtcNow)
            throw new DomainException("A data e hora do agendamento devem ser futuras.");

        if (string.IsNullOrWhiteSpace(motivo))
            throw new DomainException("O motivo do agendamento é obrigatório.");

        PessoaId = pessoaId;
        VeiculoId = veiculoId;
        DataHoraAgendada = DateTime.SpecifyKind(dataHoraAgendada, DateTimeKind.Utc);
        Motivo = motivo.Trim();
        Observacoes = observacoes?.Trim();
        Status = StatusAgendamento.Solicitado;
        CanalAtendimento = canalAtendimento;
    }

    public void Cancelar(string? observacoes = null)
    {
        if (Status is StatusAgendamento.Cancelado or StatusAgendamento.Concluido)
            throw new DomainException("Não é possível cancelar um agendamento finalizado.");

        Status = StatusAgendamento.Cancelado;
        Observacoes = observacoes?.Trim() ?? Observacoes;
        AtualizarDataModificacao();
    }

    public void Confirmar()
    {
        if (Status != StatusAgendamento.Solicitado)
            throw new DomainException("Somente agendamentos solicitados podem ser confirmados.");

        Status = StatusAgendamento.Confirmado;
        AtualizarDataModificacao();
    }
}