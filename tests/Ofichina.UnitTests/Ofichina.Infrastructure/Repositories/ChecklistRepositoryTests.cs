using Microsoft.EntityFrameworkCore;
using Ofichina.Domain.Entities;
using Ofichina.Infrastructure.Persistence;
using Ofichina.Infrastructure.Repositories;

namespace Ofichina.UnitTests.Ofichina.Infrastructure.Repositories;

public sealed class ChecklistRepositoryTests
{
    // ============================================================  
    // CONSTRUTOR  
    // ============================================================  

    [Fact]
    public void ChecklistRepository_ComContextoValido_Deve_CriarInstancia()
    {
        // Arrange  
        using var context = CriarContexto(Guid.NewGuid().ToString());

        // Act  
        var repository = new ChecklistRepository(context);

        // Assert  
        Assert.NotNull(repository);
    }

    // ============================================================  
    // GetByAgendamentoChecklistIdAsync - SUCESSO (sem tracking)  
    // ============================================================  

    [Fact]
    public async Task GetByAgendamentoChecklistIdAsync_QuandoNaoRastreado_Deve_Retornar_Checklist_Sem_Tracking()
    {
        // Arrange  
        var dbName = Guid.NewGuid().ToString();
        var agendamentoId = Guid.NewGuid();
        var checklistAlvo = new Checklist(agendamentoId, "Itens verificados", "Observações");
        var outroChecklist = new Checklist(Guid.NewGuid(), "Outros itens", null);

        await using (var seedContext = CriarContexto(dbName))
        {
            seedContext.AddRange(checklistAlvo, outroChecklist);
            await seedContext.SaveChangesAsync();
        }

        await using var readContext = CriarContexto(dbName);
        var repository = new ChecklistRepository(readContext);

        // Act  
        var resultado = await repository.GetByAgendamentoChecklistIdAsync(
            agendamentoId,
            checklistAlvo.Id);

        // Assert  
        Assert.NotNull(resultado);
        Assert.Equal(checklistAlvo.Id, resultado!.Id);
        Assert.Equal(agendamentoId, resultado.AgendamentoId);
        Assert.Empty(readContext.ChangeTracker.Entries<Checklist>());
    }

    // ============================================================  
    // GetByAgendamentoChecklistIdAsync - SUCESSO (com tracking)  
    // ============================================================  

    [Fact]
    public async Task GetByAgendamentoChecklistIdAsync_QuandoRastreado_Deve_Retornar_Checklist_Com_Tracking()
    {
        // Arrange  
        var dbName = Guid.NewGuid().ToString();
        var agendamentoId = Guid.NewGuid();
        var checklistAlvo = new Checklist(agendamentoId, "Itens verificados", "Observações");

        await using (var seedContext = CriarContexto(dbName))
        {
            seedContext.Add(checklistAlvo);
            await seedContext.SaveChangesAsync();
        }

        await using var readContext = CriarContexto(dbName);
        var repository = new ChecklistRepository(readContext);

        // Act  
        var resultado = await repository.GetByAgendamentoChecklistIdAsync(
            agendamentoId,
            checklistAlvo.Id,
            tracking: true);

        // Assert  
        Assert.NotNull(resultado);
        Assert.Equal(checklistAlvo.Id, resultado!.Id);
        Assert.NotEmpty(readContext.ChangeTracker.Entries<Checklist>());
    }

    // ============================================================  
    // GetByAgendamentoChecklistIdAsync - NÃO ENCONTRADO  
    // ============================================================  

    [Fact]
    public async Task GetByAgendamentoChecklistIdAsync_QuandoNaoExiste_Deve_Retornar_Null()
    {
        // Arrange  
        var dbName = Guid.NewGuid().ToString();
        var checklistExistente = new Checklist(Guid.NewGuid(), "Itens verificados", null);

        await using (var seedContext = CriarContexto(dbName))
        {
            seedContext.Add(checklistExistente);
            await seedContext.SaveChangesAsync();
        }

        await using var readContext = CriarContexto(dbName);
        var repository = new ChecklistRepository(readContext);

        // Act  
        var resultado = await repository.GetByAgendamentoChecklistIdAsync(
            Guid.NewGuid(),
            Guid.NewGuid());

        // Assert  
        Assert.Null(resultado);
    }

    // ============================================================  
    // HELPERS  
    // ============================================================  

    private static ApplicationDbContext CriarContexto(string dbName)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;

        return new ApplicationDbContext(options);
    }
}