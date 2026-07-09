using System.Text.RegularExpressions;

namespace Ofichina.Domain.ValueObjects;

/// <summary>
/// Representa um CPF válido do domínio.
/// </summary>
public sealed class Cpf : ValueObject
{
    public string Value { get; }

    private Cpf(string value)
    {
        Value = value;
    }

    public static Cpf Criar(string cpf)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(cpf);

        #pragma warning disable S6444
        cpf = Regex.Replace(cpf, @"\D", "");
        #pragma warning restore S6444

        if (!EhValido(cpf))
            throw new ArgumentException("CPF inválido.", nameof(cpf));

        return new Cpf(cpf);
    }

    private static bool EhValido(string cpf)
    {
        if (cpf.Length != 11)
            return false;

        // Rejeita CPFs com todos os dígitos iguais
        if (cpf.Distinct().Count() == 1)
            return false;

        // Primeiro dígito
        var soma = 0;

        for (int i = 0; i < 9; i++)
            soma += (cpf[i] - '0') * (10 - i);

        var resto = soma % 11;
        var primeiroDigito = resto < 2 ? 0 : 11 - resto;

        if (cpf[9] - '0' != primeiroDigito)
            return false;

        // Segundo dígito
        soma = 0;

        for (int i = 0; i < 10; i++)
            soma += (cpf[i] - '0') * (11 - i);

        resto = soma % 11;
        var segundoDigito = resto < 2 ? 0 : 11 - resto;

        return cpf[10] - '0' == segundoDigito;
    }

    public override string ToString() => Value;

    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }
}