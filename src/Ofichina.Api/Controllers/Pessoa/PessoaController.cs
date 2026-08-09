using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ofichina.Application.UseCases.Pessoas.Commands;
using Ofichina.Application.UseCases.Pessoas.Queries;
using Ofichina.Contracts;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Requests.Pessoa;
using Ofichina.Contracts.Responses;
using Ofichina.Contracts.Responses.Pessoa;

namespace Ofichina.Api.Controllers.Pessoa
{
    /// <summary>
    /// Controller responsável pelo CRUD de pessoas.
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("api/pessoa")]
#pragma warning disable S6960
    public sealed class PessoaController : ControllerBase
#pragma warning restore S6960
    {
        private readonly IValidator<CreatePessoaRequest> _createValidator;
        private readonly IValidator<UpdatePessoaRequest> _updateValidator;
        private readonly IMediator _mediator;
        private readonly ILogger<PessoaController> _logger;
#pragma warning disable S107
        public PessoaController(
            IValidator<CreatePessoaRequest> createValidator,
            IValidator<UpdatePessoaRequest> updateValidator,
            IMediator mediator,
            ILogger<PessoaController> logger)
#pragma warning restore S107
        {
            _createValidator = createValidator;
            _updateValidator = updateValidator;
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Retorna todas as pessoas cadastradas.
        /// </summary>
        /// <param name="pagination"></param>
        /// <param name="cancellationToken">Token de cancelamento.</param>
        /// <returns>Lista de pessoas.</returns>
        //[Authorize(Policy = UserPolicyEnum.Ler)]
        [Authorize(Roles = "ADMIN")]
        [HttpGet("listar")]
        [ProducesResponseType(typeof(ApiResponse<PagedResponse<PessoaResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<ApiResponse<PagedResponse<PessoaResponse>>>> BuscarPessoas(
            [FromQuery] Pagination pagination, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Iniciando a obtenção de todas as pessoas.");

            var result = await _mediator.Send(new GetAllPessoasPaginadasQuery(pagination), cancellationToken);

            if (!result.IsSuccess)
            {
                _logger.LogError("Erro ao obter as pessoas: {Erro}", result.Error);
                return BadRequest(ApiResponse.FailureResponse(result.Error ?? "Não foi possível obter as pessoas."));
            }

            return Ok(ApiResponse<PagedResponse<PessoaResponse>>.SuccessResponse(result.Value));
        }

        /// <summary>
        /// Retorna uma pessoa pelo identificador.
        /// </summary>
        /// <param name="pessoaId">Identificador da pessoa.</param>
        /// <param name="cancellationToken">Token de cancelamento.</param>
        /// <returns>Pessoa encontrada ou erro 404.</returns>
        [Authorize(Roles = "ADMIN")]
        [HttpGet("detalhar/{pessoaId:guid}")]
        [ProducesResponseType(typeof(ApiResponse<PessoaResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<PessoaResponse>>> BuscarPessoaPorId(Guid pessoaId, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Iniciando a obtenção da pessoa com Id: {pessoaId}", pessoaId);

            var result = await _mediator.Send(new GetPessoaByIdQuery(pessoaId), cancellationToken);

            if (!result.IsSuccess || result.Value is null)
            {
                _logger.LogError("Pessoa com Id: {pessoaId} não encontrada.", pessoaId);
                return NotFound(ApiResponse.FailureResponse(result.Error ?? "Pessoa não encontrada."));
            }

            return Ok(ApiResponse<PessoaResponse>.SuccessResponse(result.Value));
        }

        /// <summary>
        /// Cria uma nova pessoa.
        /// </summary>
        /// <param name="request">Dados da pessoa.</param>
        /// <param name="cancellationToken">Token de cancelamento.</param>
        /// <returns>Mensagem de sucesso ou erro de validação.</returns>
        //[Authorize(Policy = UserPolicyEnum.Escrever)]
        [Authorize(Roles = "ADMIN")]
        [HttpPost("adicionar")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<ApiResponse>> CriarPessoa([FromBody] CreatePessoaRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Iniciando a criação de uma nova pessoa. Nome: {Nome}", request.Nome);

            var validation = await _createValidator.ValidateAsync(request, cancellationToken);

            if (!validation.IsValid)
            {
                _logger.LogError("Erro ao validar a criação da pessoa. Erros: {Erros}", string.Join(", ", validation.Errors.Select(x => x.ErrorMessage)));
                return BadRequest(ApiResponse.FailureResponse(validation.Errors.Select(x => x.ErrorMessage)));
            }

            var result = await _mediator.Send(new CreatePessoaCommand(request), cancellationToken);

            if (!result.IsSuccess)
            {
                _logger.LogError("Erro ao criar a pessoa. Erro: {Erro}", result.Error);
                return BadRequest(ApiResponse.FailureResponse(result.Error ?? "Não foi possível criar a pessoa."));
            }

            return Ok(ApiResponse.SuccessResponse("Pessoa criada com sucesso."));
        }

        /// <summary>
        /// Atualiza uma pessoa existente.
        /// </summary>
        /// <param name="request">Dados atualizados da pessoa.</param>
        /// <param name="cancellationToken">Token de cancelamento.</param>
        /// <returns>Mensagem de sucesso, erro de validação ou pessoa não encontrada.</returns>
        [Authorize(Roles = "ADMIN")]
        [HttpPut("atualizar")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse>> AtualizarPessoa([FromBody] UpdatePessoaRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Iniciando a atualização da pessoa com Id: {Id}", request.PessoaId);

            var validation = await _updateValidator.ValidateAsync(request, cancellationToken);

            if (!validation.IsValid)
            {
                _logger.LogError("Erro ao validar a atualização da pessoa com Id: {Id}. Erros: {Erros}", request.PessoaId, string.Join(", ", validation.Errors.Select(x => x.ErrorMessage)));
                return BadRequest(ApiResponse.FailureResponse(validation.Errors.Select(x => x.ErrorMessage)));
            }

            var result = await _mediator.Send(new UpdatePessoaCommand(request), cancellationToken);

            if (!result.IsSuccess)
            {
                _logger.LogError("Erro ao atualizar a pessoa com Id: {Id}. Erro: {Erro}", request.PessoaId, result.Error);
                return BadRequest(ApiResponse.FailureResponse(result.Error ?? "Não foi possível atualizar a pessoa."));
            }

            return Ok(ApiResponse.SuccessResponse("Pessoa atualizada com sucesso."));
        }

        /// <summary>
        /// Desativa uma pessoa existente.
        /// </summary>
        /// <param name="request">Identificador da pessoa.</param>
        /// <param name="cancellationToken">Token de cancelamento.</param>
        /// <returns>Mensagem de sucesso ou erro 404.</returns>
        [Authorize(Roles = "ADMIN")]
        [HttpDelete("remover")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse>> DeletarPessoa([FromBody] RemovePessoaRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Iniciando a desativação da pessoa com Id: {Id}", request.PessoaId);

            var result = await _mediator.Send(new DeletePessoaCommand(request), cancellationToken);

            if (!result.IsSuccess)
            {
                _logger.LogError("Erro ao desativar a pessoa com Id: {Id}. Erro: {Erro}", request.PessoaId, result.Error);
                return NotFound(ApiResponse.FailureResponse(result.Error ?? "Pessoa não encontrada."));
            }

            return Ok(ApiResponse.SuccessResponse("Pessoa desativada com sucesso."));
        }
    }
}



