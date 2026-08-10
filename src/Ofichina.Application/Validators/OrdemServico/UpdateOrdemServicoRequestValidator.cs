using FluentValidation;
using Ofichina.Contracts.Requests.OrdensServico;

namespace Ofichina.Application.Validators.OrdensServico;

/// <summary>
/// Validador para atualização de ordem de serviço.
/// </summary>
public sealed class UpdateOrdemServicoRequestValidator : AbstractValidator<UpdateOrdemServicoRequest>
{
    public UpdateOrdemServicoRequestValidator()
    {
        RuleFor(x => x.OrdemServicoId)
            .NotEmpty().WithMessage("O identificador da ordem de serviço é obrigatório.");

        RuleFor(x => x.PessoaId)
            .NotEmpty().WithMessage("A pessoa vinculada é obrigatória.");

        RuleFor(x => x.VeiculoId)
            .NotEmpty().WithMessage("O veículo vinculado é obrigatório.");

        RuleFor(x => x.ConsultorId)
            .NotEmpty().WithMessage("O consultor responsável é obrigatório.");

        RuleFor(x => x.Hodometro)
            .GreaterThanOrEqualTo(0).WithMessage("O hodômetro de entrada não pode ser negativo.");

        RuleFor(x => x.ProblemaRelatado)
            .NotEmpty().WithMessage("O problema relatado é obrigatório.")
            .MaximumLength(500).WithMessage("O problema relatado não pode exceder 500 caracteres.");

        RuleFor(x => x.Observacoes)
            .MaximumLength(1000).WithMessage("As observações não podem exceder 1000 caracteres.")
            .When(x => !string.IsNullOrWhiteSpace(x.Observacoes));
    }
}
