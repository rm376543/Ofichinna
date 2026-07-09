using Ofichina.Authentication.Abstractions;
using Ofichina.Authentication.Services;
using Ofichina.Contracts.Responses;
using Ofichina.Contracts.Requests;
using Ofichina.Domain.Entities;
using Ofichina.Domain.Interfaces;
using Ofichina.Domain.ValueObjects;

namespace Ofichina.UnitTests.Autenticacao;

public sealed class CadastrarUsuarioCommandHandlerTests
{
    [Fact]
    public async Task Deve_Cadastrar_Usuario_E_Retornar_Token()
    {
        var usuarios = new List<Usuario>();
        var repository = new FakeUsuarioRepository(usuarios);
        var consultaRepository = new FakeUsuarioAutenticacaoRepository(usuarios);
        var jwtTokenService = new FakeJwtTokenService();
        var senhaHasher = new FakeSenhaHasher();
        var perfilService = new FakePerfilAutorizacaoService();
        var unitOfWork = new FakeUnitOfWork();
        var handler = new AutenticacaoService(repository, unitOfWork, consultaRepository, perfilService, jwtTokenService, senhaHasher);

        var result = await handler.CadastrarAsync(new CadastrarUsuarioRequest { Nome = "Maria Silva", Email = "maria@ofichinna.com", Senha = "123456" });

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Single(usuarios);
        Assert.Equal("Maria Silva", usuarios[0].Nome);
        Assert.Equal("maria@ofichinna.com", usuarios[0].Email.Value);
        Assert.Equal("hash:123456", usuarios[0].SenhaHash);
        Assert.Equal("fake-token", result.Value!.AccessToken);
        Assert.Empty(result.Value.Perfis);
        Assert.Equal(1, unitOfWork.SaveChangesCalls);
        Assert.Empty(jwtTokenService.PerfisRecebidos);
    }

    [Fact]
    public async Task Deve_Falhar_Quando_Email_Ja_Existe()
    {
        var emailCriado = Email.Criar("maria@ofichinna.com");

        var usuarios = new List<Usuario>
        {
            new("Maria Silva", emailCriado, "hash:123456")
        };
        var repository = new FakeUsuarioRepository(usuarios);
        var consultaRepository = new FakeUsuarioAutenticacaoRepository(usuarios);
        var jwtTokenService = new FakeJwtTokenService();
        var senhaHasher = new FakeSenhaHasher();
        var perfilService = new FakePerfilAutorizacaoService();
        var unitOfWork = new FakeUnitOfWork();
        var handler = new AutenticacaoService(repository, unitOfWork, consultaRepository, perfilService, jwtTokenService, senhaHasher);

        var result = await handler.CadastrarAsync(new CadastrarUsuarioRequest { Nome = "Maria Silva", Email = "maria@ofichinna.com", Senha = "123456" });

        Assert.False(result.IsSuccess);
        Assert.Equal("Já existe um usuário cadastrado com este e-mail.", result.Error);
        Assert.Equal(1, usuarios.Count);
        Assert.Equal(0, unitOfWork.SaveChangesCalls);
    }

    private sealed class FakeUsuarioRepository : IRepository<Usuario>
    {
        private readonly List<Usuario> _usuarios;

        public FakeUsuarioRepository(List<Usuario> usuarios)
        {
            _usuarios = usuarios;
        }

        public Task AddAsync(Usuario entity)
        {
            _usuarios.Add(entity);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Usuario entity) => Task.CompletedTask;

        public Task<IEnumerable<Usuario>> GetAllAsync() => Task.FromResult<IEnumerable<Usuario>>(_usuarios);

        public Task<Usuario?> GetByIdAsync(Guid id) => Task.FromResult(_usuarios.FirstOrDefault(x => x.Id == id));

        public Task UpdateAsync(Usuario entity) => Task.CompletedTask;
    }

    private sealed class FakeUsuarioAutenticacaoRepository : IUsuarioAutenticacaoRepository
    {
        private readonly List<Usuario> _usuarios;

        public FakeUsuarioAutenticacaoRepository(List<Usuario> usuarios)
        {
            _usuarios = usuarios;
        }

        public Task<Usuario?> ObterPorEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            var normalizedEmail = Email.Criar(email).Value;
            return Task.FromResult(_usuarios.FirstOrDefault(x => x.Email.Value == normalizedEmail));
        }
    }

    private sealed class FakeJwtTokenService : IJwtTokenService
    {
        public IReadOnlyCollection<string> PerfisRecebidos { get; private set; } = [];

        public Task<TokenJwtResponse> GerarTokenAsync(Usuario usuario, IReadOnlyCollection<string> perfis, CancellationToken cancellationToken = default)
        {
            PerfisRecebidos = perfis;
            return Task.FromResult(new TokenJwtResponse
            {
                AccessToken = "fake-token",
                ExpiraEm = DateTime.UtcNow.AddHours(1)
            });
        }
    }

    private sealed class FakePerfilAutorizacaoService : IPerfilAutorizacaoService
    {
        public Task<IReadOnlyCollection<string>> ObterPerfisAsync(Guid usuarioId, CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IReadOnlyCollection<string>>([]);
        }

        public Task<bool> PossuiPerfilAsync(Guid usuarioId, string perfil, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(false);
        }
    }

    private sealed class FakeSenhaHasher : ISenhaHasher
    {
        public string GerarHash(string senha) => $"hash:{senha}";

        public bool Verificar(string senha, string hash) => hash == $"hash:{senha}";
    }

    private sealed class FakeUnitOfWork : IUnitOfWork
    {
        public int SaveChangesCalls { get; private set; }

        public Task BeginTransactionAsync() => Task.CompletedTask;

        public Task CommitTransactionAsync() => Task.CompletedTask;

        public ValueTask DisposeAsync() => ValueTask.CompletedTask;

        public Task RollbackTransactionAsync() => Task.CompletedTask;

        public Task<int> SaveChangesAsync()
        {
            SaveChangesCalls++;
            return Task.FromResult(1);
        }
    }
}