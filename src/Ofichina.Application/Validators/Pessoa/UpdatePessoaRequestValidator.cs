using FluentValidation;
using Ofichina.Contracts.Requests.Pessoa;
using System.Text.RegularExpressions;

namespace Ofichina.Application.Validators.Pessoa;

/// <summary>
/// Validador para atualização de pessoa.
/// </summary>
public sealed class UpdatePessoaRequestValidator : AbstractValidator<UpdatePessoaRequest>
{
    public UpdatePessoaRequestValidator()
    {
        RuleFor(x => x.PessoaId)
            .NotEmpty().WithMessage("O Id da pessoa é obrigatório.");

        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("O nome é obrigatório.")
            .MinimumLength(3).WithMessage("O nome deve ter pelo menos 3 caracteres.")
            .MaximumLength(100).WithMessage("O nome não pode exceder 100 caracteres.");

        RuleFor(x => x.Telefone)
            .NotEmpty().WithMessage("O telefone é obrigatório.")
            .Must(TelefoneEhValido).WithMessage("O telefone deve conter 10 ou 11 dígitos.");

        RuleFor(x => x.Logradouro)
            .NotEmpty().WithMessage("O logradouro é obrigatório.")
            .MaximumLength(200).WithMessage("O logradouro não pode exceder 200 caracteres.");

        RuleFor(x => x.Numero)
            .NotEmpty().WithMessage("O número é obrigatório.")
            .MaximumLength(20).WithMessage("O número não pode exceder 20 caracteres.");

        RuleFor(x => x.Complemento)
            .MaximumLength(100).WithMessage("O complemento não pode exceder 100 caracteres.");

        RuleFor(x => x.Bairro)
            .NotEmpty().WithMessage("O bairro é obrigatório.")
            .MaximumLength(100).WithMessage("O bairro não pode exceder 100 caracteres.");

        RuleFor(x => x.Cidade)
            .NotEmpty().WithMessage("A cidade é obrigatória.")
            .MaximumLength(100).WithMessage("A cidade não pode exceder 100 caracteres.");

        RuleFor(x => x.Estado)
            .NotEmpty().WithMessage("O estado é obrigatório.")
            .Length(2).WithMessage("O estado deve conter 2 caracteres.");

        RuleFor(x => x.Cep)
            .NotEmpty().WithMessage("O CEP é obrigatório.")
            .Must(CepEhValido).WithMessage("O CEP deve conter 8 dígitos.");
    }

    private static bool TelefoneEhValido(string telefone)
    {
        var numerico = Regex.Replace(
            telefone ?? string.Empty,
            @"\D",
            string.Empty,
            RegexOptions.None,
            TimeSpan.FromMilliseconds(100));

        return numerico.Length is 10 or 11;
    }

    private static bool CepEhValido(string cep)
    {
        var numerico = Regex.Replace(
            cep ?? string.Empty,
            @"\D",
            string.Empty,
            RegexOptions.None,
            TimeSpan.FromMilliseconds(100));

        return numerico.Length == 8;
    }
}
