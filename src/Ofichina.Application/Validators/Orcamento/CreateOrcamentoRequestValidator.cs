using FluentValidation;
using Ofichina.Contracts.Requests.Orcamento;

namespace Ofichina.Application.Validators.Orcamento;

/// <summary>
/// Validador para criação de orçamento.
/// </summary>
public sealed class CreateOrcamentoRequestValidator : AbstractValidator<CreateOrcamentoRequest>
{
    public CreateOrcamentoRequestValidator()
    {
        RuleFor(x => x.PessoaId)
            .NotEmpty().WithMessage("A pessoa vinculada é obrigatória.");

        RuleFor(x => x.VeiculoId)
            .NotEmpty().WithMessage("O veículo vinculado é obrigatório.");

        RuleFor(x => x.ChecklistId)
            .NotEmpty().WithMessage("O checklist de origem é obrigatório.");

        RuleFor(x => x.ResponsavelId)
            .NotEmpty().WithMessage("O responsável pelo orçamento é obrigatório.");

        RuleFor(x => x.MecanicoDiagnosticoId)
            .NotEmpty().WithMessage("O mecânico do diagnóstico é obrigatório.");

        RuleFor(x => x.DataValidade)
            .NotEmpty().WithMessage("A data de validade é obrigatória.");

        RuleFor(x => x.Desconto)
            .GreaterThanOrEqualTo(0).WithMessage("O desconto não pode ser negativo.");

        RuleFor(x => x.Observacoes)
            .MaximumLength(1000).WithMessage("As observações não podem exceder 1000 caracteres.")
            .When(x => !string.IsNullOrWhiteSpace(x.Observacoes));

    }
}
