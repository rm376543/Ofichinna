using Microsoft.Extensions.Logging;
using Ofichina.Application.Abstractions;
using Ofichina.Application.Exceptions;
using Ofichina.Application.Abstractions.Authentication;
using Ofichina.Application.UseCases.Autenticacao.Commands;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Enums;
using Ofichina.Domain.Entities;
using Ofichina.Domain.ValueObjects;
using Ofichina.Domain.Common;
using Ofichina.Contracts.Responses.Authentication;

namespace Ofichina.Application.UseCases.Autenticacao.Handlers;

public sealed class CadastrarUsuarioCommandHandler : ICommandHandler<CadastrarUsuarioCommand, Result<AuthenticationResponse>>
{
    private readonly IRepository<Usuario> _usuarioRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUsuarioAutenticacaoRepository _usuarioAutenticacaoRepository;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly ISenhaHasher _senhaHasher;
    private readonly ILogger<CadastrarUsuarioCommandHandler> _logger;

    public CadastrarUsuarioCommandHandler(
        IRepository<Usuario> usuarioRepository,
        IUnitOfWork unitOfWork,
        IUsuarioAutenticacaoRepository usuarioAutenticacaoRepository,
        IJwtTokenService jwtTokenService,
        ISenhaHasher senhaHasher,
        ILogger<CadastrarUsuarioCommandHandler> logger)
    {
        _usuarioRepository = usuarioRepository;
        _unitOfWork = unitOfWork;
        _usuarioAutenticacaoRepository = usuarioAutenticacaoRepository;
        _jwtTokenService = jwtTokenService;
        _senhaHasher = senhaHasher;
        _logger = logger;
    }

    public async Task<Result<AuthenticationResponse>> HandleAsync(CadastrarUsuarioCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Iniciando cadastro de usuário. Email: {Email}", command.Email);

            Email email = new Email(command.Email);

            var usuarioExistente = await _usuarioAutenticacaoRepository.ObterPorEmailAsync(email.Value);

            if (usuarioExistente is not null)
            {
                _logger.LogWarning("Já existe um usuário cadastrado com este e-mail. Email: {Email}", command.Email);
                return Result.Failure<AuthenticationResponse>("Já existe um usuário cadastrado com este e-mail.");
            }

            var usuario = new Usuario(email, _senhaHasher.GerarHash(command.Senha));

            await _usuarioRepository.AddAsync(usuario, cancellationToken);
            await _unitOfWork.SaveChangesAsync();

            var perfis = Array.Empty<string>();
            var token = await _jwtTokenService.GerarTokenAsync(usuario, perfis);

            _logger.LogInformation("Cadastro de usuário realizado com sucesso. UsuarioId: {UsuarioId}, Email: {Email}", usuario.Id, usuario.Email.Value);

            return Result.Success(new AuthenticationResponse
            {
                UsuarioId = usuario.Id,
                Email = usuario.Email.Value,
                Perfis = perfis,
                Permissoes = [],
                AccessToken = token.AccessToken,
                ExpiraEm = token.ExpiraEm
            });
        }
        catch (BusinessException ex)
        {
            _logger.LogError(ex, "Erro de negócio ao cadastrar usuário. Email: {Email}, Erro: {Erro}", command.Email, ex.Message);
            return Result.Failure<AuthenticationResponse>(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro desconhecido ao cadastrar usuário. Email: {Email}, Erro: {Erro}", command.Email, ex.Message);
            return Result.Failure<AuthenticationResponse>($"{ApplicationErrors.ErroDesconhecido} - {ex.Message}");
        }
    }
}
