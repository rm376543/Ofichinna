using FluentValidation;
using Ofichina.Contracts.Requests.Orcamento;

namespace Ofichina.Application.Validators.Orcamento;

/// <summary>
/// Validador para atualização de orçamento.
/// </summary>
public sealed class UpdateOrcamentoRequestValidator : AbstractValidator<UpdateOrcamentoRequest>
{
    public UpdateOrcamentoRequestValidator()
    {
        RuleFor(x => x.OrcamentoId)
            .NotEmpty().WithMessage("O Id do orçamento é obrigatório.");

        RuleFor(x => x.PessoaId)
            .NotEmpty().WithMessage("A pessoa vinculada é obrigatória.");

        RuleFor(x => x.VeiculoId)
            .NotEmpty().WithMessage("O veículo vinculado é obrigatório.");

        RuleFor(x => x.ConsultorId)
            .NotEmpty().WithMessage("O consultor do orçamento é obrigatório.");

        RuleFor(x => x.MecanicoId)
            .NotEmpty().WithMessage("O mecânico do diagnóstico é obrigatório.");

        RuleFor(x => x.DataValidade)
            .NotEmpty().WithMessage("A data de validade é obrigatória.");

        RuleFor(x => x.Observacoes)
            .MaximumLength(1000).WithMessage("As observações não podem exceder 1000 caracteres.")
            .When(x => !string.IsNullOrWhiteSpace(x.Observacoes));

        RuleFor(x => x.ItensServico)
            .NotEmpty().WithMessage("O orçamento deve conter ao menos um item de serviço.");

        RuleForEach(x => x.ItensServico)
            .SetValidator(new OrcamentoItemServicoRequestValidator());
    }
}
