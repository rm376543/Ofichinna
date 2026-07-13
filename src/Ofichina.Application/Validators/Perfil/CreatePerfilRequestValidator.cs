using FluentValidation;
using Ofichina.Contracts.Requests.Perfil;

namespace Ofichina.Application.Validators.Perfil;

/// <summary>
/// Validador para criação de perfil.
/// </summary>
public sealed class CreatePerfilRequestValidator : AbstractValidator<CreatePerfilRequest>
{
    public CreatePerfilRequestValidator()
    {
        RuleFor(x => x.NomePerfil)
            .NotEmpty().WithMessage("O nome do perfil é obrigatório.")
            .MaximumLength(30).WithMessage("O nome do perfil não pode exceder 30 caracteres.");

        RuleFor(x => x.NomePerfil)
            .NotEmpty().WithMessage("O nome do perfil é obrigatório.")
            .MinimumLength(3).WithMessage("O nome do perfil deve ter pelo menos 3 caracteres.");

        RuleFor(x => x.Descricao)
            .MaximumLength(100).WithMessage("A descrição não pode exceder 100 caracteres.");
    }
}