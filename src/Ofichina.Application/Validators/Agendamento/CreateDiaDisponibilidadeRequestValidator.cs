using FluentValidation;
using Ofichina.Contracts.Requests.Agendamento;

namespace Ofichina.Application.Validators.Agendamento;

/// <summary>
/// Validador para cadastro de dia de disponibilidade.
/// </summary>
public sealed class CreateDiaDisponibilidadeRequestValidator : AbstractValidator<CreateDiaDisponibilidadeRequest>
{
    public CreateDiaDisponibilidadeRequestValidator()
    {
        RuleFor(x => x.Data)
            .NotEqual(default(DateOnly)).WithMessage("A data da disponibilidade é obrigatória.");

        RuleFor(x => x.Horarios)
            .NotNull().WithMessage("Os horários da disponibilidade são obrigatórios.")
            .Must(x => x.Count > 0).WithMessage("Informe ao menos um horário para o dia de disponibilidade.");

        RuleForEach(x => x.Horarios)
            .NotEqual(default(TimeOnly)).WithMessage("O horário da disponibilidade é obrigatório.");
    }
}