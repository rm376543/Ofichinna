using Ofichina.Domain.Entities;
using Ofichina.Domain.Exceptions;
using Ofichina.Domain.Enums;

namespace Ofichina.Domain.Aggregates;

/// <summary>
/// Representa um agendamento entre cliente, veículo e um horário de disponibilidade vinculado a um consultor.
/// </summary>
public class Agendamento : Entity
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
    /// Dia de disponibilidade associado ao agendamento.
    /// </summary>
    public Guid DiaDisponibilidadeId { get; private set; }

    /// <summary>
    /// Dia de disponibilidade associado ao agendamento.
    /// </summary>
    public DiaDisponibilidade DiaDisponibilidade { get; private set; } = null!;

    /// <summary>
    /// Vínculo entre horário e consultor selecionados para o agendamento.
    /// </summary>
    public Guid HorarioConsultorId { get; private set; }

    /// <summary>
    /// Vínculo entre horário e consultor selecionados para o agendamento.
    /// </summary>
    public HorarioConsultor HorarioConsultor { get; private set; } = null!;

    /// <summary>
    /// Pessoa consultora vinculada ao agendamento.
    /// </summary>
    public Guid ConsultorPessoaId { get; private set; }

    /// <summary>
    /// Pessoa consultora vinculada ao agendamento.
    /// </summary>
    public Pessoa Consultor { get; private set; } = null!;

    /// <summary>
    /// Veículo informado para o agendamento.
    /// </summary>
    public Guid VeiculoId { get; private set; }

    /// <summary>
    /// Veículo informado para o agendamento.
    /// </summary>
    public Veiculo Veiculo { get; private set; } = null!;

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
        Status = StatusAgendamento.Agendado;
        Descricao = string.IsNullOrWhiteSpace(descricao) ? null : descricao.Trim();
    }

    /// <summary>
    /// Confirma o agendamento, alterando seu status para "Confirmado".
    /// </summary>
    /// <exception cref="DomainException"></exception>
    public void Confirmar()
    {
        if (Status is StatusAgendamento.Cancelado or StatusAgendamento.Concluido)
            throw new DomainException("Não é possível confirmar um agendamento cancelado ou concluído.");

        if (Status == StatusAgendamento.Confirmado)
            return;

        Status = StatusAgendamento.Confirmado;
        AtualizarDataModificacao();
    }

    /// <summary>
    /// Cancela o agendamento, alterando seu status para "Cancelado".
    /// </summary>
    /// <exception cref="DomainException"></exception>
    public void Cancelar()
    {
        if (Status == StatusAgendamento.Concluido)
            throw new DomainException("Não é possível cancelar um agendamento concluído.");

        if (Status == StatusAgendamento.Cancelado)
            throw new DomainException("Não é possível cancelar um agendamento já cancelado.");

        Status = StatusAgendamento.Cancelado;
        AtualizarDataModificacao();
    }

    /// <summary>
    /// Conclui o agendamento, alterando seu status para "Concluído".
    /// </summary>
    /// <exception cref="DomainException"></exception>
    public void Concluir()
    {
        if (Status == StatusAgendamento.Cancelado)
            throw new DomainException("Não é possível concluir um agendamento cancelado.");

        if (Status == StatusAgendamento.Concluido)
            return;

        Status = StatusAgendamento.Concluido;
        AtualizarDataModificacao();
    }
}