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
        RuleFor(x => x.VeiculoId)
            .NotEmpty().WithMessage("O veículo é obrigatório.");

        RuleFor(x => x.DataHoraPreferida)
            .Must(dataHora => dataHora > DateTime.UtcNow).WithMessage("A data e hora do agendamento devem ser futuras.");

        RuleFor(x => x.Motivo)
            .NotEmpty().WithMessage("O motivo do agendamento é obrigatório.")
            .MaximumLength(200).WithMessage("O motivo do agendamento não pode exceder 200 caracteres.");

        RuleFor(x => x.Observacoes)
            .MaximumLength(1000).WithMessage("As observações não podem exceder 1000 caracteres.")
            .When(x => !string.IsNullOrWhiteSpace(x.Observacoes));
    }
}