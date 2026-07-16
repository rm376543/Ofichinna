using FluentValidation;
using Ofichina.Contracts.Requests.OrdemServico;

namespace Ofichina.Application.Validators.OrdemServico;

/// <summary>
/// Validador para item de peça da ordem de serviço.
/// </summary>
public sealed class CreateOrdemServicoItemPecaRequestValidator : AbstractValidator<CreateOrdemServicoItemPecaRequest>
{
    public CreateOrdemServicoItemPecaRequestValidator()
    {
        RuleFor(x => x.OrdemServicoItemPecaId)
            .NotEmpty().WithMessage("O identificador do item de peça é obrigatório.");

        RuleFor(x => x.Quantidade)
            .GreaterThan(0).WithMessage("A quantidade da peça deve ser maior que zero.");
    }
}
