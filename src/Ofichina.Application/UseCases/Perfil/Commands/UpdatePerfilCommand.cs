using Ofichina.Application.Abstractions;
using Ofichina.Domain.ValueObjects;

namespace Ofichina.Application.UseCases.Perfis.Commands;

/// <summary>
/// Comando para atualizar um perfil.
/// </summary>
public class UpdatePerfilCommand : ICommand<Result>
{
    public Guid Id { get; set; }

    public string Codigo { get; set; } = string.Empty;

    public string Nome { get; set; } = string.Empty;

    public string? Descricao { get; set; }

    public bool Ativo { get; set; }

    public UpdatePerfilCommand(Guid id, string codigo, string nome, string? descricao, bool ativo)
    {
        Id = id;
        Codigo = codigo;
        Nome = nome;
        Descricao = descricao;
        Ativo = ativo;
    }
}