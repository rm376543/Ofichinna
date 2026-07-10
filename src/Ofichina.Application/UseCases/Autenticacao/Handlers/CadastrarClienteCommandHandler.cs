using Ofichina.Application.Abstractions;
using Ofichina.Authentication.Abstractions;
using Ofichina.Contracts.Responses;
using Ofichina.Domain.Entities;
using Ofichina.Domain.Interfaces;
using Ofichina.Domain.ValueObjects;
using Ofichina.Application.UseCases.Autenticacao.Commands;
using Ofichina.Contracts.Common;

namespace Ofichina.Application.UseCases.Autenticacao.Handlers;

public sealed class CadastrarClienteCommandHandler : ICommandHandler<CadastrarClienteCommand, Result<AutenticacaoResponse>>
{
    private readonly IRepository<Usuario> _usuarioRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUsuarioAutenticacaoRepository _usuarioAutenticacaoRepository;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly ISenhaHasher _senhaHasher;

    public CadastrarClienteCommandHandler(
        IRepository<Usuario> usuarioRepository,
        IUnitOfWork unitOfWork,
        IUsuarioAutenticacaoRepository usuarioAutenticacaoRepository,
        IJwtTokenService jwtTokenService,
        ISenhaHasher senhaHasher)
    {
        _usuarioRepository = usuarioRepository;
        _unitOfWork = unitOfWork;
        _usuarioAutenticacaoRepository = usuarioAutenticacaoRepository;
        _jwtTokenService = jwtTokenService;
        _senhaHasher = senhaHasher;
    }

    public async Task<Result<AutenticacaoResponse>> HandleAsync(CadastrarClienteCommand command)
    {
        var nome = command.Nome.Trim();
        var email = Email.Criar(command.Email);

        var usuarioExistente = await _usuarioAutenticacaoRepository.ObterPorEmailAsync(email.Value);

        if (usuarioExistente is not null)
        {
            return Result.Failure<AutenticacaoResponse>("Já existe um usuário cadastrado com este e-mail.");
        }

        var usuario = new Usuario(nome, email, _senhaHasher.GerarHash(command.Senha));

        await _usuarioRepository.AddAsync(usuario);
        await _unitOfWork.SaveChangesAsync();

        var perfis = Array.Empty<string>();
        var token = await _jwtTokenService.GerarTokenAsync(usuario, perfis);

        return Result.Success(new AutenticacaoResponse
        {
            UsuarioId = usuario.Id,
            Nome = usuario.Nome,
            Email = usuario.Email.Value,
            Perfis = perfis,
            AccessToken = token.AccessToken,
            ExpiraEm = token.ExpiraEm
        });
    }
}