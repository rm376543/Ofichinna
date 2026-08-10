using Microsoft.EntityFrameworkCore;
using Ofichina.Application.Abstractions.Interfaces.Service;
using Ofichina.Domain.Entities;
using Ofichina.Domain.Enums;
using Ofichina.Infrastructure.Persistence;

namespace Ofichina.Infrastructure.Services;

public sealed class MecanicoDisponibilidadeService : IMecanicoDisponibilidadeService
{
    private readonly ApplicationDbContext _context;

    public MecanicoDisponibilidadeService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid?> ObterMecanicoDisponivelAsync(CancellationToken cancellationToken = default)
    {
        var mecanico = await _context.UsuariosPerfis
            .AsNoTracking()
            .Where(x => x.Perfil.NomePerfil.ToUpper() == "MECANICO")
            .Select(x => x.UsuarioId)
            .Distinct()
            .Join(_context.Pessoas.AsNoTracking(), usuarioId => usuarioId, pessoa => pessoa.UsuarioId, (_, pessoa) => pessoa)
            .OrderBy(x => x.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);

        if (mecanico is null)
            return null;

        var possuiOrdemEmExecucao = await _context.OrdensServico
            .AsNoTracking()
            .AnyAsync(x => x.MecanicoId == mecanico.Id && x.Status == StatusOrdemServico.EmExecucao, cancellationToken);

        return possuiOrdemEmExecucao ? null : mecanico.Id;
    }
}