using FluentValidation;
using Ofichina.Contracts.Requests.PerfilCliente;

namespace Ofichina.Application.Validators.PerfilCliente;

public sealed class VincularPerfilClienteRequestValidator : AbstractValidator<VincularPerfilClienteRequest>
{
    public VincularPerfilClienteRequestValidator()
    {
        RuleFor(x => x.UsuarioId)
            .NotEmpty().WithMessage("O Id do cliente é obrigatório.");

        RuleFor(x => x.PerfilId)
            .NotEmpty().WithMessage("O Id do perfil é obrigatório.");
    }
}