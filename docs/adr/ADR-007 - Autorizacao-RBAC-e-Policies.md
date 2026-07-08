# ADR-006 - Adoção de Autorização Centralizada com Roles, Policies e FallbackPolicy

-   **Status:** Aceita
-   **Data:** 2026-07-08
-   **Autores:** Equipe de Arquitetura

------------------------------------------------------------------------

# Contexto

A aplicação Ofichina já possui autenticação JWT e emissão de perfis do usuário no token.

O domínio da solução exige controle de acesso por perfil e por tipo de operação, especialmente em endpoints administrativos e em ações sensíveis.

Além disso, a base do projeto segue Clean Architecture e já possui uma camada de Bootstrap para centralizar a composição dos módulos da aplicação.

Era necessário definir uma estratégia de autorização que:

- bloqueasse rotas por padrão;
- permitisse exceções explícitas;
- suportasse controle por roles;
- suportasse policies nomeadas;
- mantivesse a configuração centralizada;
- evitasse regras espalhadas pela aplicação.

------------------------------------------------------------------------

# Problema

Como estruturar a autorização da API para que:

- endpoints fiquem bloqueados por padrão;
- apenas rotas públicas sejam liberadas explicitamente;
- o acesso por perfil seja simples de aplicar;
- regras específicas possam ser descritas por policies;
- a solução permaneça organizada e fácil de evoluir?

------------------------------------------------------------------------

# Decisão

Foi adotado o seguinte modelo de autorização:

1. **JWT com roles**
   - o token carrega os perfis do usuário em `ClaimTypes.Role`;

2. **FallbackPolicy global**
   - qualquer endpoint sem regra explícita exige autenticação;

3. **Policies nomeadas**
   - regras de negócio de acesso são expostas por policies como `usuario.ler`, `usuario.escrever` e `admin.apenas`;

4. **Configuração centralizada**
   - a autorização é registrada em um módulo dedicado, chamado `AddAuthorizationModule`;

5. **Exceções explícitas**
   - endpoints públicos usam `[AllowAnonymous]`;
   - endpoints protegidos usam `[Authorize]`, `[Authorize(Roles = "...")]` ou `[Authorize(Policy = "...")]`.

------------------------------------------------------------------------

# Justificativa

A decisão foi tomada para equilibrar simplicidade, organização e escalabilidade.

## 1. Bloqueio por padrão

A `FallbackPolicy` reduz o risco de expor endpoints sem proteção.
Se um endpoint não for explicitamente liberado, ele fica protegido automaticamente.

Isso evita falhas por esquecimento de anotação.

## 2. Roles para RBAC

Os perfis já fazem parte do modelo de autenticação do sistema.
O uso de roles no JWT permite aplicar RBAC de forma direta, simples e compatível com o ecossistema ASP.NET Core.

Exemplo:

- `ADMIN`
- `USUARIO`

Esse modelo é adequado para permissões amplas.

## 3. Policies para regras nomeadas

Policies permitem nomear regras de autorização de forma clara e legível.

Exemplo:

- `usuario.ler`
- `usuario.escrever`
- `admin.apenas`

Isso melhora a manutenção e facilita entender a intenção de cada regra.

## 4. Configuração centralizada

Concentrar a autorização em um módulo evita duplicação e facilita mudanças futuras.

Sem centralização, a aplicação poderia acabar com regras espalhadas em:

- `Program.cs`
- controllers
- services
- handlers

A centralização melhora o controle e reduz inconsistências.

## 5. Compatibilidade com a arquitetura atual

A solução já usa Bootstrap para composição e já possui um módulo de autenticação.

Adicionar a autorização no mesmo fluxo mantém a arquitetura coerente com o restante da solução.

------------------------------------------------------------------------

# Alternativas Consideradas

## Alternativa 1 - Usar apenas `[Authorize]` nos controllers

### Vantagens

- simples de aplicar;
- baixo esforço inicial.

### Desvantagens

- não expressa regras específicas;
- tende a espalhar a lógica de acesso;
- não resolve bem a necessidade de perfis e permissões distintas.

### Motivo da rejeição

Não atende à necessidade de granularidade e organização.

------------------------------------------------------------------------

## Alternativa 2 - Usar apenas roles no atributo `[Authorize(Roles = "...")]`

### Vantagens

- implementação direta;
- entendimento simples.

### Desvantagens

- pouca expressividade para regras nomeadas;
- difícil evoluir para permissões mais granulares;
- reduz a clareza arquitetural.

### Motivo da rejeição

Funciona para casos simples, mas não cobre bem o cenário de policies nomeadas.

------------------------------------------------------------------------

## Alternativa 3 - Criar um sistema completo de permissões em banco com handlers customizados

### Vantagens

- grande flexibilidade;
- permite autorização altamente granular.

### Desvantagens

- maior complexidade;
- mais esforço de implementação;
- mais custo de manutenção;
- não era necessário neste estágio do projeto.

### Motivo da rejeição

Seria excessivo para a necessidade atual.

------------------------------------------------------------------------

## Alternativa 4 - Centralizar a autorização com JWT, roles, policies e fallback

### Vantagens

- bloqueio por padrão;
- RBAC simples;
- policies nomeadas;
- configuração centralizada;
- menor chance de erro;
- boa evolução futura.

### Desvantagens

- exige disciplina na criação de novas policies;
- pode demandar revisão futura se o modelo de permissões ficar mais complexo.

### Motivo da escolha

Foi a alternativa que melhor equilibra segurança, simplicidade e evolução.

------------------------------------------------------------------------

# Consequências

## Positivas

- endpoints passam a ser protegidos por padrão;
- exceções públicas ficam explícitas;
- roles continuam sendo usadas como RBAC;
- policies deixam a autorização mais legível;
- a configuração fica concentrada em um único módulo;
- a API fica mais segura e organizada;
- a evolução futura para permissões mais granulares fica facilitada.

## Negativas

- maior disciplina é necessária na criação de endpoints;
- cada nova policy precisa ser registrada manualmente;
- desenvolvedores precisam seguir o padrão definido;
- o modelo atual ainda depende de roles no token para as regras principais.

------------------------------------------------------------------------

# Impacto Arquitetural

A decisão impacta principalmente:

- `Ofichina.Authentication`
- `Ofichina.Bootstrap`
- `Ofichina.Api`
- documentação técnica da solução

Também influencia o padrão de criação de novos controllers e actions.

------------------------------------------------------------------------

# Padrão adotado

## Autenticação

O token JWT continua sendo o mecanismo de autenticação.

## RBAC

Roles são o mecanismo principal para controle por perfil.

## Policies

Policies são usadas para nomear e organizar regras de acesso.

## Fallback

A `FallbackPolicy` protege toda a API por padrão.

## Rotas públicas

Somente endpoints com `[AllowAnonymous]` ficam expostos sem autenticação.

------------------------------------------------------------------------

# Exemplo prático

## Endpoint público

```csharp
[AllowAnonymous]
[HttpPost("login")]
public async Task<ActionResult> LoginAsync(...)
{
}
```

## Endpoint protegido por autenticação

```csharp
[Authorize]
[HttpGet]
public async Task<ActionResult> GetAsync()
{
}
```

## Endpoint protegido por role

```csharp
[Authorize(Roles = "ADMIN")]
[HttpDelete("{id}")]
public async Task<ActionResult> DeleteAsync(Guid id)
{
}
```

## Endpoint protegido por policy

```csharp
[Authorize(Policy = "usuario.ler")]
[HttpGet("usuarios")]
public async Task<ActionResult> ListarAsync()
{
}
```

------------------------------------------------------------------------

# Revisão

Esta decisão poderá ser revisada caso a aplicação passe a exigir:

- permissões dinâmicas carregadas do banco;
- políticas compostas por múltiplas regras;
- autorização por tenant;
- autorização baseada em atributos mais avançados;
- um motor próprio de autorização.

------------------------------------------------------------------------

# Histórico

| Data | Alteração | Responsável |
|------|-----------|-------------|
| 2026-07-08 | Documento criado | Equipe de Arquitetura |
