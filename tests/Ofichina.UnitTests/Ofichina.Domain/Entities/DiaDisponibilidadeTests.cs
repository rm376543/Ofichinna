using Ofichina.Domain.Entities;
using Ofichina.Domain.Exceptions;

namespace Ofichina.UnitTests.Domain.Entities;

public class DiaDisponibilidadeTests
{
    [Fact]
    public void DiaDisponibilidade_DeveInicializar_ComDataValida()
    {
        var data = new DateOnly(2026, 8, 20);

        var dia = new DiaDisponibilidade(data);

        Assert.Equal(data, dia.Data);
        Assert.Empty(dia.Horarios);

        Assert.NotEqual(Guid.Empty, dia.Id);
        Assert.True(dia.CreatedAt <= DateTime.UtcNow);
        Assert.Null(dia.UpdatedAt);
        Assert.Null(dia.DeletedAt);
    }

    [Fact]
    public void DiaDisponibilidade_DeveLancarExcecao_QuandoDataForDefault()
    {
        var exception = Assert.Throws<DomainException>(() =>
            new DiaDisponibilidade(default));

        Assert.Equal(
            "A data do dia disponível deve ser informada.",
            exception.Message);
    }

    [Fact]
    public void DiaDisponibilidade_AlterarData_DeveAlterarData()
    {
        var dataInicial = new DateOnly(2026, 8, 20);
        var novaData = new DateOnly(2026, 8, 25);

        var dia = new DiaDisponibilidade(dataInicial);

        dia.AlterarData(novaData);

        Assert.Equal(novaData, dia.Data);
    }

    [Fact]
    public void DiaDisponibilidade_AlterarData_DeveAtualizarDataDeModificacao()
    {
        var dia = new DiaDisponibilidade(
            new DateOnly(2026, 8, 20));

        var antesDaAlteracao = DateTime.UtcNow;

        dia.AlterarData(new DateOnly(2026, 8, 25));

        var depoisDaAlteracao = DateTime.UtcNow;

        Assert.NotNull(dia.UpdatedAt);
        Assert.InRange(
            dia.UpdatedAt.Value,
            antesDaAlteracao,
            depoisDaAlteracao);
    }

    [Fact]
    public void DiaDisponibilidade_AlterarData_DeveLancarExcecao_QuandoDataForDefault()
    {
        var dataInicial = new DateOnly(2026, 8, 20);

        var dia = new DiaDisponibilidade(dataInicial);

        var exception = Assert.Throws<DomainException>(() =>
            dia.AlterarData(default));

        Assert.Equal(
            "A data do dia disponível deve ser informada.",
            exception.Message);

        Assert.Equal(dataInicial, dia.Data);
        Assert.Null(dia.UpdatedAt);
    }

    [Fact]
    public void DiaDisponibilidade_DeveInicializar_HorariosComoColecaoVazia()
    {
        var dia = new DiaDisponibilidade(
            new DateOnly(2026, 8, 20));

        Assert.NotNull(dia.Horarios);
        Assert.Empty(dia.Horarios);
    }

    [Fact]
    public void DiaDisponibilidade_AdicionarHorario_DeveAdicionarHorario()
    {
        var dia = new DiaDisponibilidade(
            new DateOnly(2026, 8, 20));

        var horario = CriarHorario();

        dia.AdicionarHorario(horario);

        Assert.Single(dia.Horarios);
        Assert.Contains(horario, dia.Horarios);
    }

    [Fact]
    public void DiaDisponibilidade_AdicionarHorario_DeveAtualizarDataDeModificacao()
    {
        var dia = new DiaDisponibilidade(
            new DateOnly(2026, 8, 20));

        var horario = CriarHorario();

        var antesDaAdicao = DateTime.UtcNow;

        dia.AdicionarHorario(horario);

        var depoisDaAdicao = DateTime.UtcNow;

        Assert.NotNull(dia.UpdatedAt);
        Assert.InRange(
            dia.UpdatedAt.Value,
            antesDaAdicao,
            depoisDaAdicao);
    }

    [Fact]
    public void DiaDisponibilidade_AdicionarHorario_DeveLancarExcecao_QuandoHorarioForNulo()
    {
        var dia = new DiaDisponibilidade(
            new DateOnly(2026, 8, 20));

        var exception = Assert.Throws<ArgumentNullException>(() =>
            dia.AdicionarHorario(null!));

        Assert.Equal("horario", exception.ParamName);
        Assert.Empty(dia.Horarios);
        Assert.Null(dia.UpdatedAt);
    }

    [Fact]
    public void DiaDisponibilidade_AdicionarHorario_DevePermitirMultiplosHorarios()
    {
        var dia = new DiaDisponibilidade(
            new DateOnly(2026, 8, 20));

        var horario1 = CriarHorario();
        var horario2 = CriarHorario();

        dia.AdicionarHorario(horario1);
        dia.AdicionarHorario(horario2);

        Assert.Equal(2, dia.Horarios.Count);
        Assert.Contains(horario1, dia.Horarios);
        Assert.Contains(horario2, dia.Horarios);
    }

    [Fact]
    public void DiaDisponibilidade_Horarios_NaoDevePermitirAlteracaoDireta()
    {
        var dia = new DiaDisponibilidade(
            new DateOnly(2026, 8, 20));

        var horario = CriarHorario();

        dia.AdicionarHorario(horario);

        Assert.IsAssignableFrom<IReadOnlyCollection<DiaHorarioDisponibilidade>>(
            dia.Horarios);

        Assert.Single(dia.Horarios);
    }

    private static DiaHorarioDisponibilidade CriarHorario()
    {
        // Ajuste esta criação conforme o construtor real
        // de DiaHorarioDisponibilidade.
        return new DiaHorarioDisponibilidade();
    }
}