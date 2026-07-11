using Ofichina.Contracts.Responses;
using Ofichina.Domain.Entities;
using Ofichina.Domain.Interfaces;
using Ofichina.Domain.ValueObjects;
using Ofichina.Authentication.Abstractions;
using Ofichina.Contracts.Requests.Autenticacao;
using Ofichina.Contracts.Requests.Usuario;
using Ofichina.Contracts.Common;

namespace Ofichina.Authentication.Services;

public sealed class AutenticacaoService : IAutenticacaoService
{
    private readonly IRepository<Usuario> _usuarioRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUsuarioAutenticacaoRepository _usuarioAutenticacaoRepository;
    private readonly IPerfilAutorizacaoService _perfilService;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly ISenhaHasher _senhaHasher;

    public AutenticacaoService(
        IRepository<Usuario> usuarioRepository,
        IUnitOfWork unitOfWork,
        IUsuarioAutenticacaoRepository usuarioAutenticacaoRepository,
        IPerfilAutorizacaoService perfilService,
        IJwtTokenService jwtTokenService,
        ISenhaHasher senhaHasher)
    {
        _usuarioRepository = usuarioRepository;
        _unitOfWork = unitOfWork;
        _usuarioAutenticacaoRepository = usuarioAutenticacaoRepository;
        _perfilService = perfilService;
        _jwtTokenService = jwtTokenService;
        _senhaHasher = senhaHasher;
    }

    public async Task<Result<AutenticacaoResponse>> AutenticarAsync(AutenticacaoRequest request, CancellationToken cancellationToken = default)
    {
        Email email = Email.Criar(request.Email);
        var usuario = await _usuarioAutenticacaoRepository.ObterPorEmailAsync(email.Value, cancellationToken);

        if (usuario is null || !usuario.UsuarioEstaAtivo())
        {
            return Result.Failure<AutenticacaoResponse>("Credenciais inválidas.");
        }

        if (!_senhaHasher.Verificar(request.Senha, usuario.SenhaHash))
        {
            return Result.Failure<AutenticacaoResponse>("Credenciais inválidas.");
        }

        var perfis = await _perfilService.ObterPerfisAsync(usuario.Id, cancellationToken);
        var token = await _jwtTokenService.GerarTokenAsync(usuario, perfis, cancellationToken);

        return Result.Success(new AutenticacaoResponse
        {
            UsuarioId = usuario.Id,
            Email = usuario.Email.Value,
            Perfis = perfis,
            AccessToken = token.AccessToken,
            ExpiraEm = token.ExpiraEm
        });
    }

    public async Task<Result<AutenticacaoResponse>> CadastrarAsync(CreateClienteRequest request, CancellationToken cancellationToken = default)
    {
        Email email = Email.Criar(request.Email);

        var usuarioExistente = await _usuarioAutenticacaoRepository.ObterPorEmailAsync(email.Value, cancellationToken);
        if (usuarioExistente is not null)
        {
            return Result.Failure<AutenticacaoResponse>("Já existe um usuário cadastrado com este e-mail.");
        }

        var usuario = new Usuario(email, _senhaHasher.GerarHash(request.Senha));
        await _usuarioRepository.AddAsync(usuario);
        await _unitOfWork.SaveChangesAsync();

        var token = await _jwtTokenService.GerarTokenAsync(usuario, [], cancellationToken);

        return Result.Success(new AutenticacaoResponse
        {
            UsuarioId = usuario.Id,
            Email = usuario.Email.Value,
            Perfis = [],
            AccessToken = token.AccessToken,
            ExpiraEm = token.ExpiraEm
        });
    }
}