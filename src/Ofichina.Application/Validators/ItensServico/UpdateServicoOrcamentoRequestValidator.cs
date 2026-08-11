using FluentValidation;
using Ofichina.Contracts.Requests.ItensServico;

namespace Ofichina.Application.Validators.ItensServico;

/// <summary>
/// Validador para atualização de item de serviço somente-serviço do orçamento.
/// </summary>
public sealed class UpdateServicoOrcamentoRequestValidator : AbstractValidator<UpdateServicoOrcamentoRequest>
{
    public UpdateServicoOrcamentoRequestValidator()
    {
        RuleFor(x => x.OrcamentoId)
            .NotEmpty().WithMessage("O identificador do orçamento é obrigatório.");

        RuleFor(x => x.ItemServicoId)
            .NotEmpty().WithMessage("O Id do item de serviço é obrigatório.");

        RuleFor(x => x.ServicoId)
            .NotEmpty().WithMessage("O identificador do serviço é obrigatório.");
    }
}