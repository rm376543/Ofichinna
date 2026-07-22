using FluentValidation;
using Ofichina.Contracts.Requests.Servicos;

namespace Ofichina.Application.Validators.Servicos;

/// <summary>
/// Validador para inclusão de peça em serviço.
/// </summary>
public sealed class CreateServicoPecaRequestValidator : AbstractValidator<CreateServicoPecaRequest>
{
    public CreateServicoPecaRequestValidator()
    {
        RuleFor(x => x.ServicoId)
            .NotEmpty().WithMessage("O identificador do serviço é obrigatório.");

        RuleFor(x => x.PecaId)
            .NotEmpty().WithMessage("O identificador da peça é obrigatório.");

        RuleFor(x => x.Quantidade)
            .GreaterThan(0).WithMessage("A quantidade da peça deve ser maior que zero.");
    }
}