using System.Reflection;
using Ofichina.Application.UseCases.Servicos.Mappings;
using Ofichina.Domain.Entities;

namespace Ofichina.UnitTests.Application.Servicos;

public sealed class ServicoResponseMappingExtensionsTests
{
    [Fact]
    public void ToResponse_Deve_Mapear_Dados_Do_Servico_E_Auditoria()
    {
        var servico = new Servico("Troca de óleo", "Substituição completa", 149.90m)
        {
            CreatedAt = new DateTime(2026, 8, 10, 0, 0, 0, DateTimeKind.Utc),
            UpdatedAt = new DateTime(2026, 8, 11, 0, 0, 0, DateTimeKind.Utc),
            DeletedAt = new DateTime(2026, 8, 12, 0, 0, 0, DateTimeKind.Utc)
        };

        var response = servico.ToResponse();

        Assert.Equal(servico.Id, response.ServicoId);
        Assert.Equal("Troca de óleo", response.Nome);
        Assert.Equal("Substituição completa", response.Descricao);
        Assert.Equal(149.90m, response.Valor);
        Assert.False(response.Ativo);
        Assert.Equal("10/08/2026", response.CreatedAt);
        Assert.Equal("11/08/2026", response.UpdatedAt);
        Assert.Equal("12/08/2026", response.DeletedAt);
    }

    [Fact]
    public void ToResponse_Deve_Rejeitar_Servico_Nulo()
    {
        Servico? servico = null;

        Assert.Throws<ArgumentNullException>(() => servico!.ToResponse());
    }
}