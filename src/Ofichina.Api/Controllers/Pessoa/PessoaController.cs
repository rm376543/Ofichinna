using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ofichina.Application.UseCases.Pessoas.Commands;
using Ofichina.Application.UseCases.Pessoas.Queries;
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
        /// <param name="cancellationToken">Token de cancelamento.</param>
        /// <returns>Lista de pessoas.</returns>
        //[Authorize(Policy = UserPolicyEnum.Ler)]
        [Authorize(Roles = "ADMIN")]
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IReadOnlyCollection<PessoaResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<ApiResponse<IReadOnlyCollection<PessoaResponse>>>> BuscarPessoas(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Iniciando a obtenção de todas as pessoas.");

            var result = await _mediator.Send(new GetPessoasQuery(), cancellationToken);

            if (!result.IsSuccess)
            {
                _logger.LogError("Erro ao obter as pessoas: {Erro}", result.Error);
                return BadRequest(ApiResponse.FailureResponse(result.Error ?? "Não foi possível obter as pessoas."));
            }

            return Ok(ApiResponse<IReadOnlyCollection<PessoaResponse>>.SuccessResponse(result.Value ?? []));
        }

        /// <summary>
        /// Retorna uma pessoa pelo identificador.
        /// </summary>
        /// <param name="id">Identificador da pessoa.</param>
        /// <param name="cancellationToken">Token de cancelamento.</param>
        /// <returns>Pessoa encontrada ou erro 404.</returns>
        [Authorize(Roles = "ADMIN")]
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<PessoaResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<PessoaResponse>>> BuscarPessoaPorId(Guid id, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Iniciando a obtenção da pessoa com Id: {Id}", id);

            var result = await _mediator.Send(new GetPessoaByIdQuery(id), cancellationToken);

            if (!result.IsSuccess || result.Value is null)
            {
                _logger.LogError("Pessoa com Id: {Id} não encontrada.", id);
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
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<ApiResponse<Guid>>> CriarPessoa([FromBody] CreatePessoaRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Iniciando a criação de uma nova pessoa. Nome: {Nome}", request.Nome);

            var validation = await _createValidator.ValidateAsync(request, cancellationToken);

            if (!validation.IsValid)
            {
                _logger.LogError("Erro ao validar a criação da pessoa. Erros: {Erros}", string.Join(", ", validation.Errors.Select(x => x.ErrorMessage)));
                return BadRequest(ApiResponse.FailureResponse(validation.Errors.Select(x => x.ErrorMessage)));
            }

            var result = await _mediator.Send(new CreatePessoaCommand
            {
                Nome = request.Nome,
                Documento = request.Documento,
                Telefone = request.Telefone,
                Logradouro = request.Logradouro,
                Numero = request.Numero,
                Complemento = request.Complemento,
                Bairro = request.Bairro,
                Cidade = request.Cidade,
                Estado = request.Estado,
                Cep = request.Cep,
                UsuarioId = request.UsuarioId
            }, cancellationToken);

            if (!result.IsSuccess)
            {
                _logger.LogError("Erro ao criar a pessoa. Erro: {Erro}", result.Error);
                return BadRequest(ApiResponse.FailureResponse(result.Error ?? "Não foi possível criar a pessoa."));
            }

            return StatusCode(StatusCodes.Status201Created, ApiResponse<Guid>.SuccessResponse(result.Value, "Pessoa criada com sucesso."));
        }

        /// <summary>
        /// Atualiza uma pessoa existente.
        /// </summary>
        /// <param name="request">Dados atualizados da pessoa.</param>
        /// <param name="cancellationToken">Token de cancelamento.</param>
        /// <returns>Mensagem de sucesso, erro de validação ou pessoa não encontrada.</returns>
        [Authorize(Roles = "ADMIN")]
        [HttpPut]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse>> AtualizarPessoa([FromBody] UpdatePessoaRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Iniciando a atualização da pessoa com Id: {Id}", request.Id);

            var validation = await _updateValidator.ValidateAsync(request, cancellationToken);

            if (!validation.IsValid)
            {
                _logger.LogError("Erro ao validar a atualização da pessoa com Id: {Id}. Erros: {Erros}", request.Id, string.Join(", ", validation.Errors.Select(x => x.ErrorMessage)));
                return BadRequest(ApiResponse.FailureResponse(validation.Errors.Select(x => x.ErrorMessage)));
            }

            var result = await _mediator.Send(new UpdatePessoaCommand
            {
                Id = request.Id,
                Nome = request.Nome,
                Telefone = request.Telefone,
                Logradouro = request.Logradouro,
                Numero = request.Numero,
                Complemento = request.Complemento,
                Bairro = request.Bairro,
                Cidade = request.Cidade,
                Estado = request.Estado,
                Cep = request.Cep
            }, cancellationToken);

            if (!result.IsSuccess)
            {
                _logger.LogError("Erro ao atualizar a pessoa com Id: {Id}. Erro: {Erro}", request.Id, result.Error);
                return result.Error == "Pessoa não encontrada."
                    ? NotFound(ApiResponse.FailureResponse(result.Error))
                    : BadRequest(ApiResponse.FailureResponse(result.Error ?? "Não foi possível atualizar a pessoa."));
            }

            return Ok(ApiResponse.SuccessResponse("Pessoa atualizada com sucesso."));
        }

        /// <summary>
        /// Desativa uma pessoa existente.
        /// </summary>
        /// <param name="id">Identificador da pessoa.</param>
        /// <param name="cancellationToken">Token de cancelamento.</param>
        /// <returns>Mensagem de sucesso ou erro 404.</returns>
        [Authorize(Roles = "ADMIN")]
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse>> DeletarPessoa(Guid id, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Iniciando a desativação da pessoa com Id: {Id}", id);

            var result = await _mediator.Send(new DeletePessoaCommand { Id = id }, cancellationToken);

            if (!result.IsSuccess)
            {
                _logger.LogError("Erro ao desativar a pessoa com Id: {Id}. Erro: {Erro}", id, result.Error);
                return NotFound(ApiResponse.FailureResponse(result.Error ?? "Pessoa não encontrada."));
            }

            return Ok(ApiResponse.SuccessResponse("Pessoa desativada com sucesso."));
        }
    }
}



