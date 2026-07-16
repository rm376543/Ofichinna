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
        RuleFor(x => x.OrdemServicoItemId)
            .NotEmpty().WithMessage("O identificador do item de serviço é obrigatório.");

        RuleFor(x => x.Quantidade)
            .GreaterThan(0).WithMessage("A quantidade do serviço deve ser maior que zero.");
    }
}
