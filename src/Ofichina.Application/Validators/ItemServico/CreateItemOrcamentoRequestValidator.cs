using FluentValidation;
using Ofichina.Contracts.Requests.ItensServico;

namespace Ofichina.Application.Validators.ItensServico;

/// <summary>
/// Validador para criação de item de serviço do orçamento.
/// </summary>
public sealed class CreateItemOrcamentoRequestValidator : AbstractValidator<CreateItemOrcamentoRequest>
{
    public CreateItemOrcamentoRequestValidator()
    {
        RuleFor(x => x.OrcamentoId)
            .NotEmpty().WithMessage("O identificador do orçamento é obrigatório.");

        RuleFor(x => x.ServicoId)
            .NotEmpty().WithMessage("O identificador do serviço é obrigatório.");

        RuleFor(x => x.Quantidade)
            .GreaterThan(0).WithMessage("A quantidade deve ser maior que zero.");
    }
}
