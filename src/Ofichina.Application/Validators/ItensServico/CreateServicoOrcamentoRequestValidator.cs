using FluentValidation;
using Ofichina.Contracts.Requests.ItensServico;

namespace Ofichina.Application.Validators.ItensServico;

/// <summary>
/// Validador para criação de item de serviço somente-serviço do orçamento.
/// </summary>
public sealed class CreateServicoOrcamentoRequestValidator : AbstractValidator<CreateServicoOrcamentoRequest>
{
    public CreateServicoOrcamentoRequestValidator()
    {
        RuleFor(x => x.OrcamentoId)
            .NotEmpty().WithMessage("O identificador do orçamento é obrigatório.");

        RuleFor(x => x.ServicoId)
            .NotEmpty().WithMessage("O identificador do serviço é obrigatório.");
    }
}