# ADR-006 - Estratégia para Validações com FluentValidation

- **Status:** Aceita
- **Data:** 2026-07-05
- **Autores:** Equipe de Arquitetura

---

# Contexto

A aplicação adota Clean Architecture, Domain-Driven Design (DDD) e CQRS, organizando os casos de uso na camada **Application**.

Cada operação exposta pela aplicação recebe dados provenientes da camada **Presentation**, sendo necessário validar essas informações antes da execução dos respectivos casos de uso.

Além disso, é importante distinguir validações relacionadas ao formato e consistência dos dados das regras de negócio pertencentes ao domínio.

---

# Decisão

Foi adotado o **FluentValidation** como biblioteca padrão para validação dos objetos de entrada da aplicação.

Sua utilização ficará restrita à camada **Application**, onde cada **Command** e **Query** poderá possuir um **Validator** responsável pela validação dos dados recebidos.

As validações ocorrerão antes da execução dos respectivos Handlers.

As regras de negócio permanecerão implementadas exclusivamente na camada **Domain**, não sendo responsabilidade dos Validators.

---

# Escopo das Validações

Os Validators poderão validar, entre outros aspectos:

- Campos obrigatórios;
- Tamanho mínimo e máximo;
- Formato de e-mail;
- Formato de documentos;
- Valores permitidos;
- Intervalos numéricos;
- Datas válidas;
- Consistência entre propriedades do mesmo objeto.

Não deverão ser implementadas nos Validators:

- Regras de negócio;
- Decisões de domínio;
- Alterações de estado;
- Persistência de dados.

---

# Justificativa

A adoção do FluentValidation foi motivada pelos seguintes fatores.

## Separação de responsabilidades

As validações de entrada permanecem separadas das regras de negócio.

Isso evita que os Handlers acumulem responsabilidades adicionais.

---

## Organização

Cada caso de uso possui seu próprio Validator, facilitando a localização e manutenção das validações.

---

## Legibilidade

A API fluente do FluentValidation permite definir regras de forma clara e expressiva.

---

## Reutilização

As regras de validação podem ser reutilizadas quando necessário, reduzindo duplicação de código.

---

## Testabilidade

Os Validators podem ser testados isoladamente, sem dependência da lógica dos Handlers.

---

# Alternativas Consideradas

## Data Annotations

### Vantagens

- Implementação simples.
- Recursos nativos da plataforma .NET.

### Desvantagens

- Acoplamento entre validações e modelos.
- Menor flexibilidade para regras complexas.
- Baixa reutilização.

### Motivo da rejeição

Não oferece a flexibilidade necessária para uma arquitetura baseada em CQRS e Clean Architecture.

---

## Validações nos Handlers

### Vantagens

- Menor quantidade de classes.

### Desvantagens

- Mistura validação com lógica do caso de uso.
- Redução da legibilidade.
- Dificulta testes unitários.

### Motivo da rejeição

Viola o princípio da responsabilidade única (SRP).

---

# Consequências

## Positivas

- Padronização das validações.
- Melhor organização da camada Application.
- Handlers menores e mais focados.
- Facilidade para testes automatizados.
- Código mais legível.
- Separação clara entre validação e regras de negócio.

---

## Negativas

- Maior quantidade de classes.
- Necessidade de manter Validators sincronizados com os contratos da aplicação.
- Curva de aprendizado da biblioteca.

---

# Impacto Arquitetural

A adoção desta decisão estabelece que:

- Todo Command poderá possuir um Validator.
- Toda Query poderá possuir um Validator.
- Os Validators deverão permanecer na camada Application.
- Os contratos utilizados pelos Validators pertencem à camada Contracts.
- Os Validators não deverão conter regras de domínio.
- A execução dos Validators deverá ocorrer antes da execução dos respectivos Handlers.

---

# Revisão

Esta decisão poderá ser revisada caso:

- Seja adotada outra estratégia de validação mais adequada ao contexto da aplicação;
- A arquitetura da solução seja alterada significativamente;
- A evolução do ecossistema .NET apresente alternativas mais aderentes aos requisitos do projeto.