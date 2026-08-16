using Ofichina.Domain.Common;

namespace Ofichina.UnitTests.Domain;

public sealed class AuditTests
{
    [Fact]
    public void Audit_Deve_Permitir_Leitura_E_Escrita_Das_Propriedades()
    {
        var audit = new TestAudit
        {
            CreatedAt = new DateTime(2025, 1, 1, 10, 0, 0, DateTimeKind.Utc),
            UpdatedAt = new DateTime(2025, 1, 2, 10, 0, 0, DateTimeKind.Utc),
            DeletedAt = new DateTime(2025, 1, 3, 10, 0, 0, DateTimeKind.Utc)
        };

        Assert.Equal(new DateTime(2025, 1, 1, 10, 0, 0, DateTimeKind.Utc), audit.CreatedAt);
        Assert.Equal(new DateTime(2025, 1, 2, 10, 0, 0, DateTimeKind.Utc), audit.UpdatedAt);
        Assert.Equal(new DateTime(2025, 1, 3, 10, 0, 0, DateTimeKind.Utc), audit.DeletedAt);
    }

    private sealed class TestAudit : Audit
    {
    }
}