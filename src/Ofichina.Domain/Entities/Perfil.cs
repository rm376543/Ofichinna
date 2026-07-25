using Ofichina.Domain.Exceptions;

namespace Ofichina.Domain.Entities;

/// <summary>
/// Representa um perfil de autorização da aplicação.
/// Define um conjunto de permissões que podem ser atribuídas aos usuários.
/// </summary>
public class Perfil : Entity
{
    /// <summary>
    /// Nome identificador do perfil.
    /// Exemplo: Administrador, Mecânico, Atendente.
    /// </summary>
    public string NomePerfil { get; private set; } = string.Empty;


    /// <summary>
    /// Descrição das responsabilidades e permissões do perfil.
    /// </summary>
    public string Descricao { get; private set; } = string.Empty;


    /// <summary>
    /// Lista de vínculos entre usuários e este perfil.
    /// </summary>
    public ICollection<UsuarioPerfil> UsuariosPerfis { get; private set; } = [];

    /// <summary>
    /// Lista de vínculos entre perfis e permissoes.
    /// </summary>
    public ICollection<PerfilPermissao> PerfisPermissoes { get; private set; } = [];

    /// <summary>
    /// Indica se o perfil está ativo.
    /// Perfis desativados permanecem armazenados através de soft delete.
    /// </summary>
    public bool EstaAtivo => DeletedAt == null;


    /// <summary>
    /// Construtor utilizado pelo Entity Framework Core.
    /// </summary>
    private Perfil()
    {
    }


    /// <summary>
    /// Cria um novo perfil de autorização.
    /// </summary>
    /// <param name="nomePerfil">
    /// Nome do perfil.
    /// </param>
    /// <param name="descricao">
    /// Descrição das responsabilidades do perfil.
    /// </param>
    public Perfil(
        string nomePerfil,
        string descricao)
    {
        if (string.IsNullOrWhiteSpace(nomePerfil))
            throw new DomainException(
                "O nome do perfil deve ser informado.");


        if (string.IsNullOrWhiteSpace(descricao))
            throw new DomainException(
                "A descrição do perfil deve ser informada.");


        NomePerfil = nomePerfil.Trim();

        Descricao = descricao.Trim();
    }


    /// <summary>
    /// Altera o nome do perfil.
    /// </summary>
    /// <param name="nomePerfil">
    /// Novo nome do perfil.
    /// </param>
    public void AlterarNome(string nomePerfil)
    {
        if (string.IsNullOrWhiteSpace(nomePerfil))
            throw new DomainException(
                "O nome do perfil deve ser informado.");


        NomePerfil = nomePerfil.Trim();

        AtualizarDataModificacao();
    }


    /// <summary>
    /// Altera a descrição do perfil.
    /// </summary>
    /// <param name="descricao">
    /// Nova descrição do perfil.
    /// </param>
    public void AlterarDescricao(string descricao)
    {
        if (string.IsNullOrWhiteSpace(descricao))
            throw new DomainException(
                "A descrição do perfil deve ser informada.");


        Descricao = descricao.Trim();

        AtualizarDataModificacao();
    }


    /// <summary>
    /// Desativa o perfil através de exclusão lógica.
    /// </summary>
    public void Desativar()
    {
        Excluir();
    }

}