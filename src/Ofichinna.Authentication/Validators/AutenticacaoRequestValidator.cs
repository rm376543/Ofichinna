using FluentValidation;
using Ofichina.Contracts.Requests;

namespace Ofichinna.Authentication.Validators;

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