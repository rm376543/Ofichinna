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
    /// Dia de disponibilidade associado ao agendamento (legado, nullable para migração).
    /// </summary>
    public Guid? DiaDisponibilidadeId { get; private set; }

    /// <summary>
    /// Dia de disponibilidade associado ao agendamento (legado).
    /// </summary>
    public DiaDisponibilidade? DiaDisponibilidade { get; private set; }

    /// <summary>
    /// Vínculo entre horário e consultor selecionados para o agendamento (legado, nullable para migração).
    /// </summary>
    public Guid? HorarioConsultorId { get; private set; }

    /// <summary>
    /// Vínculo entre horário e consultor selecionados para o agendamento (legado).
    /// </summary>
    public HorarioConsultor? HorarioConsultor { get; private set; }

    /// <summary>
    /// Pessoa consultora vinculada ao agendamento (legado, nullable para migração).
    /// </summary>
    public Guid? ConsultorPessoaId { get; private set; }

    /// <summary>
    /// Pessoa consultora vinculada ao agendamento (legado).
    /// </summary>
    public Pessoa? Consultor { get; private set; }

    /// <summary>
    /// Veículo informado para o agendamento.
    /// </summary>
    public Guid VeiculoId { get; private set; }

    /// <summary>
    /// Veículo informado para o agendamento.
    /// </summary>
    public Veiculo Veiculo { get; private set; } = null!;

    /// <summary>
    /// Identificador do slot de disponibilidade (HorarioConsultorDisponibilidade).
    /// </summary>
    public Guid HorarioConsultorDisponibilidadeId { get; private set; }

    /// <summary>
    /// Slot de disponibilidade vinculado (Dia + Horário + Consultor).
    /// </summary>
    public HorarioConsultorDisponibilidade HorarioConsultorDisponibilidade { get; private set; } = null!;

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
    /// Cria uma nova instância de agendamento com os parâmetros fornecidos.
    /// </summary>
    /// <param name="clientePessoaId"></param>
    /// <param name="diaDisponibilidadeId"></param>
    /// <param name="horarioConsultorId"></param>
    /// <param name="consultorPessoaId"></param>
    /// <param name="veiculoId"></param>
    /// <param name="descricao"></param>
    /// <exception cref="DomainException"></exception>
    public Agendamento(
        Guid clientePessoaId,
        Guid diaDisponibilidadeId,
        Guid horarioConsultorId,
        Guid consultorPessoaId,
        Guid veiculoId,
        string? descricao = null)
    {
        if (clientePessoaId == Guid.Empty)
            throw new DomainException("A pessoa é obrigatória.");

        if (diaDisponibilidadeId == Guid.Empty)
            throw new DomainException("O dia de disponibilidade é obrigatório.");

        if (horarioConsultorId == Guid.Empty)
            throw new DomainException("O horário do consultor é obrigatório.");

        if (consultorPessoaId == Guid.Empty)
            throw new DomainException("O consultor é obrigatório.");

        if (veiculoId == Guid.Empty)
            throw new DomainException("O veículo é obrigatório.");

        ClientePessoaId = clientePessoaId;
        DiaDisponibilidadeId = diaDisponibilidadeId;
        HorarioConsultorId = horarioConsultorId;
        ConsultorPessoaId = consultorPessoaId;
        VeiculoId = veiculoId;
        Status = StatusAgendamento.AGENDADO;
        Descricao = string.IsNullOrWhiteSpace(descricao) ? null : descricao.Trim();
    }

    /// <summary>
    /// Cria uma nova instância de agendamento usando o novo modelo de slot.
    /// </summary>
    /// <param name="clientePessoaId">ID da pessoa cliente.</param>
    /// <param name="horarioConsultorDisponibilidadeId">ID do slot de disponibilidade.</param>
    /// <param name="veiculoId">ID do veículo.</param>
    /// <param name="descricao">Descrição opcional.</param>
    /// <exception cref="DomainException"></exception>
    public Agendamento(
        Guid clientePessoaId,
        Guid horarioConsultorDisponibilidadeId,
        Guid veiculoId,
        string? descricao = null)
    {
        if (clientePessoaId == Guid.Empty)
            throw new DomainException("A pessoa cliente é obrigatória.");

        if (horarioConsultorDisponibilidadeId == Guid.Empty)
            throw new DomainException("O slot de disponibilidade é obrigatório.");

        if (veiculoId == Guid.Empty)
            throw new DomainException("O veículo é obrigatório.");

        ClientePessoaId = clientePessoaId;
        HorarioConsultorDisponibilidadeId = horarioConsultorDisponibilidadeId;
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