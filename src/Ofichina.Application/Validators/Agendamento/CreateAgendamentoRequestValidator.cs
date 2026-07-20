using FluentValidation;
using Ofichina.Contracts.Requests.Agendamento;

namespace Ofichina.Application.Validators.Agendamento;

/// <summary>
/// Validador para criação de agendamento.
/// </summary>
public sealed class CreateAgendamentoRequestValidator : AbstractValidator<CreateAgendamentoRequest>
{
    public CreateAgendamentoRequestValidator()
    {
        RuleFor(x => x.DiaDisponibilidadeId)
            .NotEmpty().WithMessage("O dia de disponibilidade é obrigatório.");

        RuleFor(x => x.HorarioConsultorId)
            .NotEmpty().WithMessage("O horário do consultor é obrigatório.");

        RuleFor(x => x.VeiculoId)
            .NotEmpty().WithMessage("O veículo é obrigatório.");

        RuleFor(x => x.Descricao)
            .MaximumLength(1000).WithMessage("A descrição do agendamento não pode exceder 1000 caracteres.");
    }
}