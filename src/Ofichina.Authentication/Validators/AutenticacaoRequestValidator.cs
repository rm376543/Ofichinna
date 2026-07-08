using FluentValidation;
using Ofichina.Contracts.Requests;

namespace Ofichina.Authentication.Validators;

public sealed class AutenticacaoRequestValidator : AbstractValidator<AutenticacaoRequest>
{
    public AutenticacaoRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Senha)
            .NotEmpty()
            .MinimumLength(6);
    }
}