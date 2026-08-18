using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Ofichina.Api.Controllers.Agendamento;
using Ofichina.Application.UseCases.Agendamentos.Commands;
using Ofichina.Application.UseCases.Agendamentos.Queries;
using Ofichina.Application.Validators.Agendamento;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Requests.Agendamento;
using Ofichina.Contracts.Responses.Agendamento;
using Ofichina.Contracts.Responses.Agendamento.Consultor;
using Ofichina.UnitTests.Api.TestDoubles;

namespace Ofichina.UnitTests.Api.Controllers.Agendamento;

public sealed class AgendamentoControllerTests
{
    #region BuscarHorarios

    [Fact]
    public async Task BuscarHorarios_Deve_Retornar_Sucesso()
    {
        var mediator = new FakeMediator();

        mediator.RegistrarResposta<
            GetHorariosDisponiveisQuery,
            Result<PagedResponse<HorarioResponse>>>(
            Result.Success(new PagedResponse<HorarioResponse>
            {
                Items =
                [
                    new HorarioResponse
                    {
                        HorarioId = Guid.NewGuid(),
                        Horario = new TimeOnly(8, 0),
                        Disponivel = true
                    }
                ],
                PageNumber = 1,
                PageSize = 10,
                TotalCount = 1,
                TotalPages = 1,
                HasNextPage = false,
                HasPreviousPage = false
            }));

        var controller = CriarController(mediator);

        var result = await controller.BuscarHorarios(new Pagination(1, 10));

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var response =
            Assert.IsType<ApiResponse<PagedResponse<HorarioResponse>>>(ok.Value);

        Assert.True(response.Success);
        Assert.NotNull(response.Data);
        Assert.Single(response.Data!.Items);
    }

    [Fact]
    public async Task BuscarHorarios_Deve_Retornar_BadRequest_Quando_Result_Falhar()
    {
        var mediator = new FakeMediator();

        mediator.RegistrarResposta<
            GetHorariosDisponiveisQuery,
            Result<PagedResponse<HorarioResponse>>>(
            Result.Failure<PagedResponse<HorarioResponse>>(
                "Não foi possível buscar os horários."));

        var controller = CriarController(mediator);

        var result = await controller.BuscarHorarios(new Pagination(1, 10));

        var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse>(badRequest.Value);

        Assert.False(response.Success);
        Assert.Equal(
            "Não foi possível buscar os horários.",
            response.Message);
    }

    [Fact]
    public async Task BuscarHorarios_Deve_Retornar_Mensagem_Padrao_Quando_Result_Falhar_Sem_Error()
    {
        var mediator = new FakeMediator();

        mediator.RegistrarResposta<
            GetHorariosDisponiveisQuery,
            Result<PagedResponse<HorarioResponse>>>(
            Result.Failure<PagedResponse<HorarioResponse>>((string)null!));

        var controller = CriarController(mediator);

        var result = await controller.BuscarHorarios(new Pagination(1, 10));

        var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse>(badRequest.Value);

        Assert.False(response.Success);
        Assert.Equal(
            "Não foi possível obter os horários disponíveis.",
            response.Message);
    }

    [Fact]
    public async Task BuscarHorarios_Deve_Retornar_BadRequest_Quando_Value_For_Null()
    {
        var mediator = new FakeMediator();

        mediator.RegistrarResposta<
            GetHorariosDisponiveisQuery,
            Result<PagedResponse<HorarioResponse>>>(
            Result.Success<PagedResponse<HorarioResponse>>(null!));

        var controller = CriarController(mediator);

        var result = await controller.BuscarHorarios(new Pagination(1, 10));

        var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse>(badRequest.Value);

        Assert.False(response.Success);
        Assert.Equal(
            "Não foi possível obter os horários disponíveis.",
            response.Message);
    }

    #endregion

    #region ListarAsync

    [Fact]
    public async Task ListarAsync_Deve_Retornar_Sucesso()
    {
        var mediator = new FakeMediator();

        mediator.RegistrarResposta<
            GetAgendamentosQuery,
            Result<IReadOnlyCollection<AgendamentoUsuarioResponse>>>(
            Result.Success<IReadOnlyCollection<AgendamentoUsuarioResponse>>(
                Array.Empty<AgendamentoUsuarioResponse>()));

        var controller = CriarController(mediator);

        var result = await controller.ListarAsync(
            Guid.NewGuid(),
            CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var response =
            Assert.IsType<ApiResponse<IReadOnlyCollection<AgendamentoUsuarioResponse>>>(
                ok.Value);

        Assert.True(response.Success);
        Assert.NotNull(response.Data);
        Assert.Empty(response.Data!);
    }

    [Fact]
    public async Task ListarAsync_Deve_Retornar_BadRequest_Quando_Result_Falhar()
    {
        var mediator = new FakeMediator();

        mediator.RegistrarResposta<
            GetAgendamentosQuery,
            Result<IReadOnlyCollection<AgendamentoUsuarioResponse>>>(
            Result.Failure<IReadOnlyCollection<AgendamentoUsuarioResponse>>(
                "Pessoa não encontrada."));

        var controller = CriarController(mediator);

        var result = await controller.ListarAsync(
            Guid.NewGuid(),
            CancellationToken.None);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse>(badRequest.Value);

        Assert.False(response.Success);
        Assert.Equal(
            "Pessoa não encontrada.",
            response.Message);
    }

    [Fact]
    public async Task ListarAsync_Deve_Usar_Mensagem_Padrao_Quando_Error_For_Null()
    {
        var mediator = new FakeMediator();

        mediator.RegistrarResposta<
            GetAgendamentosQuery,
            Result<IReadOnlyCollection<AgendamentoUsuarioResponse>>>(
            Result.Failure<IReadOnlyCollection<AgendamentoUsuarioResponse>>(
                (string)null!));

        var controller = CriarController(mediator);

        var result = await controller.ListarAsync(
            Guid.NewGuid(),
            CancellationToken.None);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse>(badRequest.Value);

        Assert.False(response.Success);
        Assert.Equal(
            "Não foi possível obter os agendamentos.",
            response.Message);
    }

    #endregion

    #region ObterPorIdAsync

    [Fact]
    public async Task ObterPorIdAsync_Deve_Retornar_Sucesso()
    {
        var mediator = new FakeMediator();

        mediator.RegistrarResposta<
            GetAgendamentoByIdQuery,
            Result<AgendamentoUsuarioDetalheResponse>>(
            Result.Success(new AgendamentoUsuarioDetalheResponse()));

        var controller = CriarController(mediator);

        var result = await controller.ObterPorIdAsync(
            Guid.NewGuid(),
            Guid.NewGuid(),
            CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var response =
            Assert.IsType<ApiResponse<AgendamentoUsuarioDetalheResponse>>(
                ok.Value);

        Assert.True(response.Success);
        Assert.NotNull(response.Data);
    }

    [Fact]
    public async Task ObterPorIdAsync_Deve_Retornar_NotFound_Quando_Result_Falhar()
    {
        var mediator = new FakeMediator();

        mediator.RegistrarResposta<
            GetAgendamentoByIdQuery,
            Result<AgendamentoUsuarioDetalheResponse>>(
            Result.Failure<AgendamentoUsuarioDetalheResponse>(
                "Agendamento não encontrado."));

        var controller = CriarController(mediator);

        var result = await controller.ObterPorIdAsync(
            Guid.NewGuid(),
            Guid.NewGuid(),
            CancellationToken.None);

        var notFound = Assert.IsType<NotFoundObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse>(notFound.Value);

        Assert.False(response.Success);
        Assert.Equal(
            "Agendamento não encontrado.",
            response.Message);
    }

    [Fact]
    public async Task ObterPorIdAsync_Deve_Usar_Mensagem_Padrao_Quando_Error_For_Null()
    {
        var mediator = new FakeMediator();

        mediator.RegistrarResposta<
            GetAgendamentoByIdQuery,
            Result<AgendamentoUsuarioDetalheResponse>>(
            Result.Failure<AgendamentoUsuarioDetalheResponse>(
                (string)null!));

        var controller = CriarController(mediator);

        var result = await controller.ObterPorIdAsync(
            Guid.NewGuid(),
            Guid.NewGuid(),
            CancellationToken.None);

        var notFound = Assert.IsType<NotFoundObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse>(notFound.Value);

        Assert.False(response.Success);
        Assert.Equal(
            "Agendamento não encontrado.",
            response.Message);
    }

    [Fact]
    public async Task ObterPorIdAsync_Deve_Retornar_NotFound_Quando_Value_For_Null()
    {
        var mediator = new FakeMediator();

        mediator.RegistrarResposta<
            GetAgendamentoByIdQuery,
            Result<AgendamentoUsuarioDetalheResponse>>(
            Result.Success<AgendamentoUsuarioDetalheResponse>(null!));

        var controller = CriarController(mediator);

        var result = await controller.ObterPorIdAsync(
            Guid.NewGuid(),
            Guid.NewGuid(),
            CancellationToken.None);

        var notFound = Assert.IsType<NotFoundObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse>(notFound.Value);

        Assert.False(response.Success);
        Assert.Equal(
            "Agendamento não encontrado.",
            response.Message);
    }

    #endregion

    #region ListarDiasDisponiveisAsync

    [Fact]
    public async Task ListarDiasDisponiveisAsync_Deve_Retornar_Sucesso()
    {
        var mediator = new FakeMediator();

        mediator.RegistrarResposta<
            ListarDiasDisponiveisQuery,
            Result<IEnumerable<DiaDisponibilidadeResponse>>>(
            Result.Success<IEnumerable<DiaDisponibilidadeResponse>>(
                Array.Empty<DiaDisponibilidadeResponse>()));

        var controller = CriarController(mediator);

        var result = await controller.ListarDiasDisponiveisAsync(
            8,
            2026,
            CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var response =
            Assert.IsType<ApiResponse<IEnumerable<DiaDisponibilidadeResponse>>>(
                ok.Value);

        Assert.True(response.Success);
        Assert.NotNull(response.Data);
        Assert.Empty(response.Data!);
    }

    [Fact]
    public async Task ListarDiasDisponiveisAsync_Deve_Retornar_BadRequest_Quando_Result_Falhar()
    {
        var mediator = new FakeMediator();

        mediator.RegistrarResposta<
            ListarDiasDisponiveisQuery,
            Result<IEnumerable<DiaDisponibilidadeResponse>>>(
            Result.Failure<IEnumerable<DiaDisponibilidadeResponse>>(
                "Não foi possível obter os dias."));

        var controller = CriarController(mediator);

        var result = await controller.ListarDiasDisponiveisAsync(
            8,
            2026,
            CancellationToken.None);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse>(badRequest.Value);

        Assert.False(response.Success);
        Assert.Equal(
            "Não foi possível obter os dias.",
            response.Message);
    }

    [Fact]
    public async Task ListarDiasDisponiveisAsync_Deve_Usar_Mensagem_Padrao_Quando_Error_For_Null()
    {
        var mediator = new FakeMediator();

        mediator.RegistrarResposta<
            ListarDiasDisponiveisQuery,
            Result<IEnumerable<DiaDisponibilidadeResponse>>>(
            Result.Failure<IEnumerable<DiaDisponibilidadeResponse>>(
                (string)null!));

        var controller = CriarController(mediator);

        var result = await controller.ListarDiasDisponiveisAsync(
            8,
            2026,
            CancellationToken.None);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse>(badRequest.Value);

        Assert.False(response.Success);
        Assert.Equal(
            "Não foi possível obter os dias disponíveis.",
            response.Message);
    }

    [Fact]
    public async Task ListarDiasDisponiveisAsync_Deve_Retornar_BadRequest_Quando_Value_For_Null()
    {
        var mediator = new FakeMediator();

        mediator.RegistrarResposta<
            ListarDiasDisponiveisQuery,
            Result<IEnumerable<DiaDisponibilidadeResponse>>>(
            Result.Success<IEnumerable<DiaDisponibilidadeResponse>>(null!));

        var controller = CriarController(mediator);

        var result = await controller.ListarDiasDisponiveisAsync(
            8,
            2026,
            CancellationToken.None);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse>(badRequest.Value);

        Assert.False(response.Success);
        Assert.Equal(
            "Não foi possível obter os dias disponíveis.",
            response.Message);
    }

    #endregion

    #region ListarHorariosPorDiaAsync

    [Fact]
    public async Task ListarHorariosPorDiaAsync_Deve_Retornar_Sucesso()
    {
        var mediator = new FakeMediator();

        mediator.RegistrarResposta<
            ListarHorariosPorDiaQuery,
            Result<IEnumerable<HorarioResponse>>>(
            Result.Success<IEnumerable<HorarioResponse>>(
                Array.Empty<HorarioResponse>()));

        var controller = CriarController(mediator);

        var result = await controller.ListarHorariosPorDiaAsync(
            Guid.NewGuid(),
            CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var response =
            Assert.IsType<ApiResponse<IEnumerable<HorarioResponse>>>(ok.Value);

        Assert.True(response.Success);
        Assert.NotNull(response.Data);
        Assert.Empty(response.Data!);
    }

    [Fact]
    public async Task ListarHorariosPorDiaAsync_Deve_Retornar_BadRequest_Quando_Result_Falhar()
    {
        var mediator = new FakeMediator();

        mediator.RegistrarResposta<
            ListarHorariosPorDiaQuery,
            Result<IEnumerable<HorarioResponse>>>(
            Result.Failure<IEnumerable<HorarioResponse>>(
                "Horários não encontrados."));

        var controller = CriarController(mediator);

        var result = await controller.ListarHorariosPorDiaAsync(
            Guid.NewGuid(),
            CancellationToken.None);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse>(badRequest.Value);

        Assert.False(response.Success);
        Assert.Equal(
            "Horários não encontrados.",
            response.Message);
    }

    [Fact]
    public async Task ListarHorariosPorDiaAsync_Deve_Usar_Mensagem_Padrao_Quando_Error_For_Null()
    {
        var mediator = new FakeMediator();

        mediator.RegistrarResposta<
            ListarHorariosPorDiaQuery,
            Result<IEnumerable<HorarioResponse>>>(
            Result.Failure<IEnumerable<HorarioResponse>>(
                (string)null!));

        var controller = CriarController(mediator);

        var result = await controller.ListarHorariosPorDiaAsync(
            Guid.NewGuid(),
            CancellationToken.None);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse>(badRequest.Value);

        Assert.False(response.Success);
        Assert.Equal(
            "Não foi possível obter os horários disponíveis.",
            response.Message);
    }

    [Fact]
    public async Task ListarHorariosPorDiaAsync_Deve_Retornar_BadRequest_Quando_Value_For_Null()
    {
        var mediator = new FakeMediator();

        mediator.RegistrarResposta<
            ListarHorariosPorDiaQuery,
            Result<IEnumerable<HorarioResponse>>>(
            Result.Success<IEnumerable<HorarioResponse>>(null!));

        var controller = CriarController(mediator);

        var result = await controller.ListarHorariosPorDiaAsync(
            Guid.NewGuid(),
            CancellationToken.None);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse>(badRequest.Value);

        Assert.False(response.Success);
        Assert.Equal(
            "Não foi possível obter os horários disponíveis.",
            response.Message);
    }

    #endregion

    #region ListarConsultoresPorDiaHorarioAsync

    [Fact]
    public async Task ListarConsultoresPorDiaHorarioAsync_Deve_Retornar_Sucesso()
    {
        var mediator = new FakeMediator();

        mediator.RegistrarResposta<
            ListarConsultoresPorDiaHorarioQuery,
            Result<IEnumerable<ConsultorDisponibilidadeResponse>>>(
            Result.Success<IEnumerable<ConsultorDisponibilidadeResponse>>(
                Array.Empty<ConsultorDisponibilidadeResponse>()));

        var controller = CriarController(mediator);

        var result = await controller.ListarConsultoresPorDiaHorarioAsync(
            Guid.NewGuid(),
            Guid.NewGuid(),
            CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var response =
            Assert.IsType<ApiResponse<IEnumerable<ConsultorDisponibilidadeResponse>>>(
                ok.Value);

        Assert.True(response.Success);
        Assert.NotNull(response.Data);
        Assert.Empty(response.Data!);
    }

    [Fact]
    public async Task ListarConsultoresPorDiaHorarioAsync_Deve_Retornar_BadRequest_Quando_Result_Falhar()
    {
        var mediator = new FakeMediator();

        mediator.RegistrarResposta<
            ListarConsultoresPorDiaHorarioQuery,
            Result<IEnumerable<ConsultorDisponibilidadeResponse>>>(
            Result.Failure<IEnumerable<ConsultorDisponibilidadeResponse>>(
                "Consultores não encontrados."));

        var controller = CriarController(mediator);

        var result = await controller.ListarConsultoresPorDiaHorarioAsync(
            Guid.NewGuid(),
            Guid.NewGuid(),
            CancellationToken.None);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse>(badRequest.Value);

        Assert.False(response.Success);
        Assert.Equal(
            "Consultores não encontrados.",
            response.Message);
    }

    [Fact]
    public async Task ListarConsultoresPorDiaHorarioAsync_Deve_Usar_Mensagem_Padrao_Quando_Error_For_Null()
    {
        var mediator = new FakeMediator();

        mediator.RegistrarResposta<
            ListarConsultoresPorDiaHorarioQuery,
            Result<IEnumerable<ConsultorDisponibilidadeResponse>>>(
            Result.Failure<IEnumerable<ConsultorDisponibilidadeResponse>>(
                (string)null!));

        var controller = CriarController(mediator);

        var result = await controller.ListarConsultoresPorDiaHorarioAsync(
            Guid.NewGuid(),
            Guid.NewGuid(),
            CancellationToken.None);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse>(badRequest.Value);

        Assert.False(response.Success);
        Assert.Equal(
            "Não foi possível obter os consultores disponíveis.",
            response.Message);
    }

    [Fact]
    public async Task ListarConsultoresPorDiaHorarioAsync_Deve_Retornar_BadRequest_Quando_Value_For_Null()
    {
        var mediator = new FakeMediator();

        mediator.RegistrarResposta<
            ListarConsultoresPorDiaHorarioQuery,
            Result<IEnumerable<ConsultorDisponibilidadeResponse>>>(
            Result.Success<IEnumerable<ConsultorDisponibilidadeResponse>>(null!));

        var controller = CriarController(mediator);

        var result = await controller.ListarConsultoresPorDiaHorarioAsync(
            Guid.NewGuid(),
            Guid.NewGuid(),
            CancellationToken.None);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse>(badRequest.Value);

        Assert.False(response.Success);
        Assert.Equal(
            "Não foi possível obter os consultores disponíveis.",
            response.Message);
    }

    #endregion

    #region ListarAgendaPorConsultorAsync

    [Fact]
    public async Task ListarAgendaPorConsultorAsync_Deve_Retornar_Sucesso()
    {
        var mediator = new FakeMediator();

        mediator.RegistrarResposta<
            ListarAgendaPorConsultorQuery,
            Result<IEnumerable<AgendaConsultorResponse>>>(
            Result.Success<IEnumerable<AgendaConsultorResponse>>(
                Array.Empty<AgendaConsultorResponse>()));

        var controller = CriarController(mediator);

        var result = await controller.ListarAgendaPorConsultorAsync(
            Guid.NewGuid(),
            new DateOnly(2026, 8, 16),
            CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var response =
            Assert.IsType<ApiResponse<IEnumerable<AgendaConsultorResponse>>>(
                ok.Value);

        Assert.True(response.Success);
        Assert.NotNull(response.Data);
        Assert.Empty(response.Data!);
    }

    [Fact]
    public async Task ListarAgendaPorConsultorAsync_Deve_Retornar_BadRequest_Quando_Result_Falhar()
    {
        var mediator = new FakeMediator();

        mediator.RegistrarResposta<
            ListarAgendaPorConsultorQuery,
            Result<IEnumerable<AgendaConsultorResponse>>>(
            Result.Failure<IEnumerable<AgendaConsultorResponse>>(
                "Agenda não encontrada."));

        var controller = CriarController(mediator);

        var result = await controller.ListarAgendaPorConsultorAsync(
            Guid.NewGuid(),
            new DateOnly(2026, 8, 16),
            CancellationToken.None);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse>(badRequest.Value);

        Assert.False(response.Success);
        Assert.Equal(
            "Agenda não encontrada.",
            response.Message);
    }

    [Fact]
    public async Task ListarAgendaPorConsultorAsync_Deve_Usar_Mensagem_Padrao_Quando_Error_For_Null()
    {
        var mediator = new FakeMediator();

        mediator.RegistrarResposta<
            ListarAgendaPorConsultorQuery,
            Result<IEnumerable<AgendaConsultorResponse>>>(
            Result.Failure<IEnumerable<AgendaConsultorResponse>>(
                (string)null!));

        var controller = CriarController(mediator);

        var result = await controller.ListarAgendaPorConsultorAsync(
            Guid.NewGuid(),
            new DateOnly(2026, 8, 16),
            CancellationToken.None);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse>(badRequest.Value);

        Assert.False(response.Success);
        Assert.Equal(
            "Não foi possível obter a agenda do consultor.",
            response.Message);
    }

    [Fact]
    public async Task ListarAgendaPorConsultorAsync_Deve_Retornar_BadRequest_Quando_Value_For_Null()
    {
        var mediator = new FakeMediator();

        mediator.RegistrarResposta<
            ListarAgendaPorConsultorQuery,
            Result<IEnumerable<AgendaConsultorResponse>>>(
            Result.Success<IEnumerable<AgendaConsultorResponse>>(null!));

        var controller = CriarController(mediator);

        var result = await controller.ListarAgendaPorConsultorAsync(
            Guid.NewGuid(),
            new DateOnly(2026, 8, 16),
            CancellationToken.None);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse>(badRequest.Value);

        Assert.False(response.Success);
        Assert.Equal(
            "Não foi possível obter a agenda do consultor.",
            response.Message);
    }

    #endregion

    #region CadastrarHorarioParaAgendamento

    [Fact]
    public async Task CadastrarHorarioParaAgendamento_Deve_Retornar_Sucesso()
    {
        var mediator = new FakeMediator();

        mediator.RegistrarResposta<
            CadastraHorarioAgendamentoCommand,
            Result>(
            Result.Success());

        var controller = CriarController(mediator);

        var result = await controller.CadastrarHorarioParaAgendamento(
            new TimeOnly(8, 0));

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse>(ok.Value);

        Assert.True(response.Success);
        Assert.Equal(
            "Horário cadastrado com sucesso.",
            response.Message);
    }

    [Fact]
    public async Task CadastrarHorarioParaAgendamento_Deve_Retornar_BadRequest_Quando_Result_Falhar()
    {
        var mediator = new FakeMediator();

        mediator.RegistrarResposta<
            CadastraHorarioAgendamentoCommand,
            Result>(
            Result.Failure("Horário já cadastrado."));

        var controller = CriarController(mediator);

        var result = await controller.CadastrarHorarioParaAgendamento(
            new TimeOnly(8, 0));

        var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse>(badRequest.Value);

        Assert.False(response.Success);
        Assert.Equal(
            "Horário já cadastrado.",
            response.Message);
    }

    [Fact]
    public async Task CadastrarHorarioParaAgendamento_Deve_Usar_Mensagem_Padrao_Quando_Error_For_Null()
    {
        var mediator = new FakeMediator();

        mediator.RegistrarResposta<
            CadastraHorarioAgendamentoCommand,
            Result>(
            Result.Failure((string)null!));

        var controller = CriarController(mediator);

        var result = await controller.CadastrarHorarioParaAgendamento(
            new TimeOnly(8, 0));

        var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse>(badRequest.Value);

        Assert.False(response.Success);
        Assert.Equal(
            "Não foi possível cadastrar os horários disponíveis.",
            response.Message);
    }

    #endregion

    #region CriarAsync

    [Fact]
    public async Task CriarAsync_Deve_Rejeitar_Requisicao_Invalida()
    {
        var controller = CriarController(new FakeMediator());

        var result = await controller.CriarAsync(
            new CreateAgendamentoRequest(),
            CancellationToken.None);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse>(badRequest.Value);

        Assert.False(response.Success);
    }

    [Fact]
    public async Task CriarAsync_Deve_Retornar_Sucesso()
    {
        var mediator = new FakeMediator();

        mediator.RegistrarResposta<CreateAgendamentoCommand, Result>(
            Result.Success());

        var controller = CriarController(mediator);

        var result = await controller.CriarAsync(
            CriarRequestValido(),
            CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse>(ok.Value);

        Assert.True(response.Success);
        Assert.Equal(
            "Agendamento criado com sucesso.",
            response.Message);
    }

    [Fact]
    public async Task CriarAsync_Deve_Retornar_BadRequest_Quando_Mediator_Falhar()
    {
        var mediator = new FakeMediator();

        mediator.RegistrarResposta<CreateAgendamentoCommand, Result>(
            Result.Failure("Não foi possível criar o agendamento."));

        var controller = CriarController(mediator);

        var result = await controller.CriarAsync(
            CriarRequestValido(),
            CancellationToken.None);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse>(badRequest.Value);

        Assert.False(response.Success);
        Assert.Equal(
            "Não foi possível criar o agendamento.",
            response.Message);
    }

    [Fact]
    public async Task CriarAsync_Deve_Usar_Mensagem_Padrao_Quando_Error_For_Null()
    {
        var mediator = new FakeMediator();

        mediator.RegistrarResposta<CreateAgendamentoCommand, Result>(
            Result.Failure((string)null!));

        var controller = CriarController(mediator);

        var result = await controller.CriarAsync(
            CriarRequestValido(),
            CancellationToken.None);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse>(badRequest.Value);

        Assert.False(response.Success);
        Assert.Equal(
            "Falha ao criar agendamento.",
            response.Message);
    }

    #endregion

    #region CancelarAgendamentoAsync

    [Fact]
    public async Task CancelarAgendamentoAsync_Deve_Retornar_Sucesso()
    {
        var mediator = new FakeMediator();

        mediator.RegistrarResposta<CancelarAgendamentoCommand, Result>(
            Result.Success());

        var controller = CriarController(mediator);

        var result = await controller.CancelarAgendamentoAsync(
            new CancelarAgendamentoRequest
            {
                AgendamentoId = Guid.NewGuid()
            },
            CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse>(ok.Value);

        Assert.True(response.Success);
        Assert.Equal(
            "Agendamento cancelado com sucesso.",
            response.Message);
    }

    [Fact]
    public async Task CancelarAgendamentoAsync_Deve_Retornar_BadRequest_Quando_Result_Falhar()
    {
        var mediator = new FakeMediator();

        mediator.RegistrarResposta<CancelarAgendamentoCommand, Result>(
            Result.Failure("Agendamento não pode ser cancelado."));

        var controller = CriarController(mediator);

        var result = await controller.CancelarAgendamentoAsync(
            new CancelarAgendamentoRequest
            {
                AgendamentoId = Guid.NewGuid()
            },
            CancellationToken.None);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse>(badRequest.Value);

        Assert.False(response.Success);
        Assert.Equal(
            "Agendamento não pode ser cancelado.",
            response.Message);
    }

    [Fact]
    public async Task CancelarAgendamentoAsync_Deve_Usar_Mensagem_Padrao_Quando_Error_For_Null()
    {
        var mediator = new FakeMediator();

        mediator.RegistrarResposta<CancelarAgendamentoCommand, Result>(
            Result.Failure((string)null!));

        var controller = CriarController(mediator);

        var result = await controller.CancelarAgendamentoAsync(
            new CancelarAgendamentoRequest
            {
                AgendamentoId = Guid.NewGuid()
            },
            CancellationToken.None);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse>(badRequest.Value);

        Assert.False(response.Success);
        Assert.Equal(
            "Não foi possível cancelar o agendamento.",
            response.Message);
    }

    #endregion

    #region IniciarAsync

    [Fact]
    public async Task IniciarAsync_Deve_Retornar_Sucesso()
    {
        var mediator = new FakeMediator();

        mediator.RegistrarResposta<IniciarAgendamentoCommand, Result>(
            Result.Success());

        var controller = CriarController(mediator);

        var result = await controller.IniciarAsync(
            Guid.NewGuid(),
            CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse>(ok.Value);

        Assert.True(response.Success);
        Assert.Equal(
            "Agendamento iniciado com sucesso.",
            response.Message);
    }

    [Fact]
    public async Task IniciarAsync_Deve_Retornar_BadRequest_Quando_Result_Falhar()
    {
        var mediator = new FakeMediator();

        mediator.RegistrarResposta<IniciarAgendamentoCommand, Result>(
            Result.Failure("Agendamento não pode ser iniciado."));

        var controller = CriarController(mediator);

        var result = await controller.IniciarAsync(
            Guid.NewGuid(),
            CancellationToken.None);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse>(badRequest.Value);

        Assert.False(response.Success);
        Assert.Equal(
            "Agendamento não pode ser iniciado.",
            response.Message);
    }

    [Fact]
    public async Task IniciarAsync_Deve_Usar_Mensagem_Padrao_Quando_Error_For_Null()
    {
        var mediator = new FakeMediator();

        mediator.RegistrarResposta<IniciarAgendamentoCommand, Result>(
            Result.Failure((string)null!));

        var controller = CriarController(mediator);

        var result = await controller.IniciarAsync(
            Guid.NewGuid(),
            CancellationToken.None);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse>(badRequest.Value);

        Assert.False(response.Success);
        Assert.Equal(
            "Não foi possível iniciar o agendamento.",
            response.Message);
    }

    #endregion

    #region Helpers

    private static CreateAgendamentoRequest CriarRequestValido()
        => new()
        {
            PessoaId = Guid.NewGuid(),
            AgendaConsultorId = Guid.NewGuid(),
            VeiculoId = Guid.NewGuid(),
            Hodometro = 1000,
            Descricao = "Troca de óleo"
        };

    private static AgendamentoController CriarController(FakeMediator mediator)
        => new(
            new CreateAgendamentoRequestValidator(),
            mediator,
            NullLogger<AgendamentoController>.Instance)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            }
        };

    #endregion
}