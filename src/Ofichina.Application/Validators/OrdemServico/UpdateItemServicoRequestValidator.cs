using FluentValidation;
using Ofichina.Contracts.Requests.ItemServico;

namespace Ofichina.Application.Validators.OrdemServico;

/// <summary>
/// Validador para atualização de item de serviço da ordem de serviço.
/// </summary>
public sealed class UpdateItemServicoRequestValidator : AbstractValidator<UpdateItemServicoRequest>
{
    public UpdateItemServicoRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("O identificador do item de serviço é obrigatório.");

        RuleFor(x => x.PecaServicoId)
            .NotEmpty().WithMessage("O identificador da peça de serviço é obrigatório.");
    }
}
