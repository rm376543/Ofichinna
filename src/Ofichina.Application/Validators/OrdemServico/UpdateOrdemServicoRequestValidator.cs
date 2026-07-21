using FluentValidation;
using Ofichina.Contracts.Requests.OrdemServico;

namespace Ofichina.Application.Validators.OrdemServico;

/// <summary>
/// Validador para atualização de ordem de serviço.
/// </summary>
public sealed class UpdateOrdemServicoRequestValidator : AbstractValidator<UpdateOrdemServicoRequest>
{
    public UpdateOrdemServicoRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("O identificador da ordem de serviço é obrigatório.");

        RuleFor(x => x.FuncionarioId)
            .NotEmpty().WithMessage("O funcionário responsável é obrigatório.");

        RuleFor(x => x.ProblemaRelatado)
            .NotEmpty().WithMessage("O problema relatado é obrigatório.")
            .MaximumLength(500).WithMessage("O problema relatado não pode exceder 500 caracteres.");

        RuleFor(x => x.Observacoes)
            .MaximumLength(1000).WithMessage("As observações não podem exceder 1000 caracteres.")
            .When(x => !string.IsNullOrWhiteSpace(x.Observacoes));

        RuleForEach(x => x.Servicos)
            .SetValidator(new UpdateOrdemServicoItemServicoRequestValidator());
    }
}
