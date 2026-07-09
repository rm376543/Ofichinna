using FluentValidation;
using Ofichina.Application.UseCases.Cliente.Commands;

namespace Ofichina.Application.Validators.Cliente;

public sealed class VincularPerfilClienteCommandValidator : AbstractValidator<VincularPerfilClienteCommand>
{
    public VincularPerfilClienteCommandValidator()
    {
        RuleFor(x => x.UsuarioId)
            .NotEmpty().WithMessage("O Id do cliente é obrigatório.");

        RuleFor(x => x.PerfilId)
            .NotEmpty().WithMessage("O Id do perfil é obrigatório.");
    }
}