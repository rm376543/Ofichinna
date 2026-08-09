using FluentValidation;
using Ofichina.Contracts.Requests.Orcamento;

namespace Ofichina.Application.Validators.Orcamento;

/// <summary>
/// Validador para atualização de desconto do orçamento.
/// </summary>
public sealed class UpdateOrcamentoDescontoRequestValidator : AbstractValidator<UpdateOrcamentoDescontoRequest>
{
    public UpdateOrcamentoDescontoRequestValidator()
    {
        RuleFor(x => x.Desconto)
            .GreaterThanOrEqualTo(0).WithMessage("O desconto não pode ser negativo.");
    }
}
