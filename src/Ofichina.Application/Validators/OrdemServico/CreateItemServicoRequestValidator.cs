using FluentValidation;
using Ofichina.Contracts.Requests.ItensServico;

namespace Ofichina.Application.Validators.OrdensServico;

/// <summary>
/// Validador para criação de item de serviço da ordem de serviço.
/// </summary>
public sealed class CreateItemServicoRequestValidator : AbstractValidator<CreateItemServicoRequest>
{
    public CreateItemServicoRequestValidator()
    {
        RuleFor(x => x.OrdemServicoId)
            .NotEmpty().WithMessage("O identificador da ordem de serviço é obrigatório.");

        RuleFor(x => x.Pecas)
            .NotEmpty().WithMessage("Informe ao menos uma peça para o item de serviço.");
    }
}
