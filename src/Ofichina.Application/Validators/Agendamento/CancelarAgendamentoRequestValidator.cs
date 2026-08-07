using FluentValidation;
using Ofichina.Contracts.Requests.Agendamento;

namespace Ofichina.Application.Validators.Agendamento
{
    /// <summary>
    /// Validator para a requisição de cancelamento de agendamento.
    /// </summary>
    public class CancelarAgendamentoRequestValidator : AbstractValidator<CancelarAgendamentoRequest>
    {
        public CancelarAgendamentoRequestValidator()
        {
            RuleFor(x => x.AgendamentoId)
                .NotEmpty().WithMessage("O ID do agendamento é obrigatório.");
        }
    }
}
