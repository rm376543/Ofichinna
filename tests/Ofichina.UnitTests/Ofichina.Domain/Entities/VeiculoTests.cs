using Ofichina.Domain.Entities;
using Ofichina.Domain.Exceptions;
using Ofichina.Domain.ValueObjects;

namespace Ofichina.UnitTests.Ofichina.Domain.Entities;

public class VeiculoTests
{
    [Fact]
    public void Veiculo_DeveInicializar_ComDadosValidos()
    {
        var pessoaId = Guid.NewGuid();
        var placa = CriarPlaca();
        var hodometro = CriarHodometro();

        var veiculo = new Veiculo(
            pessoaId,
            placa,
            "  Toyota  ",
            "  Corolla  ",
            2020,
            "  Prata  ",
            hodometro);

        Assert.Equal(pessoaId, veiculo.PessoaId);
        Assert.Same(placa, veiculo.Placa);
        Assert.Equal("Toyota", veiculo.Marca);
        Assert.Equal("Corolla", veiculo.Modelo);
        Assert.Equal(2020, veiculo.AnoFabricacao);
        Assert.Equal("Prata", veiculo.Cor);
        Assert.Same(hodometro, veiculo.Hodometro);

        Assert.NotEqual(Guid.Empty, veiculo.Id);
        Assert.True(veiculo.CreatedAt <= DateTime.UtcNow);
        Assert.Null(veiculo.UpdatedAt);
        Assert.Null(veiculo.DeletedAt);
    }

    [Fact]
    public void Veiculo_DeveLancarExcecao_QuandoPessoaIdForVazio()
    {
        var exception = Assert.Throws<DomainException>(() =>
            new Veiculo(
                Guid.Empty,
                CriarPlaca(),
                "Toyota",
                "Corolla",
                2020,
                "Prata",
                CriarHodometro()));

        Assert.Equal(
            "A pessoa deve ser informada.",
            exception.Message);
    }

    [Fact]
    public void Veiculo_DeveLancarExcecao_QuandoPlacaForNula()
    {
        var exception = Assert.Throws<DomainException>(() =>
            new Veiculo(
                Guid.NewGuid(),
                null!,
                "Toyota",
                "Corolla",
                2020,
                "Prata",
                CriarHodometro()));

        Assert.Equal(
            "A placa deve ser informada.",
            exception.Message);
    }

    [Fact]
    public void Veiculo_DeveLancarExcecao_QuandoMarcaForNula()
    {
        var exception = Assert.Throws<DomainException>(() =>
            new Veiculo(
                Guid.NewGuid(),
                CriarPlaca(),
                null!,
                "Corolla",
                2020,
                "Prata",
                CriarHodometro()));

        Assert.Equal(
            "A marca deve ser informada.",
            exception.Message);
    }

    [Fact]
    public void Veiculo_DeveLancarExcecao_QuandoMarcaForVazia()
    {
        var exception = Assert.Throws<DomainException>(() =>
            new Veiculo(
                Guid.NewGuid(),
                CriarPlaca(),
                string.Empty,
                "Corolla",
                2020,
                "Prata",
                CriarHodometro()));

        Assert.Equal(
            "A marca deve ser informada.",
            exception.Message);
    }

    [Fact]
    public void Veiculo_DeveLancarExcecao_QuandoMarcaContiverApenasEspacos()
    {
        var exception = Assert.Throws<DomainException>(() =>
            new Veiculo(
                Guid.NewGuid(),
                CriarPlaca(),
                "   ",
                "Corolla",
                2020,
                "Prata",
                CriarHodometro()));

        Assert.Equal(
            "A marca deve ser informada.",
            exception.Message);
    }

    [Fact]
    public void Veiculo_DeveLancarExcecao_QuandoModeloForNulo()
    {
        var exception = Assert.Throws<DomainException>(() =>
            new Veiculo(
                Guid.NewGuid(),
                CriarPlaca(),
                "Toyota",
                null!,
                2020,
                "Prata",
                CriarHodometro()));

        Assert.Equal(
            "O modelo deve ser informado.",
            exception.Message);
    }

    [Fact]
    public void Veiculo_DeveLancarExcecao_QuandoModeloForVazio()
    {
        var exception = Assert.Throws<DomainException>(() =>
            new Veiculo(
                Guid.NewGuid(),
                CriarPlaca(),
                "Toyota",
                string.Empty,
                2020,
                "Prata",
                CriarHodometro()));

        Assert.Equal(
            "O modelo deve ser informado.",
            exception.Message);
    }

    [Fact]
    public void Veiculo_DeveLancarExcecao_QuandoModeloContiverApenasEspacos()
    {
        var exception = Assert.Throws<DomainException>(() =>
            new Veiculo(
                Guid.NewGuid(),
                CriarPlaca(),
                "Toyota",
                "   ",
                2020,
                "Prata",
                CriarHodometro()));

        Assert.Equal(
            "O modelo deve ser informado.",
            exception.Message);
    }

    [Fact]
    public void Veiculo_DeveLancarExcecao_QuandoAnoForMenorQue1900()
    {
        var exception = Assert.Throws<DomainException>(() =>
            new Veiculo(
                Guid.NewGuid(),
                CriarPlaca(),
                "Toyota",
                "Corolla",
                1899,
                "Prata",
                CriarHodometro()));

        Assert.Equal(
            "Ano do veículo inválido.",
            exception.Message);
    }

    [Fact]
    public void Veiculo_DeveLancarExcecao_QuandoAnoForMaiorQueAnoAtualMaisUm()
    {
        var anoInvalido = DateTime.Now.Year + 2;

        var exception = Assert.Throws<DomainException>(() =>
            new Veiculo(
                Guid.NewGuid(),
                CriarPlaca(),
                "Toyota",
                "Corolla",
                anoInvalido,
                "Prata",
                CriarHodometro()));

        Assert.Equal(
            "Ano do veículo inválido.",
            exception.Message);
    }

    [Fact]
    public void Veiculo_DeveAceitar_Ano1900()
    {
        var veiculo = CriarVeiculo(anoFabricacao: 1900);

        Assert.Equal(1900, veiculo.AnoFabricacao);
    }

    [Fact]
    public void Veiculo_DeveAceitar_AnoAtualMaisUm()
    {
        var anoValido = DateTime.Now.Year + 1;

        var veiculo = CriarVeiculo(anoFabricacao: anoValido);

        Assert.Equal(anoValido, veiculo.AnoFabricacao);
    }

    [Fact]
    public void Veiculo_DeveLancarExcecao_QuandoHodometroForNulo()
    {
        var exception = Assert.Throws<DomainException>(() =>
            new Veiculo(
                Guid.NewGuid(),
                CriarPlaca(),
                "Toyota",
                "Corolla",
                2020,
                "Prata",
                null!));

        Assert.Equal(
            "A quilometragem deve ser informada.",
            exception.Message);
    }

    [Fact]
    public void Veiculo_DeveUsarCorVazia_QuandoCorForNula()
    {
        var veiculo = CriarVeiculo(cor: null);

        Assert.Equal(string.Empty, veiculo.Cor);
    }

    [Fact]
    public void Veiculo_DeveUsarCorVazia_QuandoCorForVazia()
    {
        var veiculo = CriarVeiculo(cor: string.Empty);

        Assert.Equal(string.Empty, veiculo.Cor);
    }

    [Fact]
    public void Veiculo_DeveUsarCorVazia_QuandoCorContiverApenasEspacos()
    {
        var veiculo = CriarVeiculo(cor: "   ");

        Assert.Equal(string.Empty, veiculo.Cor);
    }

    [Fact]
    public void Veiculo_DeveRemoverEspacosDaMarcaModeloECor()
    {
        var veiculo = CriarVeiculo(
            marca: "  Toyota  ",
            modelo: "  Corolla  ",
            cor: "  Prata  ");

        Assert.Equal("Toyota", veiculo.Marca);
        Assert.Equal("Corolla", veiculo.Modelo);
        Assert.Equal("Prata", veiculo.Cor);
    }

    [Fact]
    public void Veiculo_AlterarPessoa_DeveAlterarPessoaId()
    {
        var novaPessoaId = Guid.NewGuid();
        var veiculo = CriarVeiculo();

        veiculo.AlterarPessoa(novaPessoaId);

        Assert.Equal(novaPessoaId, veiculo.PessoaId);
        Assert.NotNull(veiculo.UpdatedAt);
    }

    [Fact]
    public void Veiculo_AlterarPessoa_DeveLancarExcecao_QuandoIdForVazio()
    {
        var pessoaIdOriginal = Guid.NewGuid();
        var veiculo = CriarVeiculo(pessoaId: pessoaIdOriginal);

        var exception = Assert.Throws<DomainException>(() =>
            veiculo.AlterarPessoa(Guid.Empty));

        Assert.Equal(
            "A pessoa deve ser informada.",
            exception.Message);

        Assert.Equal(pessoaIdOriginal, veiculo.PessoaId);
        Assert.Null(veiculo.UpdatedAt);
    }

    [Fact]
    public void Veiculo_AlterarPlaca_DeveAlterarPlaca()
    {
        var novaPlaca = CriarPlaca();
        var veiculo = CriarVeiculo();

        veiculo.AlterarPlaca(novaPlaca);

        Assert.Same(novaPlaca, veiculo.Placa);
        Assert.NotNull(veiculo.UpdatedAt);
    }

    [Fact]
    public void Veiculo_AlterarPlaca_DeveLancarExcecao_QuandoPlacaForNula()
    {
        var placaOriginal = CriarPlaca();
        var veiculo = CriarVeiculo(placa: placaOriginal);

        var exception = Assert.Throws<DomainException>(() =>
            veiculo.AlterarPlaca(null!));

        Assert.Equal(
            "A placa deve ser informada.",
            exception.Message);

        Assert.Same(placaOriginal, veiculo.Placa);
        Assert.Null(veiculo.UpdatedAt);
    }

    [Fact]
    public void Veiculo_AlterarModelo_DeveAlterarModelo()
    {
        var veiculo = CriarVeiculo();

        veiculo.AlterarModelo("  Civic  ");

        Assert.Equal("Civic", veiculo.Modelo);
        Assert.NotNull(veiculo.UpdatedAt);
    }

    [Fact]
    public void Veiculo_AlterarModelo_DeveLancarExcecao_QuandoModeloForNulo()
    {
        var modeloOriginal = "Corolla";
        var veiculo = CriarVeiculo(modelo: modeloOriginal);

        var exception = Assert.Throws<DomainException>(() =>
            veiculo.AlterarModelo(null!));

        Assert.Equal(
            "O modelo deve ser informado.",
            exception.Message);

        Assert.Equal(modeloOriginal, veiculo.Modelo);
        Assert.Null(veiculo.UpdatedAt);
    }

    [Fact]
    public void Veiculo_AlterarModelo_DeveLancarExcecao_QuandoModeloForVazio()
    {
        var veiculo = CriarVeiculo();

        var exception = Assert.Throws<DomainException>(() =>
            veiculo.AlterarModelo(string.Empty));

        Assert.Equal(
            "O modelo deve ser informado.",
            exception.Message);
    }

    [Fact]
    public void Veiculo_AlterarModelo_DeveLancarExcecao_QuandoModeloForApenasEspacos()
    {
        var veiculo = CriarVeiculo();

        var exception = Assert.Throws<DomainException>(() =>
            veiculo.AlterarModelo("   "));

        Assert.Equal(
            "O modelo deve ser informado.",
            exception.Message);
    }

    [Fact]
    public void Veiculo_AlterarMarca_DeveAlterarMarca()
    {
        var veiculo = CriarVeiculo();

        veiculo.AlterarMarca("  Honda  ");

        Assert.Equal("Honda", veiculo.Marca);
        Assert.NotNull(veiculo.UpdatedAt);
    }

    [Fact]
    public void Veiculo_AlterarMarca_DeveLancarExcecao_QuandoMarcaForNula()
    {
        var marcaOriginal = "Toyota";
        var veiculo = CriarVeiculo(marca: marcaOriginal);

        var exception = Assert.Throws<DomainException>(() =>
            veiculo.AlterarMarca(null!));

        Assert.Equal(
            "A marca deve ser informada.",
            exception.Message);

        Assert.Equal(marcaOriginal, veiculo.Marca);
        Assert.Null(veiculo.UpdatedAt);
    }

    [Fact]
    public void Veiculo_AlterarMarca_DeveLancarExcecao_QuandoMarcaForVazia()
    {
        var veiculo = CriarVeiculo();

        var exception = Assert.Throws<DomainException>(() =>
            veiculo.AlterarMarca(string.Empty));

        Assert.Equal(
            "A marca deve ser informada.",
            exception.Message);
    }

    [Fact]
    public void Veiculo_AlterarMarca_DeveLancarExcecao_QuandoMarcaForApenasEspacos()
    {
        var veiculo = CriarVeiculo();

        var exception = Assert.Throws<DomainException>(() =>
            veiculo.AlterarMarca("   "));

        Assert.Equal(
            "A marca deve ser informada.",
            exception.Message);
    }

    [Fact]
    public void Veiculo_AlterarAnoFabricacao_DeveAlterarAno()
    {
        var veiculo = CriarVeiculo();

        veiculo.AlterarAnoFabricacao(2025);

        Assert.Equal(2025, veiculo.AnoFabricacao);
        Assert.NotNull(veiculo.UpdatedAt);
    }

    [Fact]
    public void Veiculo_AlterarAnoFabricacao_DeveLancarExcecao_QuandoAnoForMenorQue1900()
    {
        var anoOriginal = 2020;
        var veiculo = CriarVeiculo(anoFabricacao: anoOriginal);

        var exception = Assert.Throws<DomainException>(() =>
            veiculo.AlterarAnoFabricacao(1899));

        Assert.Equal(
            "Ano do veículo inválido.",
            exception.Message);

        Assert.Equal(anoOriginal, veiculo.AnoFabricacao);
        Assert.Null(veiculo.UpdatedAt);
    }

    [Fact]
    public void Veiculo_AlterarAnoFabricacao_DeveLancarExcecao_QuandoAnoForMaiorQueAnoAtualMaisUm()
    {
        var anoOriginal = 2020;
        var veiculo = CriarVeiculo(anoFabricacao: anoOriginal);

        var exception = Assert.Throws<DomainException>(() =>
            veiculo.AlterarAnoFabricacao(DateTime.Now.Year + 2));

        Assert.Equal(
            "Ano do veículo inválido.",
            exception.Message);

        Assert.Equal(anoOriginal, veiculo.AnoFabricacao);
        Assert.Null(veiculo.UpdatedAt);
    }

    [Fact]
    public void Veiculo_AlterarCor_DeveAlterarCor()
    {
        var veiculo = CriarVeiculo(cor: "Prata");

        veiculo.AlterarCor("  Preto  ");

        Assert.Equal("Preto", veiculo.Cor);
        Assert.NotNull(veiculo.UpdatedAt);
    }

    [Fact]
    public void Veiculo_AlterarCor_DeveUsarStringVazia_QuandoCorForNula()
    {
        var veiculo = CriarVeiculo(cor: "Prata");

        veiculo.AlterarCor(null);

        Assert.Equal(string.Empty, veiculo.Cor);
        Assert.NotNull(veiculo.UpdatedAt);
    }

    [Fact]
    public void Veiculo_AlterarCor_DeveUsarStringVazia_QuandoCorForApenasEspacos()
    {
        var veiculo = CriarVeiculo(cor: "Prata");

        veiculo.AlterarCor("   ");

        Assert.Equal(string.Empty, veiculo.Cor);
        Assert.NotNull(veiculo.UpdatedAt);
    }

    [Fact]
    public void Veiculo_AlterarHodometro_DeveAlterarHodometro()
    {
        var novoHodometro = CriarHodometro();
        var veiculo = CriarVeiculo();

        veiculo.AlterarHodometro(novoHodometro);

        Assert.Same(novoHodometro, veiculo.Hodometro);
        Assert.NotNull(veiculo.UpdatedAt);
    }

    [Fact]
    public void Veiculo_AlterarHodometro_DeveLancarExcecao_QuandoHodometroForNulo()
    {
        var hodometroOriginal = CriarHodometro();
        var veiculo = CriarVeiculo(hodometro: hodometroOriginal);

        var exception = Assert.Throws<DomainException>(() =>
            veiculo.AlterarHodometro(null!));

        Assert.Equal(
            "A quilometragem deve ser informada.",
            exception.Message);

        Assert.Same(hodometroOriginal, veiculo.Hodometro);
        Assert.Null(veiculo.UpdatedAt);
    }

    [Fact]
    public void Veiculo_Ativar_DeveReativarVeiculoDesativado()
    {
        var veiculo = CriarVeiculo();

        veiculo.Desativar();

        Assert.True(veiculo.EstaExcluida());

        veiculo.Ativar();

        Assert.False(veiculo.EstaExcluida());
        Assert.Null(veiculo.DeletedAt);
        Assert.NotNull(veiculo.UpdatedAt);
    }

    [Fact]
    public void Veiculo_Ativar_DeveSerIgnorado_QuandoVeiculoNaoEstiverDesativado()
    {
        var veiculo = CriarVeiculo();

        veiculo.Ativar();

        Assert.False(veiculo.EstaExcluida());
        Assert.Null(veiculo.DeletedAt);
        Assert.Null(veiculo.UpdatedAt);
    }

    [Fact]
    public void Veiculo_Desativar_DeveExcluirVeiculoLogicamente()
    {
        var veiculo = CriarVeiculo();

        veiculo.Desativar();

        Assert.True(veiculo.EstaExcluida());
        Assert.NotNull(veiculo.DeletedAt);
        Assert.NotNull(veiculo.UpdatedAt);
    }

    [Fact]
    public void Veiculo_Desativar_DeveSerIgnorado_QuandoJaEstiverDesativado()
    {
        var veiculo = CriarVeiculo();

        veiculo.Desativar();

        var deletedAt = veiculo.DeletedAt;
        var updatedAt = veiculo.UpdatedAt;

        veiculo.Desativar();

        Assert.True(veiculo.EstaExcluida());
        Assert.Equal(deletedAt, veiculo.DeletedAt);
        Assert.Equal(updatedAt, veiculo.UpdatedAt);
    }

    private static Veiculo CriarVeiculo(
        Guid? pessoaId = null,
        Placa? placa = null,
        string marca = "Toyota",
        string modelo = "Corolla",
        int anoFabricacao = 2020,
        string cor = "Prata",
        Hodometro? hodometro = null)
    {
        return new Veiculo(
            pessoaId ?? Guid.NewGuid(),
            placa ?? CriarPlaca(),
            marca,
            modelo,
            anoFabricacao,
            cor,
            hodometro ?? CriarHodometro());
    }

    private static Placa CriarPlaca()
    {
        return new Placa("ABC1D23");
    }

    private static Hodometro CriarHodometro()
    {
        return new Hodometro(10_000);
    }
}