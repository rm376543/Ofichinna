using FluentValidation;
using Ofichina.Contracts.Requests.Pecas;

namespace Ofichina.Application.Validators.Pecas;

/// <summary>
/// Validador para criação de peça.
/// </summary>
public sealed class CreatePecaRequestValidator : AbstractValidator<CreatePecaRequest>
{
    /// <summary>
    /// Inicializa as regras de validação da criação de peça.
    /// </summary>
    public CreatePecaRequestValidator()
    {
        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("O nome da peça é obrigatório.")
            .MaximumLength(150).WithMessage("O nome da peça não pode exceder 150 caracteres.");

        RuleFor(x => x.Descricao)
            .MaximumLength(500).WithMessage("A descrição da peça não pode exceder 500 caracteres.")
            .When(x => !string.IsNullOrWhiteSpace(x.Descricao));

        RuleFor(x => x.Codigo)
            .NotEmpty().WithMessage("O código da peça é obrigatório.")
            .MaximumLength(50).WithMessage("O código da peça não pode exceder 50 caracteres.");

        RuleFor(x => x.Valor)
            .GreaterThan(0).WithMessage("O valor da peça deve ser maior que zero.");

        RuleFor(x => x.QuantidadeEstoque)
            .GreaterThanOrEqualTo(0).WithMessage("A quantidade em estoque não pode ser negativa.");
    }
}