# Guia de Contribuição e Documentação - Ofichinna

## Objetivo

Definir o padrão mínimo para contribuir com código e documentação sem perder alinhamento entre arquitetura, Swagger e textos de apoio.

## Regras principais

1. Atualize a documentação sempre que adicionar ou alterar endpoint, contrato, policy ou módulo de DI.
2. Prefira comentários XML em membros públicos expostos por API ou usados pelo Swagger.
3. Mantenha `ProducesResponseType` e exemplos de resposta coerentes com os contratos reais.
4. Documente mudanças estruturais em `README.md`, `INDICE.md` e, quando necessário, em `SUMARIO_EXECUTIVO.md`.
5. Adicione ou atualize exemplos em `API_REFERENCE.md` para qualquer endpoint novo.

## Antes de abrir PR

- Executar `dotnet build`.
- Executar os testes relevantes.
- Validar o Swagger localmente.
- Revisar se houve impacto em documentação de apoio.

## Onde documentar cada tipo de mudança

- **Nova feature de API**: `API_REFERENCE.md`, `README.md`, `QUICK_REFERENCE.md`.
- **Mudança de arquitetura**: `ARQUITETURA.md`, `MAPA_VISUAL.md`, `SUMARIO_EXECUTIVO.md`.
- **Mudança operacional**: `TROUBLESHOOTING.md`, `START_HERE.md`, `QUICK_REFERENCE.md`.
- **Mudança de autorização**: `AUTORIZACAO-RBAC-POLICIES.md`.

## Padrão para novos endpoints

- Nomear controller e action de forma explícita.
- Usar atributos `Authorize` / `AllowAnonymous` corretamente.
- Definir `ProducesResponseType` para os principais cenários.
- Incluir exemplo de request/response na documentação da API.
