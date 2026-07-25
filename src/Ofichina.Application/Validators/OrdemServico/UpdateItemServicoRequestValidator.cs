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

        RuleFor(x => x.ServicoPecaId)
            .NotEmpty().WithMessage("O identificador do serviço/peça é obrigatório.");
    }
}
