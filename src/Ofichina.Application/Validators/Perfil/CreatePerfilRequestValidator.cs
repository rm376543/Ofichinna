using FluentValidation;
using Ofichina.Contracts.Requests.Perfil;

namespace Ofichina.Application.Validators;

/// <summary>
/// Validador para criação de perfil.
/// </summary>
public class CreatePerfilRequestValidator : AbstractValidator<CreatePerfilRequest>
{
    public CreatePerfilRequestValidator()
    {
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