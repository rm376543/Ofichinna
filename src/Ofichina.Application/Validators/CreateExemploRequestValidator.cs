using FluentValidation;
using Ofichina.Contracts.Requests;

namespace Ofichina.Application.Validators;

/// <summary>
/// Exemplo de validador usando FluentValidation.
/// Este validador deve ser criado para cada requisição específica.
/// </summary>
public class CreateExemploRequestValidator : AbstractValidator<CreateRequest>
{
    public CreateExemploRequestValidator()
    {
        // Adicione suas regras de validação aqui
        // Exemplo:
        // RuleFor(x => x.Nome)
        //     .NotEmpty().WithMessage("Nome é obrigatório")
        //     .MaximumLength(100).WithMessage("Nome não pode exceder 100 caracteres");
    }
}
