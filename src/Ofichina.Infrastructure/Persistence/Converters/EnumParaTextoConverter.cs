using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Ofichina.Domain.Common;

namespace Ofichina.Infrastructure.Persistence.Converters;

public sealed class EnumParaTextoConverter<TEnum> : ValueConverter<TEnum, string> where TEnum : struct, Enum
{
    public EnumParaTextoConverter()
        : base(
            value => value.ToUpperSnakeCase(),
            value => EnumTextExtensions.ParseUpperSnakeCase<TEnum>(value))
    {
    }
}