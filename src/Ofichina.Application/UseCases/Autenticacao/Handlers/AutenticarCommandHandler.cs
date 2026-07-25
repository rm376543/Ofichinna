using Microsoft.Extensions.Logging;
using Ofichina.Application.Abstractions;
using Ofichina.Application.Abstractions.Authentication;
using Ofichina.Application.UseCases.Autenticacao.Commands;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Responses.Authentication;
using Ofichina.Domain.Exceptions;
using Ofichina.Domain.ValueObjects;

namespace Ofichina.Application.UseCases.Autenticacao.Handlers;

public sealed class AutenticarCommandHandler : ICommandHandler<AutenticarCommand, Result<AuthenticationResponse>>
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

    public async Task<Result<AuthenticationResponse>> HandleAsync(AutenticarCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Iniciando autenticação para o email: {Email}", command.Email);

            Email email = new(command.Email);

            var usuario = await _usuarioAutenticacaoRepository.ObterPorEmailAsync(email.Value, cancellationToken);

            if (usuario is null || !usuario.EstaAtivo())
            {
                _logger.LogWarning("Usuário não encontrado ou está inativo. Email: {Email}", command.Email);
                return Result.Failure<AuthenticationResponse>("Verifique os dados fornecidos.");
            }

            if (!_senhaHasher.Verificar(command.Senha, usuario.SenhaHash))
            {
                _logger.LogWarning("Credenciais inválidas para o email: {Email}", command.Email);
                return Result.Failure<AuthenticationResponse>("Credenciais inválidas.");
            }

            var perfis = await _perfilService.ObterPerfisAsync(usuario.Id, cancellationToken);
            var permissoes = await _perfilService.ObterPermissoesAsync(usuario.Id, cancellationToken);
            var token = await _jwtTokenService.GerarTokenAsync(usuario, perfis, cancellationToken);

            _logger.LogInformation("Autenticação bem-sucedida para o email: {Email}", command.Email);

            return Result.Success(new AuthenticationResponse
            {
                UsuarioId = usuario.Id,
                Email = usuario.Email.Value,
                Perfis = perfis,
                Permissoes = permissoes,
                AccessToken = token.AccessToken,
                ExpiraEm = token.ExpiraEm
            });
        }
        catch (DomainException ex)
        {
            _logger.LogError(ex, "Erro de domínio durante a autenticação para o email: {Email}", command.Email);
            return Result.Failure<AuthenticationResponse>(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro desconhecido durante a autenticação para o email: {Email}", command.Email);
            return Result.Failure<AuthenticationResponse>($"Erro Desconhecido - {ex.Message}");
        }
    }
}
