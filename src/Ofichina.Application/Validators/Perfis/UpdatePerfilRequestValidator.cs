using FluentValidation;
using Ofichina.Contracts.Requests.Perfis;

namespace Ofichina.Application.Validators.Perfis;

/// <summary>
/// Validador para atualização de perfil.
/// </summary>
public sealed class UpdatePerfilRequestValidator : AbstractValidator<UpdatePerfilRequest>
{
    public UpdatePerfilRequestValidator()
    {
        RuleFor(x => x.PerfilId)
            .NotEmpty().WithMessage("O Id do perfil é obrigatório.");

        RuleFor(x => x.NomePerfil)
            .NotEmpty().WithMessage("O nome do perfil é obrigatório.")
            .MaximumLength(30).WithMessage("O nome do perfil não pode exceder 30 caracteres.");

        RuleFor(x => x.NomePerfil)
            .NotEmpty().WithMessage("O nome do perfil é obrigatório.")
            .MinimumLength(3).WithMessage("O nome do perfil deve ter pelo menos 3 caracteres.");

        RuleFor(x => x.Descricao)
            .MaximumLength(300).WithMessage("A descrição não pode exceder 300 caracteres.");
    }
}
