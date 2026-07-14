using Ofichina.Application.UseCases.Autenticacao.Commands;
using Ofichina.Application.UseCases.Autenticacao.Handlers;
using Ofichina.Authentication.Abstractions;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Responses;
using Ofichina.Domain.Entities;
using Ofichina.Domain.ValueObjects;
using Microsoft.Extensions.Logging.Abstractions;

namespace Ofichina.UnitTests.Application.Autenticacao;

public class AutenticarCommandHandlerTests
{
    [Fact]
    public async Task HandleAsync_DeveRetornarToken_QuandoCredenciaisForemValidas()
    {
        var usuario = CriarUsuarioAtivo("admin@ofichinna.com", "hash-da-senha");

        var repository = new FakeUsuarioAutenticacaoRepository(usuario);
        var perfilService = new FakePerfilAutorizacaoService(new[] { "ADMIN" });
        var tokenService = new FakeJwtTokenService();
        var senhaHasher = new FakeSenhaHasher(true);

        var handler = new AutenticarCommandHandler(
            repository,
            perfilService,
            tokenService,
            senhaHasher,
            NullLogger<AutenticarCommandHandler>.Instance);
        var command = new AutenticarCommand("admin@ofichinna.com", "Senha@123");

        Result<AutenticacaoResponse> result = await handler.HandleAsync(command);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(usuario.Id, result.Value!.UsuarioId);
        Assert.Equal("admin@ofichinna.com", result.Value.Email);
        Assert.Equal("token-gerado", result.Value.AccessToken);
        Assert.Equal(new[] { "ADMIN" }, result.Value.Perfis);
        Assert.Equal("token-gerado", tokenService.TokenGerado);
        Assert.True(repository.FoiChamado);
        Assert.True(perfilService.FoiChamado);
        Assert.True(tokenService.FoiChamado);
    }

    [Fact]
    public async Task HandleAsync_DeveFalhar_QuandoUsuarioNaoExistir()
    {
        var repository = new FakeUsuarioAutenticacaoRepository(null);
        var perfilService = new FakePerfilAutorizacaoService(Array.Empty<string>());
        var tokenService = new FakeJwtTokenService();
        var senhaHasher = new FakeSenhaHasher(true);

        var handler = new AutenticarCommandHandler(
            repository,
            perfilService,
            tokenService,
            senhaHasher,
            NullLogger<AutenticarCommandHandler>.Instance);
        var command = new AutenticarCommand("inexistente@ofichinna.com", "Senha@123");

        Result<AutenticacaoResponse> result = await handler.HandleAsync(command);

        Assert.False(result.IsSuccess);
        Assert.Equal("Verifique os dados fornecidos.", result.Error);
        Assert.Null(result.Value);
        Assert.True(repository.FoiChamado);
        Assert.False(perfilService.FoiChamado);
        Assert.False(tokenService.FoiChamado);
    }

    private static Usuario CriarUsuarioAtivo(string email, string senhaHash)
    {
        Email usuarioEmail = new Email(email);

        var usuario = new Usuario(usuarioEmail, senhaHash);

        return usuario;
    }

    private sealed class FakeUsuarioAutenticacaoRepository : IUsuarioAutenticacaoRepository
    {
        private readonly Usuario? _usuario;

        public bool FoiChamado { get; private set; }

        public FakeUsuarioAutenticacaoRepository(Usuario? usuario)
        {
            _usuario = usuario;
        }

        public Task<Usuario?> ObterPorEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            FoiChamado = true;
            return Task.FromResult(_usuario);
        }
    }

    private sealed class FakePerfilAutorizacaoService : IPerfilAutorizacaoService
    {
        private readonly IReadOnlyCollection<string> _perfis;

        public bool FoiChamado { get; private set; }

        public FakePerfilAutorizacaoService(IReadOnlyCollection<string> perfis)
        {
            _perfis = perfis;
        }

        public Task<IReadOnlyCollection<string>> ObterPerfisAsync(Guid usuarioId, CancellationToken cancellationToken = default)
        {
            FoiChamado = true;
            return Task.FromResult(_perfis);
        }

        public Task<bool> PossuiPerfilAsync(Guid usuarioId, string perfil, CancellationToken cancellationToken = default)
            => Task.FromResult(_perfis.Contains(perfil));
    }

    private sealed class FakeJwtTokenService : IJwtTokenService
    {
        public bool FoiChamado { get; private set; }

        public string? TokenGerado { get; private set; }

        public Task<TokenJwtResponse> GerarTokenAsync(Usuario usuario, IReadOnlyCollection<string> perfis, CancellationToken cancellationToken = default)
        {
            FoiChamado = true;
            TokenGerado = "token-gerado";

            return Task.FromResult(new TokenJwtResponse
            {
                AccessToken = TokenGerado,
                ExpiraEm = new DateTime(2026, 07, 11, 12, 00, 00, DateTimeKind.Utc)
            });
        }
    }

    private sealed class FakeSenhaHasher : ISenhaHasher
    {
        private readonly bool _resultado;

        public FakeSenhaHasher(bool resultado)
        {
            _resultado = resultado;
        }

        public string GerarHash(string senha) => "hash";

        public bool Verificar(string senha, string hash) => _resultado;
    }
}