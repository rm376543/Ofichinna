# Autorização por Roles e Policies - Ofichina

## Objetivo

Documentar como o sistema aplica bloqueio de rotas por autenticação, roles e policies, e como criar uma nova policy seguindo o fluxo completo da solução.

## Visão geral da implementação atual

A solução já possui:

- autenticação JWT;
- emissão de roles no token;
- `FallbackPolicy` para exigir usuário autenticado por padrão;
- policies centralizadas no módulo de autorização;
- uso de `[AllowAnonymous]` apenas nos endpoints públicos;
- uso de `[Authorize]`, `[Authorize(Roles = "...")]` e `[Authorize(Policy = "...")]` nos endpoints protegidos.

### Fluxo atual

1. O usuário faz `login`.
2. O sistema autentica e gera um JWT.
3. O JWT carrega os perfis do usuário em `ClaimTypes.Role`.
4. A API valida o token no pipeline.
5. A `FallbackPolicy` bloqueia qualquer endpoint sem liberação explícita.
6. As rotas públicas usam `[AllowAnonymous]`.
7. As rotas protegidas usam `[Authorize]`, `Roles` ou `Policy`.

---

## Conceitos usados

### Role

É o perfil amplo do usuário, por exemplo:

- `ADMIN`
- `USUARIO`
- `GESTOR`

Role é adequada para regras gerais de acesso.

### Policy

É uma regra nomeada de autorização, por exemplo:

- `usuario.ler`
- `usuario.escrever`
- `admin.apenas`

Policy é adequada para regras mais específicas e legíveis.

### FallbackPolicy

É a política aplicada quando um endpoint não possui uma autorização explícita.

Na prática:

- se a action não tiver `[AllowAnonymous]`;
- e não tiver uma policy ou role definida;
- o sistema exige autenticação automaticamente.

---

## Estrutura atual dos arquivos

### `src/Ofichina.Authentication/AuthenticationModule.cs`

Configura o JWT e a validação do token.

### `src/Ofichina.Authentication/AddAuthorizationModule.cs`

Centraliza a configuração de autorização, incluindo:

- `FallbackPolicy`
- policies nomeadas
- regras baseadas em roles

### `src/Ofichina.Bootstrap/DependencyInjection.cs`

Registra os módulos da aplicação.

### `src/Ofichina.Api/Program.cs`

Executa o pipeline HTTP com:

- `UseAuthentication()`
- `UseAuthorization()`

---

## Como criar uma nova policy

A criação de uma nova policy segue este fluxo.

### Passo 1 - Definir a necessidade de acesso

Antes de criar a policy, responda:

- quem pode acessar?
- o acesso é amplo ou específico?
- a regra depende de perfil, permissão ou ambos?

Exemplo:

- qualquer usuário autenticado pode ler;
- apenas `ADMIN` pode excluir;
- apenas quem tem permissão `produto.apagar` pode executar a ação.

---

### Passo 2 - Escolher o modelo da regra

#### Modelo A - baseado em role

Use quando o acesso depender de perfil amplo.

Exemplo:

- `ADMIN`
- `USUARIO`

Regra típica:

- `RequireRole("ADMIN")`

#### Modelo B - baseado em policy

Use quando quiser nomear uma regra específica.

Exemplo:

- `usuario.ler`
- `usuario.escrever`
- `pedido.aprovar`

Regra típica:

- `RequireRole(...)`
- ou `RequireClaim(...)` se houver permissão real no token

---

### Passo 3 - Registrar a policy no módulo de autorização

A policy deve ser adicionada no `AddAuthorizationModule`.

Exemplo de policy baseada em roles:

```csharp
options.AddPolicy("usuario.ler", policy =>
	policy.RequireRole("USUARIO", "ADMIN"));
```

Exemplo de policy baseada em permissão:

```csharp
options.AddPolicy("pedido.aprovar", policy =>
	policy.RequireClaim("permission", "pedido.aprovar"));
```

---

### Passo 4 - Garantir que o token contém os dados necessários

Se a policy usar roles, o JWT já precisa carregar os perfis.

Hoje o projeto já faz isso com:

```csharp
claims.AddRange(perfis.Select(perfil => new Claim(ClaimTypes.Role, perfil)));
```

Se a policy usar permissões reais, o token também deve carregar claims do tipo `permission`.

Exemplo:

```csharp
new Claim("permission", "pedido.aprovar")
```

---

### Passo 5 - Proteger o endpoint

A policy pode ser aplicada em:

- controller inteiro;
- action específica.

#### Controller inteiro

Use quando todas as rotas exigirem autenticação.

```csharp
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ProdutosController : ControllerBase
{
}
```

#### Action específica

Use quando só uma rota precisar de regra especial.

```csharp
[Authorize(Policy = "pedido.aprovar")]
[HttpPost("{id}/aprovar")]
public async Task<IActionResult> AprovarAsync(Guid id)
{
}
```

---

### Passo 6 - Liberar exceções públicas

Endpoints públicos devem usar:

```csharp
[AllowAnonymous]
```

Exemplo:

- `login`
- `register`

---

### Passo 7 - Testar o comportamento

Após criar a policy, validar:

1. sem token → deve negar;
2. com token válido, mas sem role/permissão → deve negar;
3. com token válido e role/permissão correta → deve liberar;
4. endpoint público com `[AllowAnonymous]` → deve liberar.

---

## Exemplo completo de criação de uma nova policy

### Cenário

Criar a policy `produto.excluir`.

### Regra desejada

Somente `ADMIN` pode excluir produtos.

### Implementação esperada no módulo

```csharp
options.AddPolicy("produto.excluir", policy =>
	policy.RequireRole("ADMIN"));
```

### Uso no controller

```csharp
[Authorize(Policy = "produto.excluir")]
[HttpDelete("{id}")]
public async Task<IActionResult> DeleteAsync(Guid id)
{
}
```

### Resultado

- usuário autenticado sem perfil `ADMIN` → bloqueado;
- usuário com perfil `ADMIN` → permitido.

---

## Padrão recomendado de nomenclatura

Usar nomes previsíveis e consistentes:

- `recurso.acao`
- `recurso.leitura`
- `recurso.escrita`
- `recurso.exclusao`

Exemplos:

- `usuario.ler`
- `usuario.escrever`
- `produto.excluir`
- `pedido.aprovar`

---

## Regras de uso recomendadas

- usar `FallbackPolicy` para bloquear por padrão;
- usar `[AllowAnonymous]` somente em endpoints públicos;
- usar `[Authorize]` em controllers protegidos;
- usar `[Authorize(Roles = "...")]` para regras simples;
- usar `[Authorize(Policy = "...")]` para regras nomeadas;
- manter a configuração centralizada no módulo de autorização;
- evitar espalhar regras de acesso pelo `Program.cs`.

---

## Checklist para criar uma nova policy

- [ ] definir a necessidade de acesso;
- [ ] escolher entre role e policy;
- [ ] registrar a policy no módulo de autorização;
- [ ] garantir que o JWT contenha role ou claim necessária;
- [ ] aplicar `[Authorize(Policy = "...")]` ou `[Authorize(Roles = "...")]`;
- [ ] manter `[AllowAnonymous]` apenas em rotas públicas;
- [ ] testar negação e liberação.

---

## Resumo

O padrão adotado no projeto é:

- JWT para autenticação;
- roles para RBAC;
- policies para regras nomeadas;
- fallback para bloqueio global;
- exceções públicas apenas por `[AllowAnonymous]`.

> **Estado verificado em 2026:** os controllers usam `[Authorize]` e `[Authorize(Roles = "ADMIN")]`; login e register usam `[AllowAnonymous]`. `UserPolicyEnum` define nomes disponíveis, mas não há `Authorize(Policy = ...)` ativo nas actions atuais. Os exemplos de policies deste documento representam uma evolução possível.

**Última atualização:** 2026  
**Versão:** 2.0  
**Status:** ✅ RBAC atual documentado
