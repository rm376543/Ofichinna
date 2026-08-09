using FluentValidation;
using Ofichina.Contracts.Requests.ItensServico;

namespace Ofichina.Application.Validators.ItensServico;

/// <summary>
/// Validador para atualização de item de serviço do orçamento.
/// </summary>
public sealed class UpdateItemOrcamentoRequestValidator : AbstractValidator<UpdateItemOrcamentoRequest>
{
    public UpdateItemOrcamentoRequestValidator()
    {
        RuleFor(x => x.OrcamentoId)
            .NotEmpty().WithMessage("O identificador do orçamento é obrigatório.");

        RuleFor(x => x.ItemServicoId)
            .NotEmpty().WithMessage("O Id do item de serviço é obrigatório.");

        RuleFor(x => x.ServicoId)
            .NotEmpty().WithMessage("O identificador do serviço é obrigatório.");

        RuleFor(x => x.Quantidade)
            .GreaterThan(0).WithMessage("A quantidade de peças deve ser maior que zero.");
    }
}
