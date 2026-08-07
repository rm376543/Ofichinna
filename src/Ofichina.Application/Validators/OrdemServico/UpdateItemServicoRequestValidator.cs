using FluentValidation;
using Ofichina.Contracts.Requests.ItensServico;

namespace Ofichina.Application.Validators.OrdensServico;

/// <summary>
/// Validador para atualização de item de serviço da ordem de serviço.
/// </summary>
public sealed class UpdateItemServicoRequestValidator : AbstractValidator<UpdateItemServicoRequest>
{
    public UpdateItemServicoRequestValidator()
    {
        RuleFor(x => x.OrdemServicoId)
            .NotEmpty().WithMessage("O identificador da ordem de serviço é obrigatório.");

        RuleFor(x => x.ServicoId)
            .NotEmpty().WithMessage("O identificador do serviço é obrigatório.");

        RuleFor(x => x.PecaId)
            .NotEmpty().WithMessage("O identificador da peça é obrigatório.");

        RuleFor(x => x.Quantidade)
            .GreaterThan(0).WithMessage("A quantidade de peças deve ser maior que zero.");

        RuleFor(x => x.ItemServicoId)
            .NotEmpty().WithMessage("O Id do item de serviço é obrigatório.");
    }
}
