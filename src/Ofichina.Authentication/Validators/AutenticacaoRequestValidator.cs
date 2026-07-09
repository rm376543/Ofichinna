using FluentValidation;
using Ofichina.Contracts.Requests.Autenticacao;

namespace Ofichina.Authentication.Validators;

public sealed class AutenticacaoRequestValidator : AbstractValidator<AutenticacaoRequest>
{
    public AutenticacaoRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("O e-mail é obrigatório.")
            .EmailAddress().WithMessage("O e-mail informado é inválido.")
            .MaximumLength(200).WithMessage("O e-mail não pode exceder 200 caracteres.");

        RuleFor(x => x.Senha)
            .NotEmpty().WithMessage("A senha é obrigatória.")
            .MinimumLength(6).WithMessage("A senha deve conter ao menos 6 caracteres.");
    }
}