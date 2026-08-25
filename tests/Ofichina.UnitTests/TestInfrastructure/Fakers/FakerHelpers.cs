using System.Text;
using Bogus;

namespace Ofichina.UnitTests.TestInfrastructure.Fakers;

internal static class FakerHelpers
{
    private static readonly Faker Faker = new();

    public static string GerarCpfValido()
    {
        // Gera 9 primeiros dígitos aleatórios
        var nums = Faker.Random.Digits(9);

        int soma = 0;
        for (int i = 0; i < 9; i++)
            soma += nums[i] * (10 - i);

        int resto = soma % 11;
        int dig1 = resto < 2 ? 0 : 11 - resto;

        soma = 0;
        for (int i = 0; i < 9; i++)
            soma += nums[i] * (11 - i);
        soma += dig1 * 2;

        resto = soma % 11;
        int dig2 = resto < 2 ? 0 : 11 - resto;

        var sb = new StringBuilder();
        foreach (var n in nums) sb.Append(n);
        sb.Append(dig1);
        sb.Append(dig2);

        return sb.ToString();
    }

    public static string GerarPlaca()
    {
        // Gerar placa no formato antigo AAA9999 (válida) para simplicidade
        var letras = new string(Faker.Random.Chars('A', 'Z', 3));
        var numeros = string.Concat(Faker.Random.Digits(4));
        return letras + numeros;
    }

    public static string GerarTelefoneValido(bool celular = true)
    {
        int ddd = Faker.Random.Int(11, 99);
        var sb = new StringBuilder();
        sb.Append(ddd.ToString("D2"));
        if (celular)
        {
            // 11 dígitos: DDD + 9 + 8 dígitos
            sb.Append('9');
        }
        // 10 dígitos: DDD + 8 dígitos
        foreach (var d in Faker.Random.Digits(8)) sb.Append(d);
        return sb.ToString();
    }

    public static string GerarCep()
    {
        return string.Concat(Faker.Random.Digits(8));
    }

    public static int GerarHodometro()
    {
        return Faker.Random.Int(0, 200_000);
    }

    public static string GerarCodigoPeca()
    {
        return $"PC-{Faker.Random.Guid():N}";
    }
}
