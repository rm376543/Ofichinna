using FluentValidation;
using Ofichina.Contracts.Requests.Agendamento;

namespace Ofichina.Application.Validators.Agendamento;

/// <summary>
/// Validador para criação de agendamento usando o novo modelo com AgendaConsultor.
/// </summary>
public sealed class CreateAgendamentoRequestValidator : AbstractValidator<CreateAgendamentoRequest>
{
    public CreateAgendamentoRequestValidator()
    {
        RuleFor(x => x.AgendaConsultorId)
            .NotEmpty().WithMessage("O slot de disponibilidade (dia + horário + consultor) é obrigatório.");

        RuleFor(x => x.VeiculoId)
            .NotEmpty().WithMessage("O veículo é obrigatório.");

        RuleFor(x => x.Descricao)
            .MaximumLength(1000).WithMessage("A descrição do agendamento não pode exceder 1000 caracteres.");
    }
}