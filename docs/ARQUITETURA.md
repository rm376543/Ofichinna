# Arquitetura Clean Architecture - Ofichina

## Visão Geral

Este projeto implementa uma arquitetura em camadas (Clean Architecture) com .NET 10, seguindo os princípios SOLID e padrões de design estabelecidos, com uma camada de Bootstrap responsável pela composição da aplicação.

### 0. **Ofichina.Bootstrap** - Camada de Composição
- Orquestra a inicialização da aplicação
- Centraliza a composição dos módulos de Authentication, Application e Infrastructure

**Configuração:**
```csharp
builder.Services.AddBootstrapMiddleware(configuration);
```

## Estrutura de Camadas

### 1. **Ofichina.Api** - Camada de Apresentação
- Controllers
- Middlewares (futuros)
- Handlers de exceções
- Autenticação e Autorização
- Integração com SwaggerModule

**Referências:**
- Ofichina.Application
- Ofichina.Authentication

**Configuração:**
```csharp
builder.Services.AddBootstrapMiddleware(builder.Configuration);
```

### 2. **Ofichina.Contracts** - Camada de Contratos
- DTOs de Requisição (CreateRequest, UpdateRequest)
- DTOs de Resposta (ApiResponse, ApiResponse<T>)
- Paginação (PaginationDto, PagedResult<T>)
- DTOs de Entidades (BaseEntityDto)

**Uso:**
Contratos entre API e Application Layer

### 3. **Ofichina.Authentication** - Camada de Autenticação
Centraliza a configuração e as regras de autenticação da solução.

**Submódulos:**
- **AddAuthenticationModule.cs**: configura JWT Bearer no pipeline
- **AddAuthorizationModule.cs**: registra policies e fallback policy
- **AuthenticationServicesModule.cs**: registra serviços, validadores e contratos de autenticação
- **AuthorizationResultHandlerModule.cs**: customiza a resposta de autorização

**Estrutura:**
```
Authentication/
├── Abstractions/
├── DependencyInjection/
├── Security/
├── Services/
└── Validators/
```

### 4. **Ofichina.Application** - Camada de Aplicação
Coordena e orquestra a lógica de negócio

**Submódulos:**
- **ValidationModule**: Validadores FluentValidation
  - Registra validadores do assembly automaticamente

- **HandlersModule**: Handlers CQRS (Commands e Queries)
  - Abstrações: ICommand<T>, IQuery<T>
  - Interfaces: ICommandHandler<T>, IQueryHandler<T>
  - Exemplo: handlers de autenticação e de perfil

- **ServicesModule**: Serviços da aplicação
  - Lógica de orquestração
  - Exemplo: services.AddApplicationServices()

**Estrutura de Use Cases:**
```
Application/
├── Abstractions/
│   ├── Contracts.cs (ICommand, IQuery)
│   └── Handlers.cs (ICommandHandler, IQueryHandler)
├── Validators/
│   └── Validators de autenticação e perfis
├── UseCases/
│   ├── Autenticacao/
│   └── Perfis/
└── DependencyInjection/
	├── ApplicationModule.cs (orquestra tudo)
	├── ValidationModule.cs
	├── HandlersModule.cs
	└── ServicesModule.cs
```

### 5. **Ofichina.Domain** - Camada de Domínio
Define as regras de negócio e entidades

**Estrutura:**
```
Domain/
├── Entities/
│   ├── Entity.cs (classe base)
│   └── Entidades de usuário e perfil
├── Interfaces/
│   ├── IRepository.cs (interface genérica)
│   └── IUnitOfWork.cs
├── Specifications/
│   └── Specification.cs (Specification Pattern)
└── Shared/
	├── ValueObject.cs (objetos de valor)
	└── Result.cs (Result Pattern)
```

**Características:**
- Entity base com Id (Guid), CreatedAt, UpdatedAt e DeletedAt
- Repositórios genéricos e específicos
- Padrão Unit of Work
- Specification Pattern para queries complexas
- Result Pattern para resultados de operação

### 6. **Ofichina.Infrastructure** - Camada de Infraestrutura
Implementação de detalhes técnicos

**Submódulos:**
- **DatabaseModule**: Configuração EF Core
  - SQL Server como banco de dados
  - Conexão via DefaultConnection

- **RepositoryModule**: Implementação de repositórios
  - Repository<T> genérico
  - UnitOfWork
  - Repositórios concretos de usuário e perfil
  - Repositório de autenticação para consulta de usuários

- **ServicesModule**: Serviços de apoio à autenticação
  - PerfilAutorizacaoService

- **InfrastructureServicesModule**: Serviços externos
  - Email, SMS, Storage (futuros)

**Estrutura:**
```
Infrastructure/
├── Modules/
│   ├── InfrastructureModule.cs (orquestra)
│   ├── DatabaseModule.cs
│   ├── RepositoryModule.cs
│   └── ServicesModule.cs
├── Persistence/
│   └── ApplicationDbContext.cs
└── Repositories/
	├── Repository.cs (genérico)
	├── UnitOfWork.cs
	└── Repositórios concretos de usuário e perfil
```

## Fluxo de Dependência

```
Program.cs
	↓
AddBootstrapMiddleware(configuration)
	↓
BootstrapMiddleware
	├── AddAuthorizationModule()
	├── AddAuthenticationModules(configuration)
	├── AddAuthorizationResultHandlerModule()
	├── AddAuthenticationServices()
	├── AddApplication()
	└── AddInfrastructure(configuration)
```

## Padrões Implementados

### 1. **CQRS (Command Query Responsibility Segregation)**
Separação entre operações de leitura (Queries) e escrita (Commands)
- Handlers específicos para cada operação
- Exemplo: handlers de autenticação e de perfil

### 2. **Specification Pattern**
Encapsulamento de critérios de consulta complexos
- Classe base: Specification<T>
- Reutilizável em diferentes contextos

### 3. **Repository Pattern**
Abstração de acesso a dados
- IRepository<T> genérica
- Implementações específicas por entidade

### 4. **Unit of Work Pattern**
Gerenciamento de transações e coordenação de repositórios
- Confirmação/reversão de operações
- Isolamento de transações

### 5. **Result Pattern**
Encapsulamento de resultados de operação com sucesso/erro
- Tipo: Result<T>
- Mensagens de erro e validação

### 6. **Value Object Pattern**
Objetos imutáveis identificados por seu conteúdo
- Classe base: ValueObject
- Implementação de Equals e GetHashCode

### 7. **Clean Architecture**
Independência de frameworks e UI
- Regras de negócio no Domain
- Orquestração na Application
- Detalhes técnicos na Infrastructure

## Dependências de NuGet

- **Microsoft.EntityFrameworkCore**: 10.0.9
- **Microsoft.EntityFrameworkCore.SqlServer**: 10.0.9
- **FluentValidation**: 11.9.2
- **FluentValidation.DependencyInjectionExtensions**: 11.9.2
- **Microsoft.Extensions.Configuration.Abstractions**: 10.0.9
- **Microsoft.Extensions.DependencyInjection.Abstractions**: 10.0.9

## Como Usar - Exemplo Prático

### 1. Criar uma Entidade no Domain

```csharp
public class Cliente : Entity
{
	public string Nome { get; set; }
	public string Email { get; set; }

	public Cliente(string nome, string email)
	{
		Nome = nome;
		Email = email;
	}
}
```

### 2. Criar um Repositório Específico

```csharp
public interface IClienteRepository : IRepository<Cliente>
{
	Task<Cliente?> GetByEmailAsync(string email);
}

public class ClienteRepository : Repository<Cliente>, IClienteRepository
{
	public async Task<Cliente?> GetByEmailAsync(string email)
	{
		return await _context.Set<Cliente>()
			.FirstOrDefaultAsync(c => c.Email == email);
	}
}
```

### 3. Implementar um Handler CQRS

```csharp
public class CreateClienteCommandHandler : ICommandHandler<CreateClienteCommand, Guid>
{
	public async Task<Guid> HandleAsync(CreateClienteCommand command)
	{
		var cliente = new Cliente(command.Nome, command.Email);
		await _repository.AddAsync(cliente);
		await _unitOfWork.SaveChangesAsync();
		return cliente.Id;
	}
}

## Notas Importantes

- **Configuração de Banco de Dados**: Adicionar connection string em `appsettings.json`
- **Migrations**: Usar `dotnet ef migrations add` para criar migrations
- **FluentValidation**: Validadores são autodiscovered do assembly
- **Unit of Work**: Sempre usar SaveChangesAsync() no handler
- **Lazy Loading**: Usar `AsNoTracking()` para queries de leitura

---

Arquitetura implementada em 2025 seguindo Clean Architecture com .NET 10