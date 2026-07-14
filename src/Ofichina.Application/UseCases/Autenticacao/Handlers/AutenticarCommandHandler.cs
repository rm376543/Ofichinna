using Ofichina.Application.Abstractions;
using Ofichina.Authentication.Abstractions;
using Ofichina.Contracts.Responses;
using Ofichina.Domain.ValueObjects;
using Ofichina.Application.UseCases.Autenticacao.Commands;
using Ofichina.Contracts.Common;
using Ofichina.Domain.Exceptions;
using Ofichina.Contracts.Enums;
using Microsoft.Extensions.Logging;

namespace Ofichina.Application.UseCases.Autenticacao.Handlers;

public sealed class AutenticarCommandHandler : ICommandHandler<AutenticarCommand, Result<AutenticacaoResponse>>
{
    private readonly IUsuarioAutenticacaoRepository _usuarioAutenticacaoRepository;
    private readonly IPerfilAutorizacaoService _perfilService;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly ISenhaHasher _senhaHasher;
    private readonly ILogger<AutenticarCommandHandler> _logger;


    public AutenticarCommandHandler(
        IUsuarioAutenticacaoRepository usuarioAutenticacaoRepository,
        IPerfilAutorizacaoService perfilService,
        IJwtTokenService jwtTokenService,
        ISenhaHasher senhaHasher,
        ILogger<AutenticarCommandHandler> logger)
    {
        _usuarioAutenticacaoRepository = usuarioAutenticacaoRepository;
        _perfilService = perfilService;
        _jwtTokenService = jwtTokenService;
        _senhaHasher = senhaHasher;
        _logger = logger;
    }

    public async Task<Result<AutenticacaoResponse>> HandleAsync(AutenticarCommand command)
    {
        try
        {
            _logger.LogInformation("Iniciando autenticação para o email: {Email}", command.Email);

            Email email = new Email(command.Email);

            var usuario = await _usuarioAutenticacaoRepository.ObterPorEmailAsync(email.Value);

            if (usuario is null || !usuario.EstaAtivo())
            {
                _logger.LogWarning("Usuário não encontrado ou está inativo. Email: {Email}", command.Email);
                return Result.Failure<AutenticacaoResponse>(ApplicationErrors.VerifiqueDados);
            }

            if (!_senhaHasher.Verificar(command.Senha, usuario.SenhaHash))
            {
                _logger.LogWarning("Credenciais inválidas para o email: {Email}", command.Email);
                return Result.Failure<AutenticacaoResponse>(ApplicationErrors.CredenciaisInvalidas);
            }

            var perfis = await _perfilService.ObterPerfisAsync(usuario.Id);
            var token = await _jwtTokenService.GerarTokenAsync(usuario, perfis);

            _logger.LogInformation("Autenticação bem-sucedida para o email: {Email}", command.Email);

            return Result.Success(new AutenticacaoResponse
            {
                UsuarioId = usuario.Id,
                Email = usuario.Email.Value,
                Perfis = perfis,
                AccessToken = token.AccessToken,
                ExpiraEm = token.ExpiraEm
            });

        }
        catch (DomainException ex)
        {
            _logger.LogError(ex, "Erro de domínio durante a autenticação para o email: {Email}", command.Email);
            return Result.Failure<AutenticacaoResponse>(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro desconhecido durante a autenticação para o email: {Email}", command.Email);
            return Result.Failure<AutenticacaoResponse>($"{ApplicationErrors.ErroDesconhecido} - {ex.Message}");
        }
    }
}