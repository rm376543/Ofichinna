using Ofichina.Domain.Entities;
using System.Reflection;

namespace Ofichina.UnitTests.Domain.Entities;

public sealed class HorarioDisponibilidadeTests
{
    // ============================================================  
    // CONSTRUTOR  
    // ============================================================  

    [Fact]
    public void Deve_Criar_Horario_Com_Hora_Informada()
    {
        // Arrange  
        var hora = new TimeOnly(9, 30);

        // Act  
        var horario = new HorarioDisponibilidade(hora);

        // Assert  
        Assert.Equal(hora, horario.Hora);
        Assert.Empty(horario.Consultores);
        Assert.Empty(horario.Dias);
    }

    [Fact]
    public void Construtor_Sem_Parametros_Deve_Inicializar_Colecoes_Vazias()
    {
        // Arrange & Act (construtor privado usado pelo EF, acessado via reflexão)  
        var horario = (HorarioDisponibilidade)Activator.CreateInstance(
            typeof(HorarioDisponibilidade),
            BindingFlags.Instance | BindingFlags.NonPublic,
            binder: null,
            args: null,
            culture: null)!;

        // Assert  
        Assert.NotNull(horario);
        Assert.Empty(horario.Consultores);
        Assert.Empty(horario.Dias);
    }

    // ============================================================  
    // AlterarHora  
    // ============================================================  

    [Fact]
    public void AlterarHora_Deve_Atualizar_Hora_E_DataModificacao()
    {
        // Arrange  
        var horario = new HorarioDisponibilidade(new TimeOnly(8, 0));
        var novaHora = new TimeOnly(14, 45);

        // Act  
        horario.AlterarHora(novaHora);

        // Assert  
        Assert.Equal(novaHora, horario.Hora);
        Assert.NotNull(horario.UpdatedAt);
    }

    // ============================================================  
    // VincularConsultor  
    // ============================================================  

    [Fact]
    public void VincularConsultor_Deve_Adicionar_Consultor_E_Atualizar_DataModificacao()
    {
        // Arrange  
        var horario = new HorarioDisponibilidade(new TimeOnly(10, 0));
        var consultor = new HorarioConsultor(Guid.NewGuid(), Guid.NewGuid());

        // Act  
        horario.VincularConsultor(consultor);

        // Assert  
        Assert.Single(horario.Consultores);
        Assert.Contains(consultor, horario.Consultores);
        Assert.NotNull(horario.UpdatedAt);
    }

    [Fact]
    public void VincularConsultor_Deve_Lancar_Excecao_Quando_Consultor_For_Nulo()
    {
        // Arrange  
        var horario = new HorarioDisponibilidade(new TimeOnly(10, 0));

        // Act & Assert  
        Assert.Throws<ArgumentNullException>(() => horario.VincularConsultor(null!));
    }

    // ============================================================  
    // VincularDia  
    // ============================================================  

    [Fact]
    public void VincularDia_Deve_Adicionar_Dia_E_Atualizar_DataModificacao()
    {
        // Arrange  
        var horario = new HorarioDisponibilidade(new TimeOnly(11, 0));
        var diaHorario = new DiaHorarioDisponibilidade(Guid.NewGuid(), Guid.NewGuid());

        // Act  
        horario.VincularDia(diaHorario);

        // Assert  
        Assert.Single(horario.Dias);
        Assert.Contains(diaHorario, horario.Dias);
        Assert.NotNull(horario.UpdatedAt);
    }

    [Fact]
    public void VincularDia_Deve_Lancar_Excecao_Quando_DiaHorario_For_Nulo()
    {
        // Arrange  
        var horario = new HorarioDisponibilidade(new TimeOnly(11, 0));

        // Act & Assert  
        Assert.Throws<ArgumentNullException>(() => horario.VincularDia(null!));
    }
}