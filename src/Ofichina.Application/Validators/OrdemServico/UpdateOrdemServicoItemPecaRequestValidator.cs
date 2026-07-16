using FluentValidation;
using Ofichina.Contracts.Requests.OrdemServico;

namespace Ofichina.Application.Validators.OrdemServico;

/// <summary>
/// Validador para atualização de item de peça da ordem de serviço.
/// </summary>
public sealed class UpdateOrdemServicoItemPecaRequestValidator : AbstractValidator<UpdateOrdemServicoItemPecaRequest>
{
    public UpdateOrdemServicoItemPecaRequestValidator()
    {
        RuleFor(x => x.PecaId)
            .NotEmpty().WithMessage("O identificador da peça é obrigatório.");

        RuleFor(x => x.Quantidade)
            .GreaterThan(0).WithMessage("A quantidade da peça deve ser maior que zero.");
    }
}
