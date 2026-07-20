using Microsoft.Extensions.Logging;
using Ofichina.Application.Abstractions;
using Ofichina.Application.UseCases.Agendamentos.Commands;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Responses.Agendamento;
using Ofichina.Domain.Aggregates;
using Ofichina.Domain.Exceptions;
using Ofichina.Authentication.Abstractions;
using Ofichina.Domain.Entities;
using Ofichina.Domain.Interfaces;

namespace Ofichina.Application.UseCases.Agendamentos.Handlers;

/// <summary>
/// Handler para criação de agendamento.
/// </summary>
public sealed class CreateAgendamentoCommandHandler : ICommandHandler<CreateAgendamentoCommand, Result<AgendamentoResponse>>
{
    private readonly IAgendamentoRepository _agendamentoRepository;
    private readonly IPessoaRepository _pessoaRepository;
    private readonly IVeiculoRepository _veiculoRepository;
    private readonly IDiaDisponibilidadeRepository _diaDisponibilidadeRepository;
    private readonly IHorarioDisponibilidadeRepository _horarioDisponibilidadeRepository;
    private readonly IHorarioConsultorRepository _horarioConsultorRepository;
    private readonly IPerfilAutorizacaoService _perfilAutorizacaoService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateAgendamentoCommandHandler> _logger;

#pragma warning disable S107
    public CreateAgendamentoCommandHandler(
        IAgendamentoRepository agendamentoRepository,
        IPessoaRepository pessoaRepository,
        IVeiculoRepository veiculoRepository,
        IDiaDisponibilidadeRepository diaDisponibilidadeRepository,
        IHorarioDisponibilidadeRepository horarioDisponibilidadeRepository,
        IHorarioConsultorRepository horarioConsultorRepository,
        IPerfilAutorizacaoService perfilAutorizacaoService,
        IUnitOfWork unitOfWork,
        ILogger<CreateAgendamentoCommandHandler> logger)
#pragma warning restore S107
    {
        _agendamentoRepository = agendamentoRepository;
        _pessoaRepository = pessoaRepository;
        _veiculoRepository = veiculoRepository;
        _diaDisponibilidadeRepository = diaDisponibilidadeRepository;
        _horarioDisponibilidadeRepository = horarioDisponibilidadeRepository;
        _horarioConsultorRepository = horarioConsultorRepository;
        _perfilAutorizacaoService = perfilAutorizacaoService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<AgendamentoResponse>> HandleAsync(CreateAgendamentoCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Iniciando a criação de agendamento para a pessoa {PessoaId}.", command.PessoaId);

            var pessoa = await _pessoaRepository.GetByIdAsync(command.PessoaId, cancellationToken);
            if (pessoa is null || pessoa.EstaExcluida())
                return Result.Failure<AgendamentoResponse>("Pessoa não encontrada.");

            var veiculo = await _veiculoRepository.GetByIdWithPessoaAsync(command.VeiculoId, cancellationToken);
            if (veiculo is null || veiculo.EstaExcluida())
                return Result.Failure<AgendamentoResponse>("Veículo não encontrado.");

            if (veiculo.PessoaId != pessoa.Id)
                return Result.Failure<AgendamentoResponse>("O veículo informado não pertence ao usuário autenticado.");

            var dia = await _diaDisponibilidadeRepository.GetByIdAsync(command.DiaDisponibilidadeId, cancellationToken);
            if (dia is null || dia.EstaExcluida())
                return Result.Failure<AgendamentoResponse>("Dia de disponibilidade não encontrado.");

            var horarioConsultor = await _horarioConsultorRepository.GetByIdAsync(command.HorarioConsultorId, cancellationToken);
            if (horarioConsultor is null || horarioConsultor.EstaExcluida())
                return Result.Failure<AgendamentoResponse>("Horário do consultor não encontrado.");

            var horariosDoDia = await _horarioDisponibilidadeRepository.GetHorariosPorDiaAsync(command.DiaDisponibilidadeId, cancellationToken);
            var horarioPertenceAoDia = horariosDoDia.Any(x => x.Id == horarioConsultor.HorarioDisponibilidadeId);

            if (!horarioPertenceAoDia)
                return Result.Failure<AgendamentoResponse>("O horário informado não pertence ao dia selecionado.");

            var consultor = await _pessoaRepository.GetByIdAsync(horarioConsultor.PessoaId, cancellationToken);
            if (consultor is null || consultor.EstaExcluida())
                return Result.Failure<AgendamentoResponse>("Consultor não encontrado.");

            var possuiPerfilConsultor = await _perfilAutorizacaoService.PossuiPerfilAsync(consultor.UsuarioId, "CONSULTOR", cancellationToken);
            if (!possuiPerfilConsultor)
                return Result.Failure<AgendamentoResponse>("A pessoa informada não possui perfil de consultor.");

            if (await _agendamentoRepository.ExisteConflitoConsultorAsync(command.HorarioConsultorId, cancellationToken))
                return Result.Failure<AgendamentoResponse>("Já existe um agendamento para este horário.");

            if (await _agendamentoRepository.ExisteConflitoVeiculoAsync(command.VeiculoId, command.DiaDisponibilidadeId, command.HorarioConsultorId, cancellationToken))
                return Result.Failure<AgendamentoResponse>("Já existe um agendamento para este veículo neste horário.");

            var agendamento = new Agendamento(
                command.PessoaId,
                command.DiaDisponibilidadeId,
                command.HorarioConsultorId,
                consultor.Id,
                command.VeiculoId,
                command.Descricao);

            await _agendamentoRepository.AddAsync(agendamento, cancellationToken);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Agendamento criado com sucesso. AgendamentoId: {AgendamentoId}", agendamento.Id);

            return Result.Success(Mapear(agendamento, pessoa, consultor, veiculo));
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Erro de domínio ao criar agendamento.");
            return Result.Failure<AgendamentoResponse>(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao criar agendamento.");
            return Result.Failure<AgendamentoResponse>("Não foi possível criar o agendamento.");
        }
    }

    private static AgendamentoResponse Mapear(
        Agendamento agendamento,
        Pessoa pessoa,
        Pessoa consultor,
        Veiculo veiculo)
    {
        return new AgendamentoResponse
        {
            Id = agendamento.Id,
            ClientePessoaId = agendamento.ClientePessoaId,
            ClienteNome = pessoa.Nome,
            DiaDisponibilidadeId = agendamento.DiaDisponibilidadeId,
            HorarioConsultorId = agendamento.HorarioConsultorId,
            ConsultorPessoaId = agendamento.ConsultorPessoaId,
            ConsultorNome = consultor.Nome,
            VeiculoId = agendamento.VeiculoId,
            VeiculoPlaca = veiculo.Placa.Numero,
            VeiculoDescricao = $"{veiculo.Marca} {veiculo.Modelo} {veiculo.AnoFabricacao}",
            Status = agendamento.Status.ToString(),
            Descricao = agendamento.Descricao,
            CreatedAt = agendamento.CreatedAt,
            UpdatedAt = agendamento.UpdatedAt,
            DeletedAt = agendamento.DeletedAt
        };
    }
}


