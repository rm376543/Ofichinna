using Ofichina.Application.Abstractions.Authentication.Repository;
using Ofichina.Application.Abstractions.Authentication.Service;
using Ofichina.Application.Abstractions.Common;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Requests.Autenticacao;
using Ofichina.Contracts.Requests.Usuario;
using Ofichina.Contracts.Responses.Authentication;
using Ofichina.Domain.Entities;
using Ofichina.Domain.ValueObjects;

namespace Ofichina.Authentication.Services;

public sealed class AutenticacaoService : IAuthService
{
    private readonly IRepository<Usuario> _usuarioRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserAuthRepository _usuarioAutenticacaoRepository;
    private readonly IProfileAuthService _perfilService;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IPasswordHasherService _senhaHasher;

    public AutenticacaoService(
        IRepository<Usuario> usuarioRepository,
        IUnitOfWork unitOfWork,
        IUserAuthRepository usuarioAutenticacaoRepository,
        IProfileAuthService perfilService,
        IJwtTokenService jwtTokenService,
        IPasswordHasherService senhaHasher)
    {
        _usuarioRepository = usuarioRepository;
        _unitOfWork = unitOfWork;
        _usuarioAutenticacaoRepository = usuarioAutenticacaoRepository;
        _perfilService = perfilService;
        _jwtTokenService = jwtTokenService;
        _senhaHasher = senhaHasher;
    }

    public async Task<Result<AuthenticationResponse>> AutenticarAsync(
        AutenticacaoRequest request,
        CancellationToken cancellationToken = default)
    {
        Email email = new(request.Email);

        var usuario = await _usuarioAutenticacaoRepository.ObterPorEmailAsync(
            email.Value,
            cancellationToken);

        if (usuario is null || !usuario.EstaAtivo())
        {
            return Result.Failure<AuthenticationResponse>("Credenciais inválidas.");
        }

        if (!_senhaHasher.Verificar(request.Senha, usuario.SenhaHash))
        {
            return Result.Failure<AuthenticationResponse>("Credenciais inválidas.");
        }

        var perfis = await _perfilService.ObterPerfisAsync(usuario.Id, cancellationToken);
        var permissoes = await _perfilService.ObterPermissoesAsync(usuario.Id, cancellationToken);
        var token = await _jwtTokenService.GerarTokenAsync(usuario, perfis, cancellationToken);

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

    public async Task<Result<AuthenticationResponse>> CadastrarAsync(
        CadastrarUsuarioRequest request,
        CancellationToken cancellationToken = default)
    {
        Email email = new(request.Email);

        var usuarioExistente = await _usuarioAutenticacaoRepository.ObterPorEmailAsync(
            email.Value,
            cancellationToken);

        if (usuarioExistente is not null)
        {
            return Result.Failure<AuthenticationResponse>("Já existe um usuário cadastrado com este e-mail.");
        }

        var usuario = new Usuario(email, _senhaHasher.GerarHash(request.Senha));
        await _usuarioRepository.AddAsync(usuario, cancellationToken);
        await _unitOfWork.SaveChangesAsync();

        var token = await _jwtTokenService.GerarTokenAsync(usuario, [], cancellationToken);

        return Result.Success(new AuthenticationResponse
        {
            UsuarioId = usuario.Id,
            Email = usuario.Email.Value,
            Perfis = [],
            Permissoes = [],
            AccessToken = token.AccessToken,
            ExpiraEm = token.ExpiraEm
        });
    }
}
