using Ofichina.Domain.Common;
using Ofichina.Domain.Enums;

namespace Ofichina.UnitTests.Domain;

public sealed class EnumTextExtensionsTests
{
    [Fact]
    public void ToUpperSnakeCase_Deve_Converter_Nome_Do_Enum_Para_Snake_Case_Maiusculo()
    {
        var valor = StatusOrcamento.AguardandoAprovacao.ToUpperSnakeCase();

        Assert.Equal("AGUARDANDO_APROVACAO", valor);
    }

    [Fact]
    public void ParseUpperSnakeCase_Deve_Converter_Texto_Valido_Para_Enum()
    {
        var valor = EnumTextExtensions.ParseUpperSnakeCase<StatusAgendamento>("finalizado");

        Assert.Equal(StatusAgendamento.FINALIZADO, valor);
    }

    [Fact]
    public void ParseUpperSnakeCase_Deve_Rejeitar_Valores_Invalidos()
    {
        Assert.Throws<ArgumentException>(() => EnumTextExtensions.ParseUpperSnakeCase<StatusAgendamento>(string.Empty));
        Assert.Throws<ArgumentException>(() => EnumTextExtensions.ParseUpperSnakeCase<StatusAgendamento>("valor-invalido"));
    }
}