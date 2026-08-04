using FluentValidation;
using Ofichina.Contracts.Requests.Orcamento;
using Ofichina.Contracts.Requests.ItensServico;

namespace Ofichina.Application.Validators.Orcamento;

/// <summary>
/// Validador para atualização de orçamento.
/// </summary>
public sealed class UpdateOrcamentoRequestValidator : AbstractValidator<UpdateOrcamentoRequest>
{
    public UpdateOrcamentoRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("O identificador do orçamento é obrigatório.");

        RuleFor(x => x.PessoaId)
            .NotEmpty().WithMessage("A pessoa vinculada é obrigatória.");

        RuleFor(x => x.VeiculoId)
            .NotEmpty().WithMessage("O veículo vinculado é obrigatório.");

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

        RuleFor(x => x.ItensServico)
            .NotEmpty().WithMessage("O orçamento deve conter ao menos um item de serviço.");

        RuleForEach(x => x.ItensServico).ChildRules(servico =>
        {
            servico.RuleFor(x => x.ServicoId)
                .NotEmpty().WithMessage("O serviço previsto é obrigatório.");

            servico.RuleFor(x => x.PecaId)
                .NotEmpty().WithMessage("A peça prevista é obrigatória.");

            servico.RuleFor(x => x.Quantidade)
                .GreaterThan(0).WithMessage("A quantidade da peça deve ser maior que zero.");
        });
    }
}
