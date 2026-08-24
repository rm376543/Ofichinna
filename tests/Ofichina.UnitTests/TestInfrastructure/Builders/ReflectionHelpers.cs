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
}
