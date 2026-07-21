using FluentValidation;
using Ofichina.Contracts.Requests.OrdemServico;

namespace Ofichina.Application.Validators.OrdemServico;

/// <summary>
/// Validador para atualização de item de serviço da ordem de serviço.
/// </summary>
public sealed class UpdateOrdemServicoItemServicoRequestValidator : AbstractValidator<UpdateOrdemServicoItemServicoRequest>
{
    public UpdateOrdemServicoItemServicoRequestValidator()
    {
        RuleFor(x => x.ServicoId)
            .NotEmpty().WithMessage("O identificador do serviço é obrigatório.");

        RuleForEach(x => x.Pecas)
            .SetValidator(new UpdateOrdemServicoPecaRequestValidator());
    }
}
