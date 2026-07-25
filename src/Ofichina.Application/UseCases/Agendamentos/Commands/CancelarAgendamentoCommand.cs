using Ofichina.Application.Abstractions;
using Ofichina.Contracts.Common;

namespace Ofichina.Application.UseCases.Agendamentos.Commands
{
    /// <summary>
    /// Comando para cancelar um agendamento existente.
    /// </summary>
    public sealed class CancelarAgendamentoCommand : ICommand<Result>
    {
        public Guid PessoaId { get; set; }
        public Guid AgendamentoId { get; init; }
        public CancelarAgendamentoCommand(Guid pessoaId, Guid agendamentoId)
        {
            PessoaId = pessoaId;
            AgendamentoId = agendamentoId;
        }
    }
}
