using Ofichina.Domain.Entities;

namespace Ofichina.Application.Abstractions.Interfaces.Repository;

public interface IChecklistRepository : IRepository<Checklist>
{
    Task<Checklist?> GetByAgendamentoChecklistIdAsync(Guid agendamentoId, Guid checklistId, CancellationToken cancellationToken = default, bool tracking = false);
}