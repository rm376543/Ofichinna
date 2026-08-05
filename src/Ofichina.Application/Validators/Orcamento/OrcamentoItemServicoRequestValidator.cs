using FluentValidation;
using Ofichina.Contracts.Requests.Orcamento;

namespace Ofichina.Application.Validators.Orcamento;

/// <summary>
/// Validador para item de serviço do orçamento.
/// </summary>
public sealed class OrcamentoItemServicoRequestValidator : AbstractValidator<OrcamentoItemServicoRequest>
{
    public OrcamentoItemServicoRequestValidator()
    {
        RuleFor(x => x.ServicoId)
            .NotEmpty().WithMessage("O serviço previsto é obrigatório.");

        RuleFor(x => x.PecaId)
            .NotEmpty().When(x => x.PecaId.HasValue).WithMessage("A peça prevista é obrigatória.");

        RuleFor(x => x.Quantidade)
            .GreaterThan(0).WithMessage("A quantidade da peça deve ser maior que zero.");
    }
}
