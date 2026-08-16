using Ofichina.Domain.Entities;
using Ofichina.Domain.Exceptions;

namespace Ofichina.UnitTests.Ofichina.Domain.Entities;

public class MotivoRecusaOrcamentoTests
{
    [Fact]
    public void MotivoRecusaOrcamento_DeveInicializar_ComOrcamentoIdEValido()
    {
        var orcamentoId = Guid.NewGuid();

        var motivo = new MotivoRecusaOrcamento(
            orcamentoId,
            "Cliente encontrou uma proposta melhor.");

        Assert.Equal(orcamentoId, motivo.OrcamentoId);
        Assert.Equal(
            "Cliente encontrou uma proposta melhor.",
            motivo.Descricao);

        Assert.Null(motivo.Orcamento);

        Assert.NotEqual(Guid.Empty, motivo.Id);
        Assert.True(motivo.CreatedAt <= DateTime.UtcNow);
        Assert.Null(motivo.UpdatedAt);
        Assert.Null(motivo.DeletedAt);
    }

    [Fact]
    public void MotivoRecusaOrcamento_DeveLancarExcecao_QuandoOrcamentoIdForVazio()
    {
        var exception = Assert.Throws<DomainException>(() =>
            new MotivoRecusaOrcamento(
                Guid.Empty,
                "Cliente recusou o orçamento."));

        Assert.Equal("Orçamento obrigatório.", exception.Message);
    }

    [Fact]
    public void MotivoRecusaOrcamento_DevePermitir_DescricaoNula()
    {
        var orcamentoId = Guid.NewGuid();

        var motivo = new MotivoRecusaOrcamento(
            orcamentoId,
            null);

        Assert.Equal(orcamentoId, motivo.OrcamentoId);
        Assert.Null(motivo.Descricao);
    }

    [Fact]
    public void MotivoRecusaOrcamento_DeveConverter_DescricaoVaziaParaNula()
    {
        var motivo = new MotivoRecusaOrcamento(
            Guid.NewGuid(),
            string.Empty);

        Assert.Null(motivo.Descricao);
    }

    [Fact]
    public void MotivoRecusaOrcamento_DeveConverter_DescricaoComEspacosParaNula()
    {
        var motivo = new MotivoRecusaOrcamento(
            Guid.NewGuid(),
            "   ");

        Assert.Null(motivo.Descricao);
    }

    [Fact]
    public void MotivoRecusaOrcamento_DeveRemoverEspacosDasExtremidadesDaDescricao()
    {
        var motivo = new MotivoRecusaOrcamento(
            Guid.NewGuid(),
            "  Cliente recusou por preço.  ");

        Assert.Equal(
            "Cliente recusou por preço.",
            motivo.Descricao);
    }

    [Fact]
    public void MotivoRecusaOrcamento_DevePreservar_DescricaoComConteudoInterno()
    {
        var motivo = new MotivoRecusaOrcamento(
            Guid.NewGuid(),
            "  Cliente   recusou   o orçamento.  ");

        Assert.Equal(
            "Cliente   recusou   o orçamento.",
            motivo.Descricao);
    }

    [Fact]
    public void MotivoRecusaOrcamento_DevePermitir_DescricaoComUmCaractere()
    {
        var motivo = new MotivoRecusaOrcamento(
            Guid.NewGuid(),
            "A");

        Assert.Equal("A", motivo.Descricao);
    }
}