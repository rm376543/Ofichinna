using FluentValidation;
using Ofichina.Contracts.Requests.PerfilUsuario;

namespace Ofichina.Application.Validators.PerfilUsuario;

public sealed class VincularPerfilUsuarioRequestValidator : AbstractValidator<VincularPerfilUsuarioRequest>
{
    public VincularPerfilUsuarioRequestValidator()
    {
        RuleFor(x => x.UsuarioId)
            .NotEmpty().WithMessage("O Id do usuário é obrigatório.");

        RuleFor(x => x.PerfilId)
            .NotEmpty().WithMessage("O Id do perfil é obrigatório.");
    }
}