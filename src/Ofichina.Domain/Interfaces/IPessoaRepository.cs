using Ofichina.Domain.Entities;

namespace Ofichina.Domain.Interfaces
{
    public interface IPessoaRepository : IRepository<Pessoa>
    {
        Task<Pessoa?> GetByUsuarioIdAsync(Guid usuarioId);
    }
}
