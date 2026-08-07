using Ofichina.Application.Abstractions;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Requests.Agendamento;

namespace Ofichina.Application.UseCases.Agendamentos.Commands
{
    /// <summary>
    /// Comando para cancelar um agendamento existente.
    /// </summary>
    public sealed class CancelarAgendamentoCommand : ICommand<Result>
    {
        /// <summary>
        /// Identificador único do agendamento a ser cancelado.
        /// </summary>
        public Guid AgendamentoId { get; init; }
        public CancelarAgendamentoCommand(CancelarAgendamentoRequest request)
        {
            AgendamentoId = request.AgendamentoId;
        }
    }
}

