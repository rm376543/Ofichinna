using FluentValidation;
using Ofichina.Contracts.Requests.Agendamento;

namespace Ofichina.Application.Validators.Agendamento;

/// <summary>
/// Validador para vínculo de consultor a horário.
/// </summary>
public sealed class VincularConsultorHorarioRequestValidator : AbstractValidator<VincularConsultorHorarioRequest>
{
    public VincularConsultorHorarioRequestValidator()
    {
        RuleFor(x => x.HorarioDisponibilidadeId)
            .NotEmpty().WithMessage("O horário disponível é obrigatório.");

        RuleFor(x => x.ConsultorPessoaId)
            .NotEmpty().WithMessage("O consultor é obrigatório.");
    }
}