using Ofichina.Application.Abstractions;
using Ofichina.Authentication.Abstractions;
using Ofichina.Contracts.Responses;
using Ofichina.Domain.ValueObjects;
using Ofichina.Application.UseCases.Autenticacao.Commands;
using Ofichina.Contracts.Common;

namespace Ofichina.Application.UseCases.Autenticacao.Handlers;

public sealed class AutenticarCommandHandler : ICommandHandler<AutenticarCommand, Result<AutenticacaoResponse>>
{
    private readonly IUsuarioAutenticacaoRepository _usuarioAutenticacaoRepository;
    private readonly IPerfilAutorizacaoService _perfilService;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly ISenhaHasher _senhaHasher;

    public AutenticarCommandHandler(
        IUsuarioAutenticacaoRepository usuarioAutenticacaoRepository,
        IPerfilAutorizacaoService perfilService,
        IJwtTokenService jwtTokenService,
        ISenhaHasher senhaHasher)
    {
        _usuarioAutenticacaoRepository = usuarioAutenticacaoRepository;
        _perfilService = perfilService;
        _jwtTokenService = jwtTokenService;
        _senhaHasher = senhaHasher;
    }

    public async Task<Result<AutenticacaoResponse>> HandleAsync(AutenticarCommand command)
    {
        Email email = Email.Criar(command.Email);

        var usuario = await _usuarioAutenticacaoRepository.ObterPorEmailAsync(email.Value);

        if (usuario is null || !usuario.Ativo)
            return Result.Failure<AutenticacaoResponse>("Credenciais inválidas.");

        if (!_senhaHasher.Verificar(command.Senha, usuario.SenhaHash))
            return Result.Failure<AutenticacaoResponse>("Credenciais inválidas.");

        var perfis = await _perfilService.ObterPerfisAsync(usuario.Id);
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