using FluentValidation;
using Ofichina.Contracts.Requests.Permissoes;

namespace Ofichina.Application.Validators.Permissoes;

public sealed class UpdatePermissaoRequestValidator : AbstractValidator<UpdatePermissaoRequest>
{
    public UpdatePermissaoRequestValidator()
    {
        RuleFor(x => x.PermissaoId)
            .NotEmpty().WithMessage("O Id da permissão é obrigatório.");

        RuleFor(x => x.Codigo)
            .NotEmpty().WithMessage("O código da permissão é obrigatório.")
            .MaximumLength(150).WithMessage("O código da permissão não pode exceder 150 caracteres.");

        RuleFor(x => x.Descricao)
            .NotEmpty().WithMessage("A descrição da permissão é obrigatória.")
            .MaximumLength(200).WithMessage("A descrição da permissão não pode exceder 200 caracteres.");
    }
}
