using Ofichina.Domain.Entities;
using Ofichina.Domain.Enums;
using Ofichina.Domain.Exceptions;

namespace Ofichina.Domain.Aggregates;

/// <summary>
/// Representa um agendamento entre cliente, veículo e um horário de disponibilidade vinculado a um consultor.
/// </summary>
public sealed class Agendamento : Entity
{
    /// <summary>
    /// Pessoa cliente vinculada ao agendamento.
    /// </summary>
    public Guid ClientePessoaId { get; private set; }

    /// <summary>
    /// Pessoa cliente vinculada ao agendamento.
    /// </summary>
    public Pessoa Cliente { get; private set; } = null!;

    /// <summary>
    /// Veículo informado para o agendamento.
    /// </summary>
    public Guid VeiculoId { get; private set; }

    /// <summary>
    /// Veículo informado para o agendamento.
    /// </summary>
    public Veiculo Veiculo { get; private set; } = null!;

    /// <summary>
    /// Identificador do slot de disponibilidade (AgendaConsultor).
    /// </summary>
    public Guid AgendaConsultorId { get; private set; }

    /// <summary>
    /// Slot de disponibilidade vinculado (Dia + Horário + Consultor).
    /// </summary>
    public AgendaConsultor AgendaConsultor { get; private set; } = null!;

    /// <summary>
    /// Status atual do agendamento.
    /// </summary>
    public StatusAgendamento Status { get; private set; }

    /// <summary>
    /// Descrição opcional do agendamento.
    /// </summary>
    public string? Descricao { get; private set; }

    /// <summary>
    /// Construtor privado para uso interno da entidade.
    /// </summary>
    private Agendamento()
    {
    }

    /// <summary>
    /// Cria uma nova instância de agendamento usando o novo modelo de slot.
    /// </summary>
    /// <param name="clientePessoaId">ID da pessoa cliente.</param>
    /// <param name="agendaConsultorId">ID do slot de disponibilidade.</param>
    /// <param name="veiculoId">ID do veículo.</param>
    /// <param name="descricao">Descrição opcional.</param>
    /// <exception cref="DomainException"></exception>
    public Agendamento(
        Guid clientePessoaId,
        Guid agendaConsultorId,
        Guid veiculoId,
        string? descricao = null)
    {
        if (clientePessoaId == Guid.Empty)
            throw new DomainException("A pessoa cliente é obrigatória.");

        if (agendaConsultorId == Guid.Empty)
            throw new DomainException("O slot de disponibilidade é obrigatório.");

        if (veiculoId == Guid.Empty)
            throw new DomainException("O veículo é obrigatório.");

        ClientePessoaId = clientePessoaId;
        AgendaConsultorId = agendaConsultorId;
        VeiculoId = veiculoId;
        Status = StatusAgendamento.AGENDADO;
        Descricao = string.IsNullOrWhiteSpace(descricao) ? null : descricao.Trim();
    }

    /// <summary>
    /// Inicia o agendamento, alterando seu status de "AGENDADO" para "INICIADO".
    /// </summary>
    /// <exception cref="DomainException"></exception>
    public void Iniciar()
    {
        if (Status == StatusAgendamento.CANCELADO)
            throw new DomainException("Não é possível iniciar um agendamento cancelado.");

        if (Status != StatusAgendamento.AGENDADO)
            throw new DomainException("Apenas agendamentos com status 'AGENDADO' podem ser iniciados.");

        Status = StatusAgendamento.INICIADO;
        AtualizarDataModificacao();
    }

    /// <summary>
    /// Finaliza o agendamento, alterando seu status de "INICIADO" para "FINALIZADO".
    /// </summary>
    /// <exception cref="DomainException"></exception>
    public void Finalizar()
    {
        if (Status != StatusAgendamento.INICIADO)
            throw new DomainException("Apenas agendamentos com status 'INICIADO' podem ser finalizados.");

        Status = StatusAgendamento.FINALIZADO;
        AtualizarDataModificacao();
    }

    /// <summary>
    /// Cancela o agendamento, alterando seu status para "CANCELADO".
    /// Permite cancelamento quando o status é "AGENDADO" ou "INICIADO" (no-show).
    /// </summary>
    /// <exception cref="DomainException"></exception>
    public void Cancelar()
    {
        if (Status == StatusAgendamento.FINALIZADO)
            throw new DomainException("Não é possível cancelar um agendamento finalizado.");

        if (Status == StatusAgendamento.CANCELADO)
            throw new DomainException("Não é possível cancelar um agendamento já cancelado.");

        Status = StatusAgendamento.CANCELADO;
        AtualizarDataModificacao();
    }
}