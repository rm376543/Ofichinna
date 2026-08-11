using FluentValidation;
using Ofichina.Contracts.Requests.ItensServico;

namespace Ofichina.Application.Validators.ItensServico;

/// <summary>
/// Validador para criação de item de serviço somente-serviço da ordem de serviço.
/// </summary>
public sealed class CreateServicoOrdemServicoRequestValidator : AbstractValidator<CreateServicoOrdemServicoRequest>
{
    public CreateServicoOrdemServicoRequestValidator()
    {
        RuleFor(x => x.OrdemServicoId)
            .NotEmpty().WithMessage("O identificador da ordem de serviço é obrigatório.");

        RuleFor(x => x.ServicoId)
            .NotEmpty().WithMessage("O identificador do serviço é obrigatório.");
    }
}