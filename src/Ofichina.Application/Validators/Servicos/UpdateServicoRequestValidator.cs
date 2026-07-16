using FluentValidation;
using Ofichina.Contracts.Requests.Servicos;

namespace Ofichina.Application.Validators.Servicos;

/// <summary>
/// Validador para atualização de serviço.
/// </summary>
public sealed class UpdateServicoRequestValidator : AbstractValidator<UpdateServicoRequest>
{
    public UpdateServicoRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("O identificador do serviço é obrigatório.");

        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("O nome do serviço é obrigatório.")
            .MinimumLength(3).WithMessage("O nome do serviço deve ter pelo menos 3 caracteres.")
            .MaximumLength(150).WithMessage("O nome do serviço não pode exceder 150 caracteres.");

        RuleFor(x => x.Descricao)
            .MaximumLength(500).WithMessage("A descrição não pode exceder 500 caracteres.")
            .When(x => !string.IsNullOrWhiteSpace(x.Descricao));

        RuleFor(x => x.Valor)
            .GreaterThan(0).WithMessage("O valor do serviço deve ser maior que zero.");
    }
}