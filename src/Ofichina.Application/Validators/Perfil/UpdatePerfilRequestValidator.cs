using FluentValidation;
using Ofichina.Contracts.Requests.Perfil;

namespace Ofichina.Application.Validators.Perfil;

/// <summary>
/// Validador para atualização de perfil.
/// </summary>
public sealed class UpdatePerfilRequestValidator : AbstractValidator<UpdatePerfilRequest>
{
    public UpdatePerfilRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("O Id do perfil é obrigatório.");

        RuleFor(x => x.Codigo)
            .NotEmpty().WithMessage("O código do perfil é obrigatório.")
            .MaximumLength(50).WithMessage("O código do perfil não pode exceder 50 caracteres.");

        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("O nome do perfil é obrigatório.")
            .MaximumLength(150).WithMessage("O nome do perfil não pode exceder 150 caracteres.");

        RuleFor(x => x.Descricao)
            .MaximumLength(300).WithMessage("A descrição não pode exceder 300 caracteres.");
    }
}