using Microsoft.EntityFrameworkCore;
using Ofichina.Domain.Entities;
using Ofichina.Domain.Interfaces;
using Ofichina.Infrastructure.Persistence;

namespace Ofichina.Infrastructure.Repositories;

/// <summary>
/// Implementação do repositório específico para Veiculo.
/// </summary>
public class VeiculoRepository : Repository<Veiculo>, IVeiculoRepository
{
    private readonly ApplicationDbContext _context;

    public VeiculoRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<Veiculo?> GetByIdWithPessoaAsync(Guid id)
    {
        return await _context.Set<Veiculo>()
            .AsNoTracking()
            .Include(x => x.Pessoa)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<IEnumerable<Veiculo>> GetAllWithPessoaAsync()
    {
        return await _context.Set<Veiculo>()
            .AsNoTracking()
            .Include(x => x.Pessoa)
            .ToListAsync();
    }
}