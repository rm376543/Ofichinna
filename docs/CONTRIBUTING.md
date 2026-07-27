# Guia de Contribuição e Documentação - Ofichinna

## Objetivo

Definir o padrão mínimo para contribuir com código e documentação sem perder alinhamento entre arquitetura, Swagger e textos de apoio.

## Regras principais

1. Atualize a documentação sempre que adicionar ou alterar endpoint, contrato, policy ou módulo de DI.
2. Prefira comentários XML em membros públicos expostos por API ou usados pelo Swagger.
3. Mantenha `ProducesResponseType` e exemplos de resposta coerentes com os contratos reais.
4. Documente mudanças estruturais em `README.md`, `INDICE.md` e, quando necessário, em `SUMARIO_EXECUTIVO.md`.
5. Adicione ou atualize exemplos em `API_REFERENCE.md` para qualquer endpoint novo.

## Design Approval Sheet (DAS)

Features de negócio significativas exigem um DAS aprovado antes do início da implementação em `src/`. Considere significativa qualquer alteração que envolva uma nova entidade ou agregado, mudança de regra de negócio, novo fluxo de estado, contrato público, novo endpoint, persistência ou migration, integração externa ou autorização.

### Fluxo obrigatório

1. Copiar `docs/DESIGN_APPROVAL_SHEET.md` para `docs/das/DAS-XXX-nome-da-feature.md`.
2. Preencher contexto, escopo, requisitos, design por camada, segurança, testes, riscos e critérios de aceite.
3. Relacionar os ADRs aplicáveis e indicar quando uma nova decisão arquitetural exige ADR próprio.
4. Submeter o DAS para revisão técnica, segurança e produto conforme o impacto.
5. Registrar o status `APROVADO` ou `APROVADO_COM_RESSALVAS` e as assinaturas antes de implementar.
6. Atualizar o DAS durante a implementação; mudanças relevantes devem gerar nova versão e nova aprovação.
7. Ao concluir, registrar evidências de build, testes, Swagger, SonarQube e documentação.

### DAS e ADRs

- **ADR:** registra uma decisão arquitetural duradoura e transversal, como Clean Architecture, CQRS, EF Core ou RBAC.
- **DAS:** registra o design de uma feature específica, seus contratos, fluxos, impacto, riscos e critérios de aprovação.
- Uma feature pode referenciar vários ADRs, mas o DAS não substitui um ADR quando a decisão altera a arquitetura da solução.

O template está em [`DESIGN_APPROVAL_SHEET.md`](./DESIGN_APPROVAL_SHEET.md) e exemplos preenchidos ficam em [`das/`](./das/).

## Antes de abrir PR

- Executar `dotnet build`.
- Executar os testes relevantes.
- Validar o Swagger localmente.
- Revisar se houve impacto em documentação de apoio.
- Confirmar que o DAS da feature está aprovado e atualizado.

## Onde documentar cada tipo de mudança

- **Nova feature de API**: `API_REFERENCE.md`, `README.md`, `QUICK_REFERENCE.md`.
- **Mudança de arquitetura**: `ARQUITETURA.md`, `MAPA_VISUAL.md`, `SUMARIO_EXECUTIVO.md`.
- **Mudança operacional**: `TROUBLESHOOTING.md`, `START_HERE.md`, `QUICK_REFERENCE.md`.
- **Mudança de autorização**: `AUTORIZACAO-RBAC-POLICIES.md`.
- **Feature significativa**: criar e aprovar um DAS em `das/` antes da implementação.

## Padrão para novos endpoints

- Nomear controller e action de forma explícita.
- Usar atributos `Authorize` / `AllowAnonymous` corretamente.
- Definir `ProducesResponseType` para os principais cenários.
- Incluir exemplo de request/response na documentação da API.
