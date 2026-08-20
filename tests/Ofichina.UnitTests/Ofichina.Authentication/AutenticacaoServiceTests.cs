using Ofichina.Application.Abstractions.Authentication.Repository;
using Ofichina.Application.Abstractions.Authentication.Service;
using Ofichina.Authentication.Services;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Requests.Autenticacao;
using Ofichina.Contracts.Responses.Authentication;
using Ofichina.Domain.Entities;
using Ofichina.Domain.ValueObjects;

namespace Ofichina.UnitTests.Ofichina.Authentication;

public sealed class AutenticacaoServiceTests
{
    [Fact]
    public async Task AutenticarAsync_Deve_Retornar_Falha_Quando_Usuario_Nao_Existir()
    {
        var usuarios = new List<Usuario>();
        var service = CriarServico(usuarios, senhaValida: true);

        var result = await service.AutenticarAsync(new AutenticacaoRequest
        {
            Email = "naoexiste@ofichinna.com",
            Senha = "123456"
        });

        Assert.False(result.IsSuccess);
        Assert.Equal("Credenciais inválidas.", result.Error);
    }

    [Fact]
    public async Task AutenticarAsync_Deve_Retornar_Falha_Quando_Usuario_Estiver_Inativo()
    {
        var usuario = new Usuario(new Email("maria@ofichinna.com"), "hash:123456");
        usuario.Excluir();

        var service = CriarServico(new List<Usuario> { usuario }, senhaValida: true);

        var result = await service.AutenticarAsync(new AutenticacaoRequest
        {
            Email = "maria@ofichinna.com",
            Senha = "123456"
        });

        Assert.False(result.IsSuccess);
        Assert.Equal("Credenciais inválidas.", result.Error);
    }

    [Fact]
    public async Task AutenticarAsync_Deve_Retornar_Falha_Quando_Senha_For_Invalida()
    {
        var usuario = new Usuario(new Email("maria@ofichinna.com"), "hash:123456");

        var service = CriarServico(new List<Usuario> { usuario }, senhaValida: false);

        var result = await service.AutenticarAsync(new AutenticacaoRequest
        {
            Email = "maria@ofichinna.com",
            Senha = "senha-errada"
        });

        Assert.False(result.IsSuccess);
        Assert.Equal("Credenciais inválidas.", result.Error);
    }

    [Fact]
    public async Task AutenticarAsync_Deve_Retornar_Token_Com_Perfis_E_Permissoes()
    {
        var usuario = new Usuario(new Email("maria@ofichinna.com"), "hash:123456");

        var perfilService = new FakeProfileAuthService(["ADMIN"], ["usuarios.listar", "usuarios.editar"]);
        var jwtTokenService = new FakeJwtTokenService();
        var service = new AutenticacaoService(
            new FakeUsuarioRepository([usuario]),
            new FakeUnitOfWork(),
            new FakeUsuarioAutenticacaoRepository([usuario]),
            perfilService,
            jwtTokenService,
            new FakePasswordHasher(true));

        var result = await service.AutenticarAsync(new AutenticacaoRequest
        {
            Email = "maria@ofichinna.com",
            Senha = "123456"
        });

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(usuario.Id, result.Value!.UsuarioId);
        Assert.Equal("maria@ofichinna.com", result.Value.Email);
        Assert.Equal(["ADMIN"], result.Value.Perfis);
        Assert.Equal(["usuarios.listar", "usuarios.editar"], result.Value.Permissoes);
        Assert.Equal("fake-token", result.Value.AccessToken);
        Assert.Equal(usuario.Id, jwtTokenService.UsuarioRecebido?.Id);
        Assert.Equal(["ADMIN"], jwtTokenService.PerfisRecebidos);
        Assert.Equal(1, perfilService.ObterPerfisCalls);
        Assert.Equal(1, perfilService.ObterPermissoesCalls);
    }

    private static AutenticacaoService CriarServico(List<Usuario> usuarios, bool senhaValida)
    {
        return new AutenticacaoService(
            new FakeUsuarioRepository(usuarios),
            new FakeUnitOfWork(),
            new FakeUsuarioAutenticacaoRepository(usuarios),
            new FakeProfileAuthService([], []),
            new FakeJwtTokenService(),
            new FakePasswordHasher(senhaValida));
    }

    private sealed class FakeUsuarioRepository : IRepository<Usuario>
    {
        private readonly List<Usuario> _usuarios;

        public FakeUsuarioRepository(List<Usuario> usuarios) => _usuarios = usuarios;

        public Task AddAsync(Usuario entity, CancellationToken cancellationToken = default)
        {
            _usuarios.Add(entity);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Usuario entity, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task<IEnumerable<Usuario>> GetAllAsync(CancellationToken cancellationToken = default) => Task.FromResult<IEnumerable<Usuario>>(_usuarios);

        public Task<Usuario?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default, bool tracking = false) => Task.FromResult(_usuarios.FirstOrDefault(x => x.Id == id));

        public Task UpdateAsync(Usuario entity, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task<PagedResponse<Usuario>> GetPagedAsync(Pagination pagination, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new PagedResponse<Usuario>());
        }

        public Task HardDeleteAsync(Usuario entity, CancellationToken cancellationToken = default) => Task.CompletedTask;
    }

    private sealed class FakeUsuarioAutenticacaoRepository : IUserAuthRepository
    {
        private readonly List<Usuario> _usuarios;

        public FakeUsuarioAutenticacaoRepository(List<Usuario> usuarios) => _usuarios = usuarios;

        public Task<Usuario?> ObterPorEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            var normalizedEmail = new Email(email);
            return Task.FromResult(_usuarios.FirstOrDefault(x => x.Email == normalizedEmail));
        }
    }

    private sealed class FakeProfileAuthService : IProfileAuthService
    {
        private readonly IReadOnlyCollection<string> _perfis;
        private readonly IReadOnlyCollection<string> _permissoes;

        public int ObterPerfisCalls { get; private set; }
        public int ObterPermissoesCalls { get; private set; }

        public FakeProfileAuthService(IReadOnlyCollection<string> perfis, IReadOnlyCollection<string> permissoes)
        {
            _perfis = perfis;
            _permissoes = permissoes;
        }

        public Task<IReadOnlyCollection<string>> ObterPerfisAsync(Guid usuarioId, CancellationToken cancellationToken = default)
        {
            ObterPerfisCalls++;
            return Task.FromResult(_perfis);
        }

        public Task<bool> PossuiPerfilAsync(Guid usuarioId, string perfil, CancellationToken cancellationToken = default)
            => Task.FromResult(_perfis.Contains(perfil));

        public Task<IReadOnlyCollection<string>> ObterPermissoesAsync(Guid usuarioId, CancellationToken cancellationToken = default)
        {
            ObterPermissoesCalls++;
            return Task.FromResult(_permissoes);
        }

        public Task<bool> PossuiPermissaoAsync(Guid usuarioId, string permissao, CancellationToken cancellationToken = default)
            => Task.FromResult(_permissoes.Contains(permissao));
    }

    private sealed class FakeJwtTokenService : IJwtTokenService
    {
        public Usuario? UsuarioRecebido { get; private set; }
        public IReadOnlyCollection<string> PerfisRecebidos { get; private set; } = [];

        public Task<JwtResponse> GerarTokenAsync(Usuario usuario, IReadOnlyCollection<string> perfis, CancellationToken cancellationToken = default)
        {
            UsuarioRecebido = usuario;
            PerfisRecebidos = perfis;

            return Task.FromResult(new JwtResponse
            {
                AccessToken = "fake-token",
                ExpiraEm = DateTime.UtcNow.AddHours(1)
            });
        }
    }

    private sealed class FakePasswordHasher : IPasswordHasherService
    {
        private readonly bool _senhaValida;

        public FakePasswordHasher(bool senhaValida) => _senhaValida = senhaValida;

        public string GerarHash(string senha) => $"hash:{senha}";

        public bool Verificar(string senha, string hash) => _senhaValida && hash == $"hash:{senha}";
    }

    private sealed class FakeUnitOfWork : IUnitOfWork
    {
        public Task BeginTransactionAsync() => Task.CompletedTask;
        public Task CommitTransactionAsync() => Task.CompletedTask;
        public ValueTask DisposeAsync() => ValueTask.CompletedTask;
        public Task RollbackTransactionAsync() => Task.CompletedTask;
        public Task<int> SaveChangesAsync() => Task.FromResult(1);
    }
}