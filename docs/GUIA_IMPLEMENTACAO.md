# Ofichina - Clean Architecture com .NET 10

## 📋 Resumo da Implementação

✅ **Clean Architecture implementada** com separação clara de responsabilidades
✅ **CQRS Pattern** para Commands e Queries
✅ **Repository Pattern** genérico e específico
✅ **Unit of Work Pattern** para transações
✅ **Specification Pattern** para queries complexas
✅ **Validação** com FluentValidation
✅ **Result Pattern** para retorno de operações
✅ **Value Object Pattern** para objetos de valor

## 🏗️ Estrutura de Projetos

```
Ofichinna/
├── src/
├── Ofichina.Bootstrap/                  [Layer: Composição]
│   └── DependencyInjection.cs           ← Composição da aplicação
│   ├── Ofichina.Api/                    [Layer: Apresentação]
│   │   ├── Program.cs
│   │   └── Modules/
│   │
│   ├── Ofichina.Contracts/              [Layer: Contratos]
│   │   ├── Requests/
│   │   ├── Responses/
│   │   └── DTOs/
│   │
│   ├── Ofichina.Application/            [Layer: Aplicação]
│   │   ├── DependencyInjection/
│   │   │   ├── ApplicationModule.cs     ← Orquestra tudo
│   │   │   ├── ValidationModule.cs
│   │   │   ├── HandlersModule.cs
│   │   │   └── ServicesModule.cs
│   │   ├── Abstractions/
│   │   │   ├── Contracts.cs
│   │   │   └── Handlers.cs
│   │   ├── Validators/
│   │   ├── UseCases/
│   │   │   └── Exemplo/
│   │   │       ├── Commands/
│   │   │       ├── Queries/
│   │   │       └── Handlers/
│   │   └── Services/
│   │
│   ├── Ofichina.Domain/                 [Layer: Domínio]
│   │   ├── Entities/
│   │   │   ├── Entity.cs               ← Base
│   │   │   └── Exemplo.cs
│   │   ├── Interfaces/
│   │   │   ├── IRepository.cs
│   │   │   ├── IUnitOfWork.cs
│   │   │   └── IExemploRepository.cs
│   │   ├── Specifications/
│   │   │   └── Specification.cs
│   │   └── Shared/
│   │       ├── ValueObject.cs
│   │       └── Result.cs
│   │
│   └── Ofichina.Infrastructure/         [Layer: Infraestrutura]
│       ├── Modules/
│       │   ├── InfrastructureModule.cs  ← Orquestra infra
│       │   ├── DatabaseModule.cs
│       │   ├── RepositoryModule.cs
│       │   └── ServicesModule.cs
│       ├── Persistence/
│       │   └── ApplicationDbContext.cs
│       └── Repositories/
│           ├── Repository.cs            ← Genérico
│           ├── UnitOfWork.cs
│           └── ExemploRepository.cs
│
└── tests/
	├── Ofichina.UnitTests/
	└── Ofichina.IntegrationTests/
```

## 🔄 Fluxo de Dependência e Inicialização

```
Program.cs
	↓
builder.Services.AddBootstrapMiddleware(builder.Configuration)
	↓
	BootstrapMiddleware
├── services.AddAuthenticationServices() [serviços e validadores de autenticação]
├── services.AddAuthenticationModule(config) [JWT Bearer]
	├── services.AddApplication()
	│   ├── services.AddValidations()       [FluentValidation autodiscovery]
	│   ├── services.AddHandlers()          [CQRS handlers]
	│   └── services.AddApplicationServices()[App services]
	└── services.AddInfrastructure(config)
		├── AddDatabase(config)             [EF Core + SqlServer]
		├── AddRepositories()               [Generics + Específicos + Auth repo]
		└── AddInfrastructureServices()     [Serviços de apoio, ex.: perfis]
```

## 📦 Padrões de Design Utilizados

| Padrão | Localização | Propósito |
|--------|-----------|----------|
| **CQRS** | Application/Abstractions | Separar Commands (escrita) de Queries (leitura) |
| **Repository** | Domain/Interfaces + Infrastructure/Repositories | Abstração de acesso a dados |
| **Unit of Work** | Domain/Interfaces + Infrastructure/Repositories | Gerenciar transações |
| **Specification** | Domain/Specifications | Encapsular critérios de query |
| **Result** | Domain/Shared | Encapsular resultado com sucesso/erro |
| **Value Object** | Domain/Shared | Objetos imutáveis por valor |
| **Dependency Injection** | Application/DependencyInjection | Inversão de controle |
| **Validation** | Application/Validators | FluentValidation |

## 🚀 Exemplo de Uso - Criar um Novo Entity

### 1️⃣ Domain Layer - Criar Entidade

```csharp
// Domain/Entities/Produto.cs
public class Produto : Entity
{
	public string Nome { get; set; }
	public decimal Preco { get; set; }

	public Produto(string nome, decimal preco)
	{
		Nome = nome;
		Preco = preco;
	}
}
```

### 2️⃣ Domain Layer - Interface do Repositório

```csharp
// Domain/Interfaces/IProdutoRepository.cs
public interface IProdutoRepository : IRepository<Produto>
{
	Task<IEnumerable<Produto>> GetByPrecoAsync(decimal minimo, decimal maximo);
}
```

### 3️⃣ Infrastructure Layer - Implementar Repositório

```csharp
// Infrastructure/Repositories/ProdutoRepository.cs
public class ProdutoRepository : Repository<Produto>, IProdutoRepository
{
	public async Task<IEnumerable<Produto>> GetByPrecoAsync(decimal minimo, decimal maximo)
	{
		return await _context.Set<Produto>()
			.Where(p => p.Preco >= minimo && p.Preco <= maximo)
			.ToListAsync();
	}
}
```

### 4️⃣ Application Layer - Criar Validador

```csharp
// Application/Validators/CreateProdutoRequestValidator.cs
public class CreateProdutoRequestValidator : AbstractValidator<CreateProdutoRequest>
{
	public CreateProdutoRequestValidator()
	{
		RuleFor(x => x.Nome)
			.NotEmpty().WithMessage("Nome é obrigatório")
			.MaximumLength(100);

		RuleFor(x => x.Preco)
			.GreaterThan(0).WithMessage("Preço deve ser maior que zero");
	}
}
```

### 5️⃣ Application Layer - Criar Command

```csharp
// Application/UseCases/Produto/Commands/CreateProdutoCommand.cs
public class CreateProdutoCommand : ICommand<Guid>
{
	public string Nome { get; set; }
	public decimal Preco { get; set; }
}
```

### 6️⃣ Application Layer - Criar Handler

```csharp
// Application/UseCases/Produto/Handlers/CreateProdutoCommandHandler.cs
public class CreateProdutoCommandHandler : ICommandHandler<CreateProdutoCommand, Guid>
{
	private readonly IProdutoRepository _repository;
	private readonly IUnitOfWork _unitOfWork;

	public async Task<Guid> HandleAsync(CreateProdutoCommand command)
	{
		var produto = new Produto(command.Nome, command.Preco);
		await _repository.AddAsync(produto);
		await _unitOfWork.SaveChangesAsync();
		return produto.Id;
	}
}
```

### 7️⃣ Infrastructure Layer - Registrar Repositório

```csharp
// Infrastructure/Modules/RepositoryModule.cs
public static IServiceCollection AddRepositories(this IServiceCollection services)
{
	services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
	services.AddScoped<IUnitOfWork, UnitOfWork>();
	services.AddScoped<IProdutoRepository, ProdutoRepository>();

	return services;
}
```

### 8️⃣ Infrastructure Layer - Adicionar DbSet

```csharp
// Infrastructure/Persistence/ApplicationDbContext.cs
public DbSet<Produto> Produtos { get; set; }
```

### 9️⃣ API Layer - Criar Controller

```csharp
// Api/Controllers/ProdutosController.cs
[ApiController]
[Route("api/[controller]")]
public class ProdutosController : ControllerBase
{
	private readonly ICommandHandler<CreateProdutoCommand, Guid> _createHandler;
	private readonly IQueryHandler<GetProdutoByIdQuery, GetProdutoResponse?> _getHandler;

	[HttpPost]
	public async Task<ActionResult<Guid>> CreateAsync(CreateProdutoRequest request)
	{
		var command = new CreateProdutoCommand { Nome = request.Nome, Preco = request.Preco };
		var id = await _createHandler.HandleAsync(command);
		return CreatedAtAction(nameof(GetAsync), new { id }, id);
	}

	[HttpGet("{id}")]
	public async Task<ActionResult<GetProdutoResponse>> GetAsync(Guid id)
	{
		var query = new GetProdutoByIdQuery(id);
		var result = await _getHandler.HandleAsync(query);

		if (result == null)
			return NotFound();

		return Ok(result);
	}
}
```

## 📝 Checklist para Nova Feature

- [ ] Criar Entidade em `Domain/Entities/`
- [ ] Criar Interface do Repositório em `Domain/Interfaces/`
- [ ] Implementar Repositório em `Infrastructure/Repositories/`
- [ ] Registrar Repositório em `Infrastructure/Modules/RepositoryModule.cs`
- [ ] Adicionar DbSet em `Infrastructure/Persistence/ApplicationDbContext.cs`
- [ ] Criar Validador em `Application/Validators/`
- [ ] Criar Command(s) em `Application/UseCases/Entity/Commands/`
- [ ] Criar Query(ies) em `Application/UseCases/Entity/Queries/`
- [ ] Criar Handler(s) em `Application/UseCases/Entity/Handlers/`
- [ ] Criar Controller em `Api/Controllers/`
- [ ] Adicionar DTO em `Contracts/Requests/` e `Contracts/Responses/`

## 🔧 Configuração Inicial Necessária

### appsettings.json
```json
{
  "ConnectionStrings": {
	"DefaultConnection": "Server=.;Database=Ofichinna;Trusted_Connection=true;TrustServerCertificate=true;"
  }
}
```

### Entity Framework Migrations
```bash
# Adicionar migration
dotnet ef migrations add InitialAuth -p src/Ofichina.Infrastructure

# Aplicar migration
dotnet ef database update -p src/Ofichina.Infrastructure
```

> A factory de design-time está configurada em `src/Ofichina.Infrastructure/Persistence/ApplicationDbContextFactory.cs` para que o comando funcione sem depender da API.

## 📚 Referências

- [Clean Architecture by Robert C. Martin](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [CQRS Pattern](https://martinfowler.com/bliki/CQRS.html)
- [FluentValidation Documentation](https://docs.fluentvalidation.net/)
- [Entity Framework Core](https://docs.microsoft.com/ef/core/)

## ✅ Status de Compilação

- **Projeto API**: ✅ Compilando com sucesso
- **Build**: ✅ Sem erros
- **Arquitetura**: ✅ Validada

---

**Implementado em:** 2026  
**Última atualização:** 2026  
**Versão:** 2.0  
**Framework:** .NET 10  
**Padrão:** Clean Architecture + CQRS
