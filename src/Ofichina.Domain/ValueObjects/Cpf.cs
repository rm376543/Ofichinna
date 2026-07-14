using System.Text.RegularExpressions;
using Ofichina.Domain.Exceptions;

namespace Ofichina.Domain.ValueObjects;

/// <summary>
/// Representa um CPF válido do domínio.
/// </summary>
public sealed class Cpf : Documento
{
    public override TipoDocumento Tipo => TipoDocumento.CPF;

    private Cpf()
    {
    }

    public Cpf(string numero) : base(Normalizar(numero))
    {
        if (!EhValido(Numero))
            throw new DomainException("CPF inválido.");
    }

    private static string Normalizar(string cpf)
    {
        if (string.IsNullOrWhiteSpace(cpf))
            throw new DomainException("CPF inválido.");

        #pragma warning disable S6444
        return Regex.Replace(cpf, @"\D", "");
        #pragma warning restore S6444
    }

    private static bool EhValido(string cpf)
    {
        if (cpf.Length != 11)
            return false;

        if (cpf.Distinct().Count() == 1)
            return false;

        var soma = 0;

        for (int i = 0; i < 9; i++)
            soma += (cpf[i] - '0') * (10 - i);

        var resto = soma % 11;
        var primeiroDigito = resto < 2 ? 0 : 11 - resto;

        if (cpf[9] - '0' != primeiroDigito)
            return false;

        soma = 0;

        for (int i = 0; i < 10; i++)
            soma += (cpf[i] - '0') * (11 - i);

        resto = soma % 11;
        var segundoDigito = resto < 2 ? 0 : 11 - resto;

        return cpf[10] - '0' == segundoDigito;
    }

    public override string ToString() => Numero;
}