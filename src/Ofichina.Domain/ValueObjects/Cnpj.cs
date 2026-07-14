using System.Text.RegularExpressions;
using Ofichina.Domain.Exceptions;

namespace Ofichina.Domain.ValueObjects;

/// <summary>
/// Representa um CNPJ válido do domínio.
/// </summary>
public sealed class Cnpj : Documento
{
    public override TipoDocumento Tipo => TipoDocumento.CNPJ;

    private Cnpj()
    {
    }

    public Cnpj(string numero)
        : base(Normalizar(numero))
    {
        if (!EhValido(Numero))
            throw new DomainException("CNPJ inválido.");
    }

    private static string Normalizar(string cnpj)
    {
        if (string.IsNullOrWhiteSpace(cnpj))
            throw new DomainException("CNPJ inválido.");

        #pragma warning disable S6444
        return Regex.Replace(cnpj, @"\D", "");
        #pragma warning restore S6444
    }

    private static bool EhValido(string cnpj)
    {
        if (cnpj.Length != 14)
            return false;

        if (cnpj.Distinct().Count() == 1)
            return false;

        int[] multiplicador1 = { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
        int[] multiplicador2 = { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };

        var soma = 0;

        for (int i = 0; i < 12; i++)
            soma += (cnpj[i] - '0') * multiplicador1[i];

        var resto = soma % 11;
        var primeiroDigito = resto < 2 ? 0 : 11 - resto;

        if (cnpj[12] - '0' != primeiroDigito)
            return false;

        soma = 0;

        for (int i = 0; i < 13; i++)
            soma += (cnpj[i] - '0') * multiplicador2[i];

        resto = soma % 11;
        var segundoDigito = resto < 2 ? 0 : 11 - resto;

        return cnpj[13] - '0' == segundoDigito;
    }

    public override string ToString() => Numero;
}