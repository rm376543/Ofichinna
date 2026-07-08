# ADR-008 - Estratégia para Validações com FluentValidation

- **Status:** Aceita
- **Data:** 2026-07-05
- **Autores:** Equipe de Arquitetura

---

# Contexto

A aplicação adota os princípios da Clean Architecture, Domain-Driven Design (DDD) e CQRS, organizando os casos de uso na camada **Application**.

Cada operação exposta pela aplicação recebe dados provenientes da camada **Presentation**, sendo necessário validar essas informações antes da execução dos respectivos casos de uso.

Além disso, é importante distinguir validações relacionadas ao formato e consistência dos dados das regras de negócio pertencentes ao domínio.

---

# Decisão

Foi adotada uma estratégia de validação baseada em **Validators** implementados na camada **Application**, utilizando a biblioteca **FluentValidation** como sua implementação.

Cada **Command** e **Query** poderá possuir um **Validator** responsável pela validação dos dados de entrada antes da execução do respectivo Handler.

Os Validators serão responsáveis apenas pela validação estrutural e sintática dos dados recebidos.

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

Os Validators **não deverão** conter:

- Regras de negócio;
- Alterações de estado;
- Persistência de dados;
- Acesso a infraestrutura;
- Decisões de domínio.

---

# Justificativa

A adoção desta estratégia foi motivada pelos seguintes fatores.

## Separação de Responsabilidades

As validações de entrada permanecem separadas da lógica dos casos de uso e das regras de negócio.

Isso mantém os Handlers menores e mais coesos.

---

## Organização

Cada caso de uso possui seu próprio Validator, facilitando a manutenção e localização das regras de validação.

---

## Legibilidade

A API fluente do FluentValidation permite definir regras de maneira clara, expressiva e de fácil compreensão.

---

## Testabilidade

Os Validators podem ser testados isoladamente, sem necessidade de executar os respectivos Handlers.

---

## Padronização

Toda validação estrutural da aplicação seguirá uma abordagem única, tornando o comportamento da solução consistente.

---

# Alternativas Consideradas

## Data Annotations

### Vantagens

- Simplicidade de utilização;
- Recursos nativos da plataforma .NET.

### Desvantagens

- Acoplamento entre validações e modelos;
- Menor flexibilidade;
- Dificuldade para regras mais elaboradas.

### Motivo da rejeição

Não oferece a flexibilidade desejada para uma arquitetura baseada em Clean Architecture e CQRS.

---

## Validações nos Handlers

### Vantagens

- Menor quantidade de classes.

### Desvantagens

- Mistura validação com lógica do caso de uso;
- Redução da legibilidade;
- Dificulta testes unitários;
- Viola o princípio da responsabilidade única.

### Motivo da rejeição

Os Handlers devem ser responsáveis apenas pela execução dos casos de uso.

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
- Curva de aprendizado da biblioteca FluentValidation.

---

# Impacto Arquitetural

A adoção desta decisão estabelece que:

- Todo Command poderá possuir um Validator.
- Toda Query poderá possuir um Validator.
- Os Validators deverão permanecer exclusivamente na camada Application.
- Os contratos utilizados pelos Validators pertencem à camada Contracts.
- Os Validators não deverão implementar regras de domínio.
- Os Validators deverão ser executados antes da execução dos respectivos Handlers.

---

# Revisão

Esta decisão poderá ser revisada caso:

- Seja adotada outra estratégia de validação;
- A arquitetura da solução seja alterada significativamente;
- A evolução do ecossistema .NET apresente uma alternativa mais aderente aos requisitos do projeto.