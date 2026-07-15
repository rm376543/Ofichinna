using System.Text.RegularExpressions;
using FluentValidation;
using Ofichina.Contracts.Requests.Pessoa;

namespace Ofichina.Application.Validators.Pessoa;

/// <summary>
/// Validador para criação de pessoa.
/// </summary>
public sealed class CreatePessoaRequestValidator : AbstractValidator<CreatePessoaRequest>
{
    public CreatePessoaRequestValidator()
    {
        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("O nome é obrigatório.")
            .MinimumLength(3).WithMessage("O nome deve ter pelo menos 3 caracteres.")
            .MaximumLength(100).WithMessage("O nome não pode exceder 100 caracteres.");

        RuleFor(x => x.Documento)
            .NotEmpty().WithMessage("O documento é obrigatório.")
            .Must(DocumentoEhValido).WithMessage("O documento deve conter 11 dígitos para CPF ou 14 dígitos para CNPJ.");

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

        RuleFor(x => x.UsuarioId)
            .NotEmpty().WithMessage("O usuário vinculado é obrigatório.");
    }

    private static bool DocumentoEhValido(string documento)
    {
#pragma warning disable S6444
        var numerico = Regex.Replace(documento ?? string.Empty, @"\D", string.Empty);
#pragma warning restore S6444
        return numerico.Length is 11 or 14;
    }

    private static bool TelefoneEhValido(string telefone)
    {
#pragma warning disable S6444
        var numerico = Regex.Replace(telefone ?? string.Empty, @"\D", string.Empty);
#pragma warning restore S6444
        return numerico.Length is 10 or 11;
    }

    private static bool CepEhValido(string cep)
    {
#pragma warning disable S6444
        var numerico = Regex.Replace(cep ?? string.Empty, @"\D", string.Empty);
#pragma warning restore S6444
        return numerico.Length == 8;
    }
}
