using FluentValidation;
using Ofichina.Contracts.Requests.OrdemServico;

namespace Ofichina.Application.Validators.OrdemServico;

/// <summary>
/// Validador para criação de item de serviço da ordem de serviço.
/// </summary>
public sealed class CreateItemServicoRequestValidator : AbstractValidator<CreateItemServicoRequest>
{
    public CreateItemServicoRequestValidator()
    {
        RuleFor(x => x.ServicoId)
            .NotEmpty().WithMessage("O identificador do serviço é obrigatório.");
    }
}
