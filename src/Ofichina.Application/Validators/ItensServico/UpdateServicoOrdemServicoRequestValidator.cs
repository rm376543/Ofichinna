using FluentValidation;
using Ofichina.Contracts.Requests.ItensServico;

namespace Ofichina.Application.Validators.ItensServico;

/// <summary>
/// Validador para atualização de item de serviço somente-serviço da ordem de serviço.
/// </summary>
public sealed class UpdateServicoOrdemServicoRequestValidator : AbstractValidator<UpdateServicoOrdemServicoRequest>
{
    public UpdateServicoOrdemServicoRequestValidator()
    {
        RuleFor(x => x.OrdemServicoId)
            .NotEmpty().WithMessage("O identificador da ordem de serviço é obrigatório.");

        RuleFor(x => x.ItemServicoId)
            .NotEmpty().WithMessage("O Id do item de serviço é obrigatório.");

        RuleFor(x => x.ServicoId)
            .NotEmpty().WithMessage("O identificador do serviço é obrigatório.");
    }
}