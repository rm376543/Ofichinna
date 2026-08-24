using System.Reflection;
using Ofichina.Domain.Entities;

namespace Ofichina.UnitTests.TestInfrastructure.Builders;

internal static class ReflectionHelpers
{
    public static void DefinirId(Entity entity, Guid id)
    {
        var prop = entity.GetType().GetProperty("Id", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (prop is not null && prop.CanWrite)
        {
            prop.SetValue(entity, id);
            return;
        }
        // tentar definir via propriedade da classe base
        var baseProp = typeof(Entity).GetProperty("Id", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        baseProp?.SetValue(entity, id);
    }

    public static void DefinirPropriedade(object target, string propertyName, object? value)
    {
        if (target is null)
            return;

        var type = target.GetType();

        var prop = type.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (prop is not null && prop.CanWrite)
        {
            prop.SetValue(target, value);
            return;
        }

        // tentar definir campo backing (auto-property)
        var backing = type.GetField($"<{propertyName}>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic);
        if (backing is not null)
        {
            backing.SetValue(target, value);
            return;
        }

        // tentar em propriedades da classe base
        var baseProp = type.BaseType?.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (baseProp is not null && baseProp.CanWrite)
        {
            baseProp.SetValue(target, value);
        }
    }
}
