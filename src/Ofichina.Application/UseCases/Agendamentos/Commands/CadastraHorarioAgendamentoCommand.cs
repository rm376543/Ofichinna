using Ofichina.Application.Abstractions;
using Ofichina.Contracts.Common;
using System.Windows.Input;

namespace Ofichina.Application.UseCases.Agendamentos.Commands
{
    /// <summary>
    /// Representa o comando para cadastrar horários disponíveis para agendamento.
    /// </summary>
    public sealed class CadastraHorarioAgendamentoCommand : ICommand<Result>
    {
        public TimeOnly Horario { get; set; }

        public CadastraHorarioAgendamentoCommand(TimeOnly horario)
        {
            Horario = horario;
        }
    }
}
