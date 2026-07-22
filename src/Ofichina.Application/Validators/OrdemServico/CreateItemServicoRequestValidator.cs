using FluentValidation;
using Ofichina.Contracts.Requests.ItemServico;

namespace Ofichina.Application.Validators.OrdemServico;

/// <summary>
/// Validador para criação de item de serviço da ordem de serviço.
/// </summary>
public sealed class CreateItemServicoRequestValidator : AbstractValidator<CreateItemServicoRequest>
{
    public CreateItemServicoRequestValidator()
    {
        RuleFor(x => x.OrdemServicoId)
            .NotEmpty().WithMessage("O identificador da ordem de serviço é obrigatório.");

        RuleFor(x => x.PecaServicoId)
            .NotEmpty().WithMessage("O identificador da peça de serviço é obrigatório.");
    }
}
