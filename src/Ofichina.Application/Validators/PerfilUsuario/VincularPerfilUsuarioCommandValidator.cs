using FluentValidation;
using Ofichina.Application.UseCases.PerfilUsuario.Commands;

namespace Ofichina.Application.Validators.PerfilUsuario;

public sealed class VincularPerfilUsuarioCommandValidator : AbstractValidator<VincularPerfilUsuarioCommand>
{
    public VincularPerfilUsuarioCommandValidator()
    {
        RuleFor(x => x.UsuarioId)
            .NotEmpty().WithMessage("O Id do usuário é obrigatório.");

        RuleFor(x => x.PerfilId)
            .NotEmpty().WithMessage("O Id do perfil é obrigatório.");
    }
}