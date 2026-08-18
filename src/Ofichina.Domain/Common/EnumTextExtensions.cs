using System.Text.RegularExpressions;

namespace Ofichina.Domain.Common;

public static class EnumTextExtensions
{
    public static string ToUpperSnakeCase<TEnum>(this TEnum value)
    where TEnum : struct, Enum
    {
        var name = value.ToString();

        return Regex.Replace(
            name,
            "([a-z0-9])([A-Z])",
            "$1_$2",
            RegexOptions.None,
            TimeSpan.FromMilliseconds(100))
            .ToUpperInvariant();
    }

    public static TEnum ParseUpperSnakeCase<TEnum>(string value) where TEnum : struct, Enum
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);

        var normalized = value.Trim().ToUpperInvariant();

        foreach (var enumValue in Enum.GetValues<TEnum>())
        {
            if (enumValue.ToUpperSnakeCase() == normalized)
                return enumValue;
        }

        throw new ArgumentException($"Valor '{value}' inválido para {typeof(TEnum).Name}.", nameof(value));
    }
}