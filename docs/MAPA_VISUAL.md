# 🏗️ Mapa Visual - Clean Architecture Ofichinna

## Estrutura Hierárquica Completa

```
📦 SOLUÇÃO OFICHINNA
│
├─ 🧩 BOOTSTRAP LAYER (Ofichina.Bootstrap)
│  │  Responsabilidade: Compor a aplicação e orquestrar a inicialização
│  │
│  ├─ DependencyInjection.cs
│  │  └─ AddBootstrapMiddleware(configuration)
│  │
│  └─ Referências: Authentication, Application, Infrastructure
│
├─ 🌐 API LAYER (Ofichina.Api)
│  │  Responsabilidade: Receber requisições HTTP
│  │
│  ├─ Program.cs
│  │  └─ builder.Services.AddBootstrapMiddleware(configuration)
│  │
│  ├─ Controllers/
│  │  └─ [Controllers a serem criados]
│  │
│  ├─ Modules/
│  │  └─ SwaggerModule.cs ✅
│  │
│  └─ Referências: Bootstrap, Application, Contracts
│
├─ 📋 CONTRACTS LAYER (Ofichina.Contracts)
│  │  Responsabilidade: Definir contratos entre API e Application
│  │
│  ├─ Requests/ ✅
│  │  └─ BaseRequest.cs
│  │     • CreateRequest
│  │     • UpdateRequest
│  │
│  ├─ Responses/ ✅
│  │  └─ ApiResponse.cs
│  │     • ApiResponse (sem dados)
│  │
├─ 🔐 AUTHENTICATION LAYER (Ofichinna.Authentication)
│  │  Responsabilidade: Regras, serviços e configuração JWT de autenticação
│  │
│  ├─ AuthenticationModule.cs
│  ├─ DependencyInjection/
│  │  └─ AuthenticationServicesModule.cs
│  ├─ Abstractions/
│  ├─ Services/
│  ├─ Security/
│  └─ Validators/
│  │     • ApiResponse<T> (com dados)
│  │
│  ├─ DTOs/ ✅
│  │  ├─ BaseEntityDto.cs
│  │  │  • Id, CreatedAt, UpdatedAt
│  │  │
│  │  └─ PaginationDto.cs
│  │     • PaginationDto
│  │     • PagedResult<T>
│  │
│  └─ Referências: Nenhuma
│
├─ 🔧 APPLICATION LAYER (Ofichina.Application)
│  │  Responsabilidade: Orquestrar lógica de negócio
│  │
│  ├─ DependencyInjection/ ✅
│  │  ├─ ApplicationModule.cs (ORQUESTRA TUDO)
│  │  │  • AddValidations()
│  │  │  • AddHandlers()
│  │  │  • AddApplicationServices()
│  │  │  • AddInfrastructure()
│  │  │
│  │  ├─ ValidationModule.cs
│  │  │  • FluentValidation autodiscovery
│  │  │
│  │  ├─ HandlersModule.cs
│  │  │  • Registra handlers (pronto para MediatR)
│  │  │
│  │  └─ ServicesModule.cs
│  │     • Serviços da aplicação
│  │
│  ├─ Abstractions/ ✅
│  │  ├─ Contracts.cs
│  │  │  • ICommand<TResponse>
│  │  │  • IQuery<TResponse>
│  │  │
│  │  └─ Handlers.cs
│  │     • ICommandHandler<TCommand, TResponse>
│  │     • IQueryHandler<TQuery, TResponse>
│  │
│  ├─ Validators/ ✅
│  │  └─ CreateExemploRequestValidator.cs
│  │     • AbstractValidator<CreateRequest>
│  │     • Auto-registrado pelo FluentValidation
│  │
│  ├─ UseCases/
│  │  └─ Exemplo/ ✅
│  │     ├─ Commands/
│  │     │  └─ CreateExemploCommand.cs
│  │     │     • Implementa ICommand<Guid>
│  │     │     • Nome, Descricao
│  │     │
│  │     ├─ Queries/
│  │     │  └─ GetExemploByIdQuery.cs
│  │     │     • Implementa IQuery<GetExemploByIdResponse?>
│  │     │     • GetExemploByIdResponse (DTO resposta)
│  │     │
│  │     └─ Handlers/
│  │        ├─ CreateExemploCommandHandler.cs
│  │        │  • ICommandHandler<CreateExemploCommand, Guid>
│  │        │  • Usa IExemploRepository
│  │        │  • Usa IUnitOfWork
│  │        │
│  │        └─ GetExemploByIdQueryHandler.cs
│  │           • IQueryHandler<GetExemploByIdQuery, GetExemploByIdResponse?>
│  │           • Usa IExemploRepository
│  │
│  └─ Referências: Domain, Infrastructure, Contracts
│
├─ 🎯 DOMAIN LAYER (Ofichina.Domain)
│  │  Responsabilidade: Regras de negócio (NUNCA depende de outros layers)
│  │
│  ├─ Entities/ ✅
│  │  ├─ Entity.cs (BASE)
│  │  │  • Id: Guid (chave primária)
│  │  │  • CreatedAt: DateTime
│  │  │  • UpdatedAt: DateTime?
│  │  │  • Equals() e GetHashCode() por Id
│  │  │
│  │  └─ Exemplo.cs
│  │     • Herda de Entity
│  │     • Nome: string
│  │     • Descricao: string?
│  │     • Ativo: bool
│  │
│  ├─ Interfaces/ ✅
│  │  ├─ IRepository.cs (GENÉRICA)
│  │  │  • AddAsync(TEntity)
│  │  │  • GetByIdAsync(Guid)
│  │  │  • GetAllAsync()
│  │  │  • UpdateAsync(TEntity)
│  │  │  • DeleteAsync(TEntity)
│  │  │
│  │  ├─ IUnitOfWork.cs
│  │  │  • SaveChangesAsync()
│  │  │  • BeginTransactionAsync()
│  │  │  • CommitTransactionAsync()
│  │  │  • RollbackTransactionAsync()
│  │  │
│  │  └─ IExemploRepository.cs (ESPECÍFICA)
│  │     • IRepository<Exemplo>
│  │     • GetByNameAsync(string)
│  │     • GetAllAtivosAsync()
│  │
│  ├─ Specifications/ ✅
│  │  └─ Specification.cs
│  │     • Criteria: Expression<Func<T, bool>>
│  │     • Includes: List<Expression>
│  │     • OrderBy / OrderByDescending
│  │     • Paginação (Take, Skip)
│  │
│  ├─ Shared/ ✅
│  │  ├─ Result.cs
│  │  │  • Result (sucesso/erro)
│  │  │  • Result<T> (sucesso/erro com valor)
│  │  │  • Métodos estáticos: Success(), Failure()
│  │  │
│  │  └─ ValueObject.cs
│  │     • Classe base imutável
│  │     • Identificação por valor
│  │     • Equals() e GetHashCode()
│  │
│  └─ Referências: NENHUMA (completamente independente)
│
└─ 🔌 INFRASTRUCTURE LAYER (Ofichina.Infrastructure)
   │  Responsabilidade: Detalhes técnicos de implementação
   │
   ├─ Modules/ ✅
   │  ├─ InfrastructureModule.cs (ORQUESTRA)
   │  │  • AddDatabase()
   │  │  • AddRepositories()
   │  │  • AddInfrastructureServices()
   │  │
   │  ├─ DatabaseModule.cs
   │  │  • Configura DbContext
   │  │  • SQL Server como provider
   │  │  • Lê connection string de appsettings
   │  │
   │  ├─ RepositoryModule.cs
   │  │  • Registra Repository<T> (genérico)
   │  │  • Registra UnitOfWork
   │  │  • Registra ExemploRepository (específico)
   │  │
   │  └─ ServicesModule.cs
   │     • Email (futuro)
   │     • SMS (futuro)
   │     • Storage (futuro)
   │
   ├─ Persistence/ ✅
   │  └─ ApplicationDbContext.cs
   │     • DbSet<Exemplo> Exemplos
   │     • OnModelCreating() para configurações
   │
   ├─ Repositories/ ✅
   │  ├─ Repository.cs (GENÉRICO)
   │  │  • Implementa IRepository<T>
   │  │  • AddAsync, GetByIdAsync, GetAllAsync, UpdateAsync, DeleteAsync
   │  │
   │  ├─ UnitOfWork.cs
   │  │  • Implementa IUnitOfWork
   │  │  • Gerencia transações
   │  │  • IAsyncDisposable
   │  │
   │  └─ ExemploRepository.cs (ESPECÍFICO)
   │     • Herda de Repository<Exemplo>
   │     • Implementa IExemploRepository
   │     • GetByNameAsync()
   │     • GetAllAtivosAsync()
   │
   └─ Referências: Domain
```

---

## 📍 Fluxo de Requisição

```
┌─────────────────────────────────────────────────────────────────┐
│ 1. Cliente HTTP faz requisição POST /api/exemplos               │
└─────────────────────────┬───────────────────────────────────────┘
						  │
		┌─────────────────┴─────────────────┐
		│                                   │
		▼                                   ▼
┌──────────────────────────┐      ┌──────────────────────────┐
│ 2. ExemplosController    │      │ 5. ValidationModule      │
│    - Recebe requisição   │──────│    - Auto-executa        │
│    - Valida entrada      │      │    - CreateExemploRequest│
└──────────────────────────┘      │      Validator           │
		│                         └──────────────────────────┘
		▼
┌──────────────────────────┐
│ 3. CreateExemploCommand  │
│    - Encapsula dados     │
│    - ICommand<Guid>      │
└──────────────────────────┘
		│
		▼
┌───────────────────────────────┐
│ 4. CreateExemploCommandHandler│
│    - Executa lógica           │
│    - Chama repository         │
└───────────────────────────────┘
		│
		├─────────────┬────────────────┐
		│             │                │
		▼             ▼                ▼
┌──────────────┐ ┌────────────┐ ┌──────────────┐
│ IExemplo     │ │ IUnitOfWork│ │ ApplicationDb│
│ Repository   │ │            │ │ Context      │
│ AddAsync()   │ │ SaveChanges│ │ SaveChanges()│
└──────────────┘ └────────────┘ └──────────────┘
		│             │                │
		└─────────────┼────────────────┘
					  ▼
			┌───────────────────┐
			│   SQL Server      │
			│   INSERT Exemplo  │
			└───────────────────┘
					  │
		┌─────────────┴─────────────┐
		│                           │
		▼                           ▼
┌──────────────────┐      ┌──────────────────┐
│ Retorna Guid Id  │      │ ApiResponse<Guid>│
│ (sucesso)        │──────│ (Status 201)     │
└──────────────────┘      └──────────────────┘
```

---

## 🔄 Ciclo de Inicialização

```
					┌─────────────────────┐
					│   Program.cs        │
					│   Main()            │
					└──────────┬──────────┘
							   │
				┌──────────────┴──────────────┐
				│                             │
		┌───────▼────────┐            ┌──────▼─────────┐
		│ Criar builder  │            │ Adicionar      │
		│ WebApplication │────────────│ Controllers    │
		└────────────────┘            └────────────────┘
				│
				▼
		┌───────────────────────┐
		│ builder.Services.Add  │
		│ Application(config)   │ ◄─── PONTO DE ENTRADA
		└───────────┬───────────┘
					│
		┌───────────▼────────────┐
		│ ApplicationModule      │
		│ .AddApplication()      │
		└───────────┬────────────┘
					│
	  ┌─────────────┼─────────────┐
	  │             │             │
	  ▼             ▼             ▼
┌──────────────┐ ┌────────────┐ ┌─────────────────┐
│ Validation   │ │ Handlers   │ │ App Services    │
│ Module       │ │ Module     │ │ Module          │
└──────────────┘ └────────────┘ └─────────────────┘
	  │             │             │
	  └─────────────┼─────────────┘
					│
					▼
		┌───────────────────────────┐
		│ InfrastructureModule      │
		│ .AddInfrastructure(config)│
		└───────────┬───────────────┘
					│
	  ┌─────────────┼─────────────┐
	  │             │             │
	  ▼             ▼             ▼
┌──────────────┐ ┌────────────┐ ┌─────────────────┐
│ Database     │ │ Repositories
│ Module       │ │ Module     │ │ Services Module │
│ (EF Core)    │ │            │ │ (Email, SMS...)  │
└──────────────┘ └────────────┘ └─────────────────┘
	  │             │
	  └─────────────┼──────────────┐
					│              │
					▼              ▼
		 ┌────────────────────┐  ┌────────────────┐
		 │ DbContext          │  │ Repository<T>  │
		 │ + SQL Server       │  │ + UnitOfWork   │
		 └────────────────────┘  │ + Specific Repos
								 └────────────────┘
									  │
									  ▼
							┌──────────────────┐
							│ Aplicação Pronta │
							│ para Requisições │
							└──────────────────┘
```

---

## 🎓 Mapeamento de Responsabilidades

```
┌──────────────────────────────────────────────────────────────────┐
│ DOMAIN LAYER - Regras de Negócio                                 │
├──────────────────────────────────────────────────────────────────┤
│ ✓ Definir entidades e suas regras                                │
│ ✓ Definir interfaces de repositório (contrato)                   │
│ ✓ Definir especificações para queries                            │
│ ✓ NÃO deve conhecer Entity Framework                             │
│ ✓ NÃO deve conhecer banco de dados                               │
│ ✓ NÃO deve fazer chamadas HTTP                                   │
└──────────────────────────────────────────────────────────────────┘

┌──────────────────────────────────────────────────────────────────┐
│ APPLICATION LAYER - Orquestração                                 │
├──────────────────────────────────────────────────────────────────┤
│ ✓ Implementar use cases (Commands e Queries)                     │
│ ✓ Validar entrada de dados                                       │
│ ✓ Orquestrar chamadas a repositórios                             │
│ ✓ Aplicar regras de negócio complexas                            │
│ ✓ NÃO deve conhecer como dados são persistidos                   │
│ ✓ NÃO deve conhecer detalhes HTTP                                │
│ ✓ NÃO deve conhecer banco de dados específico                    │
└──────────────────────────────────────────────────────────────────┘

┌──────────────────────────────────────────────────────────────────┐
│ INFRASTRUCTURE LAYER - Implementação Técnica                     │
├──────────────────────────────────────────────────────────────────┤
│ ✓ Implementar repositórios (concrete classes)                    │
│ ✓ Configurar Entity Framework                                    │
│ ✓ Gerenciar conexões com banco                                   │
│ ✓ Implementar serviços externos (Email, SMS)                     │
│ ✓ Lógica de cache                                                │
│ ✓ NÃO deve conter lógica de negócio                              │
│ ✓ NÃO deve ser conhecida diretamente pela API                    │
└──────────────────────────────────────────────────────────────────┘

┌──────────────────────────────────────────────────────────────────┐
│ API LAYER - Apresentação                                         │
├──────────────────────────────────────────────────────────────────┤
│ ✓ Receber requisições HTTP                                       │
│ ✓ Retornar respostas HTTP                                        │
│ ✓ Chamar handlers (Commands/Queries)                             │
│ ✓ Mapear DTOs para respostas                                     │
│ ✓ Gerenciar autenticação/autorização                             │
│ ✓ NÃO deve conter lógica de negócio                              │
│ ✓ NÃO deve conhecer detalhes de persistência                     │
└──────────────────────────────────────────────────────────────────┘

┌──────────────────────────────────────────────────────────────────┐
│ CONTRACTS LAYER - Comunicação                                    │
├──────────────────────────────────────────────────────────────────┤
│ ✓ Definir DTOs de entrada (Requests)                             │
│ ✓ Definir DTOs de saída (Responses)                              │
│ ✓ Definir estrutura de dados compartilhada                       │
│ ✓ NÃO deve conter lógica                                         │
│ ✓ NÃO deve conter regras de negócio                              │
└──────────────────────────────────────────────────────────────────┘
```

---

## 📊 Matriz de Dependências

```
			  Domain  Application  Infrastructure  API  Contracts
Domain          ✗         ✗            ✓           ✗     ✓
Application     ✓         ✗            ✓           ✗     ✓
Infrastructure  ✓         ✗            ✗           ✗     ✗
API             ✗         ✓            ✗           ✗     ✓
Contracts       ✗         ✓            ✗           ✓     ✗

Legenda:
✓ = Pode referenciar
✗ = NÃO pode referenciar (Inversão de Dependência)
```

---

## 🎯 Exemplo de Extensão - Adicionar Nova Entidade

Para adicionar uma nova entidade (ex: Produto), siga esta sequência:

```
1️⃣  DOMAIN
	├─ Criar Entity: Produto(nome, preco)
	├─ Criar Interface: IProductoRepository
	└─ Status: ✅ Pronto para qualquer implementação

2️⃣  CONTRACTS
	├─ Criar CreateProdutoRequest
	├─ Criar GetProdutoResponse
	└─ Status: ✅ Contrato definido

3️⃣  APPLICATION
	├─ Criar CreateProdutoCommand
	├─ Criar GetProdutoByIdQuery
	├─ Criar CreateProdutoCommandHandler
	├─ Criar GetProdutoByIdQueryHandler
	├─ Criar CreateProdutoRequestValidator
	└─ Status: ✅ Use cases prontos

4️⃣  INFRASTRUCTURE
	├─ Criar ProdutoRepository (implementa IProductoRepository)
	├─ Registrar em RepositoryModule
	├─ Adicionar DbSet<Produto> em ApplicationDbContext
	└─ Status: ✅ Persistência pronta

5️⃣  API
	├─ Criar ProdutosController
	├─ Injetar handlers
	├─ Criar endpoints
	└─ Status: ✅ API pronta
```

---

## 💡 Dicas de Desenvolvimento

```
✓ FAÇA:
  • Coloque lógica de negócio no Domain
  • Use Repository para persistência
  • Crie Commands para escrita, Queries para leitura
  • Valide sempre na Application Layer
  • Use DTOs para comunicação entre layers

✗ NÃO FAÇA:
  • Não misture lógica de diferentes layers
  • Não deixe controllers com lógica complexa
  • Não passe entidades do Domain direto para API
  • Não use DbContext fora da Infrastructure
  • Não ignore o Result Pattern
```

---

**Última atualização:** 2026  
**Versão:** 2.0  
**Status:** ✅ Arquitetura sincronizada com API, módulos e middlewares atuais
