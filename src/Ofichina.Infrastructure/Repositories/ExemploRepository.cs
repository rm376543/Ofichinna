using Microsoft.EntityFrameworkCore;
using Ofichina.Domain.Entities;
using Ofichina.Domain.Interfaces;
using Ofichina.Infrastructure.Persistence;

namespace Ofichina.Infrastructure.Repositories;

/// <summary>
/// Implementação do repositório específico para Exemplo.
/// Estende o repositório genérico com métodos específicos do domínio.
/// </summary>
public class ExemploRepository : Repository<Exemplo>, IExemploRepository
{
    private readonly ApplicationDbContext _context;

    public ExemploRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<Exemplo?> GetByNameAsync(string nome)
    {
        return await _context.Set<Exemplo>()
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Nome == nome);
    }

    public async Task<IEnumerable<Exemplo>> GetAllAtivosAsync()
    {
        return await _context.Set<Exemplo>()
            .AsNoTracking()
            .Where(e => e.Ativo)
            .ToListAsync();
    }
}
