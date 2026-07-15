using Ofichina.Domain.Exceptions;

namespace Ofichina.Domain.Entities;

public class Permissao : Entity
{
    public string Codigo { get; private set; } = string.Empty;

    public string Descricao { get; private set; } = string.Empty;

    public ICollection<PerfilPermissao> PerfisPermissoes { get; private set; } = [];

    private Permissao()
    {
    }

    public Permissao(string codigo, string descricao)
    {
        if (string.IsNullOrWhiteSpace(codigo))
            throw new DomainException("O código da permissão deve ser informado.");

        if (string.IsNullOrWhiteSpace(descricao))
            throw new DomainException("A descrição da permissão deve ser informada.");

        Codigo = codigo.Trim();
        Descricao = descricao.Trim();
    }
}