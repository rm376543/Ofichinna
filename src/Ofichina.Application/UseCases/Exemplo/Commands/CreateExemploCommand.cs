using Ofichina.Application.Abstractions;

namespace Ofichina.Application.UseCases.Exemplo.Commands;

/// <summary>
/// Comando para criar um novo Exemplo.
/// </summary>
public class CreateExemploCommand : ICommand<Guid>
{
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }

    public CreateExemploCommand(string nome, string? descricao = null)
    {
        Nome = nome;
        Descricao = descricao;
    }
}
