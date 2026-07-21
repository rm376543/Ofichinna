using FluentValidation;
using Ofichina.Contracts.Requests.OrdemServico;

namespace Ofichina.Application.Validators.OrdemServico;

/// <summary>
/// Validador para item de serviço da ordem de serviço.
/// </summary>
public sealed class CreateOrdemServicoItemServicoRequestValidator : AbstractValidator<CreateOrdemServicoItemServicoRequest>
{
    public CreateOrdemServicoItemServicoRequestValidator()
    {
        RuleFor(x => x.ServicoId)
            .NotEmpty().WithMessage("O identificador do serviço é obrigatório.");

        RuleForEach(x => x.Pecas)
            .SetValidator(new CreateOrdemServicoPecaRequestValidator());
    }
}
