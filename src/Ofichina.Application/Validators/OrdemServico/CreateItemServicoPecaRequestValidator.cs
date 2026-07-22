using FluentValidation;
using Ofichina.Contracts.Requests.OrdemServico;

namespace Ofichina.Application.Validators.OrdemServico;

/// <summary>
/// Validador para inclusão de peça em item de serviço.
/// </summary>
public sealed class CreateItemServicoPecaRequestValidator : AbstractValidator<CreateItemServicoPecaRequest>
{
    public CreateItemServicoPecaRequestValidator()
    {
        RuleFor(x => x.PecaId)
            .NotEmpty().WithMessage("O identificador da peça é obrigatório.");

        RuleFor(x => x.Quantidade)
            .GreaterThan(0).WithMessage("A quantidade da peça deve ser maior que zero.");
    }
}