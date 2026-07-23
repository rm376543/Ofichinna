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
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("O identificador do item de serviço é obrigatório.");

        RuleFor(x => x.Pecas)
            .NotEmpty().WithMessage("Informe ao menos uma peça para o item de serviço.");
    }
}
