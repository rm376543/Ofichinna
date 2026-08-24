using System.Text;

namespace Ofichina.UnitTests.TestInfrastructure.Fakers;

internal static class FakerHelpers
{
    public static string GerarCpfValido()
    {
        // Gera 9 primeiros dígitos aleatórios
        var rnd = new Random();
        var nums = new int[9];
        for (int i = 0; i < 9; i++)
            nums[i] = rnd.Next(0, 10);

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
        var rnd = new Random();
        // Gerar placa no formato antigo AAA9999 (válida) para simplicidade
        string letras()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            var sb = new StringBuilder(3);
            for (int i = 0; i < 3; i++) sb.Append(chars[rnd.Next(chars.Length)]);
            return sb.ToString();
        }

        string numeros()
        {
            var sb = new StringBuilder(4);
            for (int i = 0; i < 4; i++) sb.Append(rnd.Next(0, 10));
            return sb.ToString();
        }

        return letras() + numeros();
    }

    public static string GerarTelefoneValido(bool celular = true)
    {
        var rnd = new Random();
        int ddd = rnd.Next(11, 100);
        if (celular)
        {
            // 11 dígitos: DDD + 9 + 8 dígitos
            var sb = new StringBuilder();
            sb.Append(ddd.ToString("D2"));
            sb.Append('9');
            for (int i = 0; i < 8; i++) sb.Append(rnd.Next(0, 10));
            return sb.ToString();
        }
        else
        {
            // 10 dígitos: DDD + 8 dígitos
            var sb = new StringBuilder();
            sb.Append(ddd.ToString("D2"));
            for (int i = 0; i < 8; i++) sb.Append(rnd.Next(0, 10));
            return sb.ToString();
        }
    }

    public static string GerarCep()
    {
        var rnd = new Random();
        var sb = new StringBuilder(8);
        for (int i = 0; i < 8; i++) sb.Append(rnd.Next(0, 10));
        return sb.ToString();
    }

    public static int GerarHodometro()
    {
        var rnd = new Random();
        return rnd.Next(0, 200_000);
    }

    public static string GerarCodigoPeca()
    {
        return $"PC-{Guid.NewGuid():N}";
    }
}
