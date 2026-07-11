using Ofichina.Domain.Entities;

namespace Ofichina.UnitTests.Domain;

public class EntityTests
{
    [Fact]
    public void Entity_DeveInicializar_Id_e_CreatedAt_Automaticamente()
    {
        var entity = new TestEntity();

        Assert.NotEqual(Guid.Empty, entity.Id);
        Assert.True(entity.CreatedAt <= DateTime.UtcNow);
        Assert.Null(entity.UpdatedAt);
        Assert.Null(entity.DeletedAt);
    }

    [Fact]
    public void Entity_DeveConsiderar_Iguais_Quando_Tiverem_O_Mesmo_Id()
    {
        var id = Guid.Parse("550e8400-e29b-41d4-a716-446655440000");

        var entity1 = new TestEntity { Id = id };
        var entity2 = new TestEntity { Id = id };

        Assert.True(entity1.Equals(entity2));
        Assert.Equal(entity1.GetHashCode(), entity2.GetHashCode());
    }

    [Fact]
    public void Entity_DeveConsiderar_Diferentes_Quando_Tiverem_Ids_Diferentes()
    {
        var entity1 = new TestEntity { Id = Guid.NewGuid() };
        var entity2 = new TestEntity { Id = Guid.NewGuid() };

        Assert.False(entity1.Equals(entity2));
    }

    private sealed class TestEntity : Entity
    {
    }
}