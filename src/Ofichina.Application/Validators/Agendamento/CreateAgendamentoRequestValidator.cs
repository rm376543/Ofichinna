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
        RuleFor(x => x.ConsultorPessoaId)
            .NotEmpty().WithMessage("O consultor é obrigatório.");

        RuleFor(x => x.DataAgendamento)
            .NotEqual(default(DateOnly)).WithMessage("A data do agendamento é obrigatória.");

        RuleFor(x => x.VeiculoId)
            .NotEmpty().WithMessage("O veículo é obrigatório.");

        RuleFor(x => x.HorarioAgendamento)
            .NotEqual(default(TimeOnly)).WithMessage("O horário do agendamento é obrigatório.");

        RuleFor(x => x)
            .Must(x => x.DataAgendamento.ToDateTime(x.HorarioAgendamento) > DateTime.Now)
            .WithMessage("A data e o horário do agendamento devem estar no futuro.")
            .When(x => x.DataAgendamento != default && x.HorarioAgendamento != default);

        RuleFor(x => x.Descricao)
            .MaximumLength(1000).WithMessage("A descrição do agendamento não pode exceder 1000 caracteres.");
    }
}