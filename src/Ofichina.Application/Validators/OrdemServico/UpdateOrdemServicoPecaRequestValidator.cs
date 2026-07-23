using FluentValidation;
using Ofichina.Contracts.Requests.OrdensServico;

namespace Ofichina.Application.Validators.OrdensServico;

/// <summary>
/// Validador para peça atualizada na ordem de serviço.
/// </summary>
public sealed class UpdateOrdemServicoPecaRequestValidator : AbstractValidator<UpdateOrdemServicoPecaRequest>
{
    public UpdateOrdemServicoPecaRequestValidator()
    {
        RuleFor(x => x.PecaId)
            .NotEmpty().WithMessage("O identificador da peça é obrigatório.");

        RuleFor(x => x.Quantidade)
            .GreaterThan(0).WithMessage("A quantidade da peça deve ser maior que zero.");
    }
}
