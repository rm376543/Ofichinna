using FluentValidation;
using Ofichina.Contracts.Requests.Orcamento;
using Ofichina.Contracts.Requests.Orcamentos;

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

        RuleFor(x => x)
            .Must(x => x.Servicos.Any() || x.Pecas.Any())
            .WithMessage("O orçamento deve conter ao menos um serviço ou uma peça.");

        RuleForEach(x => x.Servicos).ChildRules(servico =>
        {
            servico.RuleFor(x => x.ServicoId)
                .NotEmpty().WithMessage("O serviço previsto é obrigatório.");

            servico.RuleFor(x => x.Quantidade)
                .GreaterThan(0).WithMessage("A quantidade do serviço deve ser maior que zero.");

            servico.RuleFor(x => x.ValorUnitario)
                .GreaterThanOrEqualTo(0).WithMessage("O valor unitário do serviço não pode ser negativo.");

            servico.RuleFor(x => x.Observacoes)
                .MaximumLength(500).WithMessage("As observações do serviço não podem exceder 500 caracteres.")
                .When(x => !string.IsNullOrWhiteSpace(x.Observacoes));
        });

        RuleForEach(x => x.Pecas).ChildRules(peca =>
        {
            peca.RuleFor(x => x.PecaId)
                .NotEmpty().WithMessage("A peça prevista é obrigatória.");

            peca.RuleFor(x => x.Quantidade)
                .GreaterThan(0).WithMessage("A quantidade da peça deve ser maior que zero.");

            peca.RuleFor(x => x.ValorUnitario)
                .GreaterThanOrEqualTo(0).WithMessage("O valor unitário da peça não pode ser negativo.");

            peca.RuleFor(x => x.Desconto)
                .GreaterThanOrEqualTo(0).WithMessage("O desconto da peça não pode ser negativo.");
        });
    }
}
