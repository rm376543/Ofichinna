using FluentValidation;
using Ofichina.Contracts.Requests.PerfilPermissoes;

namespace Ofichina.Application.Validators.PerfilPermissoes;

public sealed class VincularPermissaoPerfilRequestValidator : AbstractValidator<VincularPermissaoPerfilRequest>
{
    public VincularPermissaoPerfilRequestValidator()
    {
        RuleFor(x => x.PerfilId)
            .NotEmpty().WithMessage("O Id do perfil é obrigatório.");

        RuleFor(x => x.PermissaoId)
            .NotEmpty().WithMessage("O Id da permissão é obrigatório.");
    }
}
