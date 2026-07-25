namespace Ofichina.Domain.Common
{
    /// <summary>
    /// Classe base para entidades que possuem informações de auditoria, como data de criação, data de modificação e usuário responsável pelas alterações.
    /// </summary>
    public abstract class Audit
    {
        /// <summary>
        /// Data de criação da entidade.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Data da última atualização da entidade.
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Data de exclusão (soft-delete) da entidade.
        /// </summary>
        public DateTime? DeletedAt { get; set; }
    }
}
