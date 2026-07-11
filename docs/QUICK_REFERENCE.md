# 📇 QUICK REFERENCE CARD - Ofichinna

## 🚀 Comece em 30 Segundos

```bash
# 1. Clonar e entrar no projeto
git clone [url]
cd Ofichinna

# 2. Restaurar dependências
dotnet restore

# 3. Compilar
dotnet build

# 4. Criar banco de dados
dotnet ef migrations add InitialCreate -p src/Ofichina.Infrastructure
dotnet ef database update -p src/Ofichina.Infrastructure

# 5. Executar
dotnet run --project src/Ofichina.Api

# 6. Acessar
https://localhost:7000/swagger
```

---

## 📚 Documentação Rápida

| Necessidade | Documento | Tempo |
|------------|-----------|-------|
| Entender projeto | README.md | 5 min |
| Visão executiva | SUMARIO_EXECUTIVO.md | 5 min |
| Estrutura visual | MAPA_VISUAL.md | 10 min |
| Técnico detalhado | ARQUITETURA.md | 15 min |
| Implementar feature | GUIA_IMPLEMENTACAO.md | 20 min |
| Validação técnica | RELATORIO_IMPLEMENTACAO.md | 10 min |
| Problemas comuns | TROUBLESHOOTING.md | 5 min |
| Navegar docs | INDICE.md | 5 min |

---

## 🏗️ Estrutura de Projetos

```
Ofichinna/
├── src/
│   ├── Ofichina.Bootstrap/
│   ├── Ofichina.Api/
│   ├── Ofichina.Contracts/
│   ├── Ofichina.Application/
│   ├── Ofichina.Domain/
│   └── Ofichina.Infrastructure/
├── tests/
│   ├── Ofichina.UnitTests/
│   └── Ofichina.IntegrationTests/
└── [Documentação - veja abaixo]
```

---

## 🔧 Comandos Úteis

```bash
# Build
dotnet build

# Testes
dotnet test

# Migrations - Adicionar
dotnet ef migrations add [NomeMigration] -p src/Ofichina.Infrastructure

# Migrations - Aplicar
dotnet ef database update -p src/Ofichina.Infrastructure

# Rodar projeto
dotnet run --project src/Ofichina.Api

# Remover última migration
dotnet ef migrations remove -p src/Ofichina.Infrastructure

# Ver migrations
dotnet ef migrations list -p src/Ofichina.Infrastructure
```

---

## 📋 Padrão: Adicionar Nova Entidade

### 1️⃣ Domain/Entities/
```csharp
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

### 2️⃣ Domain/Interfaces/
```csharp
public interface IProdutoRepository : IRepository<Produto>
{
	Task<Produto?> GetByNameAsync(string nome);
}
```

### 3️⃣ Infrastructure/Repositories/
```csharp
public class ProdutoRepository : Repository<Produto>, IProdutoRepository
{
	public async Task<Produto?> GetByNameAsync(string nome)
	{
		return await _context.Set<Produto>()
			.FirstOrDefaultAsync(p => p.Nome == nome);
	}
}
```

### 4️⃣ Infrastructure/Modules/RepositoryModule.cs
```csharp
services.AddScoped<IProdutoRepository, ProdutoRepository>();
```

### 5️⃣ Infrastructure/Persistence/ApplicationDbContext.cs
```csharp
public DbSet<Produto> Produtos { get; set; }
```

### 6️⃣ Application/Validators/
```csharp
public class CreateProdutoRequestValidator : AbstractValidator<CreateProdutoRequest>
{
	public CreateProdutoRequestValidator()
	{
		RuleFor(x => x.Nome).NotEmpty().MaximumLength(100);
		RuleFor(x => x.Preco).GreaterThan(0);
	}
}
```

### 7️⃣ Application/UseCases/Produto/Commands/
```csharp
public class CreateProdutoCommand : ICommand<Guid>
{
	public string Nome { get; set; }
	public decimal Preco { get; set; }
}
```

### 8️⃣ Application/UseCases/Produto/Handlers/
```csharp
public class CreateProdutoCommandHandler : ICommandHandler<CreateProdutoCommand, Guid>
{
	public async Task<Guid> HandleAsync(CreateProdutoCommand command)
	{
		var produto = new Produto(command.Nome, command.Preco);
		await _repository.AddAsync(produto);
		await _unitOfWork.SaveChangesAsync();
		return produto.Id;
	}
}
```

### 9️⃣ Api/Controllers/
```csharp
[ApiController]
[Route("api/[controller]")]
public class ProdutosController : ControllerBase
{
	private readonly ICommandHandler<CreateProdutoCommand, Guid> _createHandler;

	[HttpPost]
	public async Task<ActionResult<Guid>> Create(CreateProdutoRequest request)
	{
		var command = new CreateProdutoCommand { Nome = request.Nome, Preco = request.Preco };
		var id = await _createHandler.HandleAsync(command);
		return CreatedAtAction(nameof(Get), new { id }, id);
	}
}
```

---

## 🎯 Padrões de Código

### ✅ CORRETO
```csharp
// Domain - Puro negócio
public class Cliente : Entity
{
	public string Email { get; set; }
	public bool ValidarEmail() => Email.Contains("@");
}

// Application - Orquestração
public class CreateClienteCommandHandler : ICommandHandler<CreateClienteCommand, Guid>
{
	public async Task<Guid> HandleAsync(CreateClienteCommand command)
	{
		var cliente = new Cliente(command.Email);
		if (!cliente.ValidarEmail()) throw new Exception("Email inválido");
		await _repository.AddAsync(cliente);
		await _unitOfWork.SaveChangesAsync();
		return cliente.Id;
	}
}

// Infrastructure - Persistência
public class ClienteRepository : Repository<Cliente>
{
	public async Task<Cliente?> GetByEmailAsync(string email)
	{
		return await _context.Set<Cliente>()
			.FirstOrDefaultAsync(c => c.Email == email);
	}
}
```

### ❌ INCORRETO
```csharp
// NÃO use DbContext em Controllers
[HttpPost]
public async Task Create(CreateClienteRequest request)
{
	var cliente = new Cliente(request.Email);
	_context.Add(cliente); // ❌ ERRADO
	await _context.SaveChangesAsync();
}

// NÃO passe Entity diretamente na API
public class ClientesController : ControllerBase
{
	[HttpGet("{id}")]
	public async Task<Cliente> Get(Guid id) // ❌ ERRADO - retorna Entity
	{
		return await _repository.GetByIdAsync(id);
	}
}

// NÃO coloque lógica complexa em Controllers
public async Task<IActionResult> Create(CreateClienteRequest request)
{
	// ❌ ERRADO - lógica deveria estar no Handler
	if (request.Email.Count(c => c == '@') != 1)
		return BadRequest();
}
```

---

## 🧪 Testando

```bash
# Executar todos os testes
dotnet test

# Executar testes de um projeto
dotnet test tests/Ofichina.UnitTests

# Executar teste específico
dotnet test --filter ClassName=TestClass

# Com cobertura
dotnet test /p:CollectCoverage=true
```

---

## 📎 Documentos úteis

- `API_REFERENCE.md` para contratos e exemplos reais da API.
- `CONTRIBUTING.md` para padrões de documentação e contribuição.
- `TROUBLESHOOTING.md` para suporte operacional.

## 🔑 Variáveis de Ambiente

```json
// appsettings.json
{
  "ConnectionStrings": {
	"DefaultConnection": "Server=.;Database=Ofichinna;Trusted_Connection=true;TrustServerCertificate=true;"
  },
  "Logging": {
	"LogLevel": {
	  "Default": "Information"
	}
  }
}

// appsettings.Production.json
{
  "ConnectionStrings": {
	"DefaultConnection": "Server=[PROD_SERVER];Database=Ofichinna;User Id=[USER];Password=[PASSWORD];"
  }
}
```

---

## 📱 HTTP Examples

```bash
# GET - Buscar por ID
curl -X GET "https://localhost:7000/api/exemplos/550e8400-e29b-41d4-a716-446655440000"

# POST - Criar
curl -X POST "https://localhost:7000/api/exemplos" \
  -H "Content-Type: application/json" \
  -d '{"nome":"Meu Exemplo","descricao":"Descrição"}'

# PUT - Atualizar
curl -X PUT "https://localhost:7000/api/exemplos/550e8400-e29b-41d4-a716-446655440000" \
  -H "Content-Type: application/json" \
  -d '{"nome":"Novo Nome"}'

# DELETE - Remover
curl -X DELETE "https://localhost:7000/api/exemplos/550e8400-e29b-41d4-a716-446655440000"

# GET - Listar com Paginação
curl -X GET "https://localhost:7000/api/exemplos?pageNumber=1&pageSize=10"
```

---

## 🧠 Conceitos-Chave

| Termo | Significa |
|-------|-----------|
| **Entity** | Objeto com identidade (Id) |
| **Repository** | Abstração de acesso a dados |
| **Unit of Work** | Gerencia múltiplas operações em transação |
| **Command** | Operação que muda estado (POST, PUT, DELETE) |
| **Query** | Operação que lê dados (GET) |
| **Handler** | Executa um Command ou Query |
| **DTO** | Objeto transferência entre camadas |
| **Specification** | Encapsula critério de consulta |

---

## 🚨 Troubleshooting

### Problema: Migration não funciona
```bash
# Solução: Checar caminho do projeto
dotnet ef migrations add InitialCreate -p src/Ofichina.Infrastructure

# Ou estar no diretório correto
cd src/Ofichina.Infrastructure
dotnet ef migrations add InitialCreate
```

### Problema: Erro de compilação
```bash
# Solução 1: Restaurar pacotes
dotnet restore

# Solução 2: Limpar e compilar
dotnet clean
dotnet build

# Solução 3: Verificar versão do .NET
dotnet --version  # Deve ser 10.0
```

### Problema: Banco de dados vazio
```bash
# Solução: Executar update
dotnet ef database update -p src/Ofichina.Infrastructure

# Ou remover e recriar
dotnet ef database drop
dotnet ef database update
```

---

## 📊 Estrutura de Response

```csharp
// Success
{
  "success": true,
  "data": {
	"id": "550e8400-e29b-41d4-a716-446655440000",
	"nome": "Exemplo",
	"createdAt": "2025-01-01T10:00:00Z"
  },
  "message": "Operação realizada com sucesso",
  "errors": []
}

// Error
{
  "success": false,
  "data": null,
  "message": "Erro ao processar requisição",
  "errors": ["Campo nome é obrigatório"]
}
```

---

## 🎯 Do's and Don'ts

### ✅ DO
- Use o Repository Pattern
- Use Unit of Work para transações
- Valide na Application Layer
- Crie DTOs para comunicação
- Use padrão CQRS
- Documente suas mudanças
- Faça testes

### ❌ DON'T
- Não exponha DbContext
- Não misture camadas
- Não retorne Entities na API
- Não coloque lógica em Controllers
- Não ignore o Result Pattern
- Não pule validações
- Não deixe código sem testes

---

## 📞 Ajuda Rápida

**Preciso de:**
- Visão geral → README.md
- Estrutura → MAPA_VISUAL.md
- Técnico → ARQUITETURA.md
- Começar → GUIA_IMPLEMENTACAO.md
- Navegar → INDICE.md

---

## ✅ Checklist Antes de Commitar

- [ ] Código compila sem erros
- [ ] Testes passam (se houver)
- [ ] Segue padrão de código
- [ ] Sem arquivos desnecessários
- [ ] Commit message clara
- [ ] Relacionado com uma issue

---

## 🔗 Links Rápidos

| Recurso | Link |
|---------|------|
| Microsoft Docs | https://docs.microsoft.com |
| Entity Framework | https://docs.microsoft.com/en-us/ef/core/ |
| FluentValidation | https://fluentvalidation.net/ |
| Clean Architecture | https://blog.cleancoder.com/uncle-bob |
| CQRS Pattern | https://martinfowler.com/bliki/CQRS.html |

---

## 🎊 Status

```
✅ Build: Sucesso
✅ Arquitetura: Pronta
✅ Documentação: Completa
✅ Pronto para: Desenvolver
```

---

**Quick Reference v1.0 - 2025**  
*Para referência rápida durante desenvolvimento*
