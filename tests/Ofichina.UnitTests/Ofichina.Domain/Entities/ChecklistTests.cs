using Ofichina.Domain.Entities;
using Ofichina.Domain.Exceptions;

namespace Ofichina.UnitTests.Domain.Entities;

public class ChecklistTests
{
    [Fact]
    public void Checklist_DeveInicializar_ComDadosValidos()
    {
        var agendamentoId = Guid.NewGuid();
        var itensVerificados = "Pneus, óleo e freios";
        var observacoes = "Veículo recebido em boas condições.";

        var checklist = new Checklist(
            agendamentoId,
            itensVerificados,
            observacoes);

        Assert.Equal(agendamentoId, checklist.AgendamentoId);
        Assert.Equal(itensVerificados, checklist.ItensVerificados);
        Assert.Equal(observacoes, checklist.Observacoes);
        Assert.False(checklist.Finalizado);

        Assert.Null(checklist.Agendamento);

        Assert.NotEqual(Guid.Empty, checklist.Id);
        Assert.True(checklist.CreatedAt <= DateTime.UtcNow);
        Assert.Null(checklist.UpdatedAt);
        Assert.Null(checklist.DeletedAt);
    }

    [Fact]
    public void Checklist_DeveLancarExcecao_QuandoAgendamentoIdForVazio()
    {
        var exception = Assert.Throws<DomainException>(() =>
            new Checklist(
                Guid.Empty,
                "Pneus, óleo e freios",
                null));

        Assert.Equal("Agendamento obrigatório.", exception.Message);
    }

    [Fact]
    public void Checklist_DeveUsarStringVazia_QuandoItensVerificadosForNulo()
    {
        var checklist = new Checklist(
            Guid.NewGuid(),
            null!,
            null);

        Assert.Equal(string.Empty, checklist.ItensVerificados);
    }

    [Fact]
    public void Checklist_DevePermitir_ObservacoesNulas()
    {
        var checklist = new Checklist(
            Guid.NewGuid(),
            "Pneus e freios",
            null);

        Assert.Null(checklist.Observacoes);
    }

    [Fact]
    public void Checklist_DeveInicializar_FinalizadoComoFalse()
    {
        var checklist = new Checklist(
            Guid.NewGuid(),
            "Pneus e freios",
            null);

        Assert.False(checklist.Finalizado);
        Assert.False(checklist.EstaFinalizado());
    }

    [Fact]
    public void Checklist_Finalizar_DeveMarcarComoFinalizado()
    {
        var checklist = new Checklist(
            Guid.NewGuid(),
            "Pneus e freios",
            null);

        checklist.Finalizar();

        Assert.True(checklist.Finalizado);
        Assert.True(checklist.EstaFinalizado());
        Assert.NotNull(checklist.UpdatedAt);
    }

    [Fact]
    public void Checklist_Finalizar_DeveAtualizarDataDeModificacao()
    {
        var checklist = new Checklist(
            Guid.NewGuid(),
            "Pneus e freios",
            null);

        var antesDaFinalizacao = DateTime.UtcNow;

        checklist.Finalizar();

        var depoisDaFinalizacao = DateTime.UtcNow;

        Assert.NotNull(checklist.UpdatedAt);
        Assert.InRange(
            checklist.UpdatedAt.Value,
            antesDaFinalizacao,
            depoisDaFinalizacao);
    }

    [Fact]
    public void Checklist_Finalizar_DeveLancarExcecao_QuandoJaEstiverFinalizado()
    {
        var checklist = new Checklist(
            Guid.NewGuid(),
            "Pneus e freios",
            null);

        checklist.Finalizar();

        var updatedAt = checklist.UpdatedAt;

        var exception = Assert.Throws<DomainException>(() =>
            checklist.Finalizar());

        Assert.Equal(
            "O checklist já foi finalizado.",
            exception.Message);

        Assert.True(checklist.Finalizado);
        Assert.Equal(updatedAt, checklist.UpdatedAt);
    }

    [Fact]
    public void Checklist_VincularAgendamento_DeveAlterarAgendamentoId()
    {
        var agendamentoIdInicial = Guid.NewGuid();
        var novoAgendamentoId = Guid.NewGuid();

        var checklist = new Checklist(
            agendamentoIdInicial,
            "Pneus e freios",
            null);

        checklist.VincularAgendamento(novoAgendamentoId);

        Assert.Equal(novoAgendamentoId, checklist.AgendamentoId);
    }

    [Fact]
    public void Checklist_VincularAgendamento_DeveAtualizarDataDeModificacao()
    {
        var checklist = new Checklist(
            Guid.NewGuid(),
            "Pneus e freios",
            null);

        var antesDaVinculacao = DateTime.UtcNow;

        checklist.VincularAgendamento(Guid.NewGuid());

        var depoisDaVinculacao = DateTime.UtcNow;

        Assert.NotNull(checklist.UpdatedAt);
        Assert.InRange(
            checklist.UpdatedAt.Value,
            antesDaVinculacao,
            depoisDaVinculacao);
    }

    [Fact]
    public void Checklist_VincularAgendamento_DeveLancarExcecao_QuandoIdForVazio()
    {
        var agendamentoId = Guid.NewGuid();

        var checklist = new Checklist(
            agendamentoId,
            "Pneus e freios",
            null);

        var exception = Assert.Throws<DomainException>(() =>
            checklist.VincularAgendamento(Guid.Empty));

        Assert.Equal(
            "O agendamento deve ser informado.",
            exception.Message);

        Assert.Equal(
            agendamentoId,
            checklist.AgendamentoId);
    }

    [Fact]
    public void Checklist_Reabrir_DeveAlterarFinalizadoParaFalse()
    {
        var checklist = new Checklist(
            Guid.NewGuid(),
            "Pneus e freios",
            null);

        checklist.Finalizar();

        Assert.True(checklist.Finalizado);

        checklist.Reabrir();

        Assert.False(checklist.Finalizado);
        Assert.False(checklist.EstaFinalizado());
    }

    [Fact]
    public void Checklist_Reabrir_DevePermanecerAberto_QuandoJaEstiverAberto()
    {
        var checklist = new Checklist(
            Guid.NewGuid(),
            "Pneus e freios",
            null);

        var updatedAtAntes = checklist.UpdatedAt;

        checklist.Reabrir();

        Assert.False(checklist.Finalizado);
        Assert.Null(checklist.UpdatedAt);
        Assert.Equal(updatedAtAntes, checklist.UpdatedAt);
    }

    [Fact]
    public void Checklist_EstaFinalizado_DeveRetornarFalse_QuandoAberto()
    {
        var checklist = new Checklist(
            Guid.NewGuid(),
            "Pneus e freios",
            null);

        Assert.False(checklist.EstaFinalizado());
    }

    [Fact]
    public void Checklist_EstaFinalizado_DeveRetornarTrue_QuandoFinalizado()
    {
        var checklist = new Checklist(
            Guid.NewGuid(),
            "Pneus e freios",
            null);

        checklist.Finalizar();

        Assert.True(checklist.EstaFinalizado());
    }
}