using FluentValidation;

namespace Ofichina.Application.Validators.Agendamento
{
    public sealed class CadastraHorarioAgendamentoRequestValidatior : AbstractValidator<TimeOnly>
    {
        public CadastraHorarioAgendamentoRequestValidatior()
        {
            RuleFor(x => x)
                .NotEmpty().WithMessage("O horário é obrigatório.")
                .Must(horario => horario >= new TimeOnly(0, 0) && horario <= new TimeOnly(23, 59))
                .WithMessage("O horário deve estar entre 00:00 e 23:59.");
        }
    }
}
