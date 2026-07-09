using FluentValidation;
using Ofichina.Contracts.Requests.Cliente;
using Ofichina.Contracts.Requests.Usuario;

namespace Ofichina.Authentication.Validators;

public sealed class CadastrarUsuarioRequestValidator : AbstractValidator<CreateClienteRequest>
{
    public CadastrarUsuarioRequestValidator()
    {
        RuleFor(x => x.Nome)
            .NotEmpty()
            .MaximumLength(150);

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(200);

        RuleFor(x => x.Senha)
            .NotEmpty()
            .MinimumLength(6);
    }
}