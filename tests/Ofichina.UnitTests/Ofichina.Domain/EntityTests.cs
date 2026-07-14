using Ofichina.Domain.Entities;

namespace Ofichina.UnitTests.Domain;

public class EntityTests
{
    [Fact]
    public void Entity_DeveInicializar_Id_e_CreatedAt_Automaticamente()
    {
        var id = Guid.NewGuid();
        var entity = new TestEntity(id);

        Assert.NotEqual(Guid.Empty, entity.Id);
        Assert.True(entity.CreatedAt <= DateTime.UtcNow);
        Assert.Null(entity.UpdatedAt);
        Assert.Null(entity.DeletedAt);
    }

    [Fact]
    public void Entity_DeveConsiderar_Iguais_Quando_Tiverem_O_Mesmo_Id()
    {
        var id = Guid.NewGuid();

        var entity1 = new TestEntity(id);
        var entity2 = new TestEntity(id);

        Assert.True(entity1.Equals(entity2));
        Assert.Equal(entity1.GetHashCode(), entity2.GetHashCode());
    }

    [Fact]
    public void Entity_DeveConsiderar_Diferentes_Quando_Tiverem_Ids_Diferentes()
    {
        var id1 = Guid.NewGuid();
        var id2 = Guid.NewGuid();

        var entity1 = new TestEntity(id1);
        var entity2 = new TestEntity(id2);

        Assert.False(entity1.Equals(entity2));
    }

    private sealed class TestEntity : Entity
    {
        public TestEntity(Guid id) : base(id)
        {
        }
    }
}