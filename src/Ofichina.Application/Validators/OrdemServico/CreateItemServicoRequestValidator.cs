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
        RuleFor(x => x.Descricao)
            .NotEmpty().WithMessage("A descrição do serviço é obrigatória.")
            .MaximumLength(200).WithMessage("A descrição do serviço não pode exceder 200 caracteres.");

        RuleFor(x => x.Valor)
            .GreaterThan(0).WithMessage("O valor do serviço deve ser maior que zero.");
    }
}
