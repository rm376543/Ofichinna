using Microsoft.Extensions.Logging;
using Ofichina.Application.Abstractions;
using Ofichina.Application.Exceptions;
using Ofichina.Application.UseCases.Autenticacao.Commands;
using Ofichina.Authentication.Abstractions;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Enums;
using Ofichina.Contracts.Responses;
using Ofichina.Domain.Entities;
using Ofichina.Domain.Interfaces;
using Ofichina.Domain.ValueObjects;

namespace Ofichina.Application.UseCases.Autenticacao.Handlers;

public sealed class CadastrarClienteCommandHandler : ICommandHandler<CadastrarClienteCommand, Result<AutenticacaoResponse>>
{
    private readonly IRepository<Usuario> _usuarioRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUsuarioAutenticacaoRepository _usuarioAutenticacaoRepository;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly ISenhaHasher _senhaHasher;
    private readonly ILogger<CadastrarClienteCommandHandler> _logger;

    public CadastrarClienteCommandHandler(
        IRepository<Usuario> usuarioRepository,
        IUnitOfWork unitOfWork,
        IUsuarioAutenticacaoRepository usuarioAutenticacaoRepository,
        IJwtTokenService jwtTokenService,
        ISenhaHasher senhaHasher,
        ILogger<CadastrarClienteCommandHandler> logger)
    {
        _usuarioRepository = usuarioRepository;
        _unitOfWork = unitOfWork;
        _usuarioAutenticacaoRepository = usuarioAutenticacaoRepository;
        _jwtTokenService = jwtTokenService;
        _senhaHasher = senhaHasher;
        _logger = logger;
    }

    public async Task<Result<AutenticacaoResponse>> HandleAsync(CadastrarClienteCommand command)
    {
        try
        {
            _logger.LogInformation("Iniciando cadastro de cliente. Email: {Email}", command.Email);

            var email = Email.Criar(command.Email);

            var usuarioExistente = await _usuarioAutenticacaoRepository.ObterPorEmailAsync(email.Value);

            if (usuarioExistente is not null)
            {
                _logger.LogWarning("Já existe um usuário cadastrado com este e-mail. Email: {Email}", command.Email);
                return Result.Failure<AutenticacaoResponse>("Já existe um usuário cadastrado com este e-mail.");
            }

            var usuario = new Usuario(email, _senhaHasher.GerarHash(command.Senha));

            await _usuarioRepository.AddAsync(usuario);
            await _unitOfWork.SaveChangesAsync();

            var perfis = Array.Empty<string>();
            var token = await _jwtTokenService.GerarTokenAsync(usuario, perfis);

            _logger.LogInformation("Cadastro de cliente realizado com sucesso. UsuarioId: {UsuarioId}, Email: {Email}", usuario.Id, usuario.Email.Value);

            return Result.Success(new AutenticacaoResponse
            {
                UsuarioId = usuario.Id,
                Email = usuario.Email.Value,
                Perfis = perfis,
                AccessToken = token.AccessToken,
                ExpiraEm = token.ExpiraEm
            });
        }
        catch(BusinessException ex)
        {
            _logger.LogError(ex, "Erro de negócio ao cadastrar cliente. Email: {Email}, Erro: {Erro}", command.Email, ex.Message);
            return Result.Failure<AutenticacaoResponse>(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro desconhecido ao cadastrar cliente. Email: {Email}, Erro: {Erro}", command.Email, ex.Message);
            return Result.Failure<AutenticacaoResponse>($"{ApplicationErrors.ErroDesconhecido} - {ex.Message}");
        }
        
    }
}