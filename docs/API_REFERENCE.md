# 📎 Referência da API - Ofichinna

## 🔐 Autenticação

Os endpoints protegidos exigem o cabeçalho:

```http
Authorization: Bearer TOKEN_JWT
```

O formato padrão de resposta é `ApiResponse` ou uma resposta tipada (`ApiResponse` com dados):

```json
{
  "success": true,
  "message": "Mensagem opcional",
  "errors": [],
  "data": {}
}
```

`UserPolicyEnum` define os nomes `perfil.ler`, `perfil.escrever`, `perfil.atualizar` e `perfil.deletar`, mas não há `[Authorize(Policy = ...)]` ativo nos controllers atuais. A autorização efetiva abaixo é a declarada no código.

## 🔑 Endpoints de autenticação

### POST /api/auth/login
Autentica um usuário com e-mail e senha.

- **Policy:** nenhuma (`AllowAnonymous`)
- **Perfis permitidos:** público

#### Requisição
```json
{ "email": "admin@ofichinna.com", "senha": "Senha@123" }
```

#### Resposta 200
```json
{ "success": true, "message": "Autenticação realizada com sucesso.", "data": { "accessToken": "eyJhbGciOi...", "expiraEm": "2026-07-11T12:00:00Z", "usuarioId": "550e8400-e29b-41d4-a716-446655440000", "email": "admin@ofichinna.com", "perfis": ["ADMIN"] } }
```

**Respostas:** `400` validação; `401` credenciais inválidas; `500` erro interno.

### POST /api/auth/register
Cria um usuário autenticável.

- **Policy:** nenhuma (`AllowAnonymous`)
- **Perfis permitidos:** público

#### Requisição
```json
{ "email": "novo.usuario@ofichinna.com", "senha": "Senha@123" }
```

#### Resposta 201
```json
{ "success": true, "message": "Cadastro realizado com sucesso.", "data": { "accessToken": "eyJhbGciOi...", "expiraEm": "2026-07-11T12:00:00Z", "usuarioId": "550e8400-e29b-41d4-a716-446655440000", "email": "novo.usuario@ofichinna.com", "perfis": [] } }
```

**Respostas:** `400` validação ou cadastro não concluído.

## 👤 Endpoints de perfil e RBAC

Todos os endpoints desta seção exigem `[Authorize(Roles = "ADMIN")]`. O campo `Policy` é `nenhuma` porque nenhuma policy está aplicada nas actions atuais.

### GET /api/perfil
Lista os perfis cadastrados.

#### Requisição
```http
GET /api/perfil
```

#### Resposta 200
```json
{ "success": true, "data": [{ "id": "550e8400-e29b-41d4-a716-446655440000", "nomePerfil": "ADMIN", "descricao": "Perfil administrativo", "createdAt": "2026-07-11T12:00:00Z", "updatedAt": null, "deletedAt": null }] }
```

**Policy:** nenhuma | **Perfis permitidos:** `ADMIN` | **Respostas:** `400`, `401`, `403`.

### GET /api/perfil/{id}
Retorna um perfil pelo identificador. **Policy:** nenhuma | **Perfis permitidos:** `ADMIN`.

#### Resposta 200
```json
{ "success": true, "data": { "id": "550e8400-e29b-41d4-a716-446655440000", "nomePerfil": "ADMIN", "descricao": "Perfil administrativo" } }
```

**Respostas:** `401`, `403`, `404`.

### POST /api/perfil
Cria um perfil.

#### Requisição
```json
{ "nomePerfil": "OPERADOR", "descricao": "Perfil operacional" }
```

**Policy:** nenhuma | **Perfis permitidos:** `ADMIN` | **Respostas:** `201`, `400`, `401`, `403`.

### PUT /api/perfil
Atualiza um perfil.

#### Requisição
```json
{ "id": "550e8400-e29b-41d4-a716-446655440000", "nomePerfil": "OPERADOR", "descricao": "Perfil atualizado" }
```

**Policy:** nenhuma | **Perfis permitidos:** `ADMIN` | **Respostas:** `200`, `400`, `401`, `403`.

### DELETE /api/perfil/{id}
Desativa logicamente um perfil. **Policy:** nenhuma | **Perfis permitidos:** `ADMIN` | **Respostas:** `200`, `401`, `403`, `404`.

### GET /api/permissao
Lista permissões com paginação (`pageNumber` e `pageSize`). **Policy:** nenhuma | **Perfis permitidos:** `ADMIN`.

#### Resposta 200
```json
{ "success": true, "data": { "items": [{ "id": "550e8400-e29b-41d4-a716-446655440000", "codigo": "ordem.ler", "descricao": "Consulta de ordens" }], "pageNumber": 1, "pageSize": 10, "totalCount": 1 } }
```

**Respostas:** `200`, `400`, `401`, `403`.

### GET /api/permissao/{id}
Retorna uma permissão. **Policy:** nenhuma | **Perfis permitidos:** `ADMIN` | **Respostas:** `200`, `401`, `403`, `404`.

### POST /api/permissao
Cria uma permissão.

#### Requisição
```json
{ "codigo": "ordem.ler", "descricao": "Consulta de ordens" }
```

**Policy:** nenhuma | **Perfis permitidos:** `ADMIN` | **Respostas:** `200`, `400`, `401`, `403`.

### PUT /api/permissao
Atualiza uma permissão.

#### Requisição
```json
{ "id": "550e8400-e29b-41d4-a716-446655440000", "codigo": "ordem.editar", "descricao": "Edição de ordens" }
```

**Policy:** nenhuma | **Perfis permitidos:** `ADMIN` | **Respostas:** `200`, `400`, `401`, `403`, `404`.

### DELETE /api/permissao/{id}
Remove logicamente uma permissão. **Policy:** nenhuma | **Perfis permitidos:** `ADMIN` | **Respostas:** `200`, `401`, `403`, `404`.

### GET /api/perfil-permissao?perfilId={perfilId}
Lista as permissões vinculadas ao perfil com paginação. **Policy:** nenhuma | **Perfis permitidos:** `ADMIN` | **Respostas:** `200`, `400`, `401`, `403`, `404`.

### POST /api/perfil-permissao
Vincula uma permissão a um perfil.

#### Requisição
```json
{ "perfilId": "550e8400-e29b-41d4-a716-446655440000", "permissaoId": "660e8400-e29b-41d4-a716-446655440000" }
```

**Policy:** nenhuma | **Perfis permitidos:** `ADMIN` | **Respostas:** `200`, `400`, `401`, `403`, `404`.

### DELETE /api/perfil-permissao/{perfilId:guid}/permissao/{permissaoId:guid}
Desvincula uma permissão de um perfil. **Policy:** nenhuma | **Perfis permitidos:** `ADMIN` | **Respostas:** `200`, `400`, `401`, `403`, `404`.

### GET /api/usuario/{usuarioId}/perfil
Lista os perfis vinculados a um usuário. **Policy:** nenhuma | **Perfis permitidos:** `ADMIN` | **Respostas:** `200`, `401`, `403`.

### POST /api/usuario/{usuarioId}/perfil/{perfilId}
Vincula um perfil a um usuário. **Policy:** nenhuma | **Perfis permitidos:** `ADMIN` | **Respostas:** `200`, `400`, `401`, `403`.

## 📅 Agendamentos

### GET /api/agendamento/horarios-disponiveis
Lista horários disponíveis com paginação. **Policy:** nenhuma | **Perfis permitidos:** `ADMIN`.

#### Resposta 200
```json
{ "success": true, "data": { "items": [{ "id": "550e8400-e29b-41d4-a716-446655440000", "horario": "08:00:00" }], "pageNumber": 1, "pageSize": 10, "totalCount": 1 } }
```

**Respostas:** `200`, `400`, `401`, `404`.

### DELETE /api/agendamento/cancelar-agendamento
Cancela um agendamento para uma pessoa. **Policy:** nenhuma | **Perfis permitidos:** `ADMIN`.

#### Requisição
```json
{ "pessoaId": "550e8400-e29b-41d4-a716-446655440000", "agendamentoId": "660e8400-e29b-41d4-a716-446655440000" }
```

**Respostas:** `200`, `400`, `401`, `404`.

### POST /api/agendamento/cadastrar-horario
Cadastra um horário disponível. **Policy:** nenhuma | **Perfis permitidos:** `ADMIN`.

#### Requisição
```json
"08:00:00"
```

**Respostas:** `200`, `400`, `401`, `404`.

### GET /api/agendamento/pessoa/{pessoaId}
Lista agendamentos da pessoa. **Policy:** nenhuma | **Perfis permitidos:** usuário autenticado (`[Authorize]`) | **Respostas:** `200`, `400`, `401`, `403`, `404`.

### GET /api/agendamento/pessoa/{pessoaId}/agendamento/{id}
Obtém um agendamento. **Policy:** nenhuma | **Perfis permitidos:** usuário autenticado (`[Authorize]`) | **Respostas:** `200`, `401`, `403`, `404`.

### POST /api/agendamento/pessoa/{pessoaId}
Cria um agendamento.

#### Requisição
```json
{ "diaDisponibilidadeId": "550e8400-e29b-41d4-a716-446655440000", "horarioConsultorId": "660e8400-e29b-41d4-a716-446655440000", "veiculoId": "770e8400-e29b-41d4-a716-446655440000", "descricao": "Revisão" }
```

**Policy:** nenhuma | **Perfis permitidos:** usuário autenticado (`[Authorize]`) | **Respostas:** `201`, `400`, `401`, `403`, `404`, `409`.

## 🚗 Pessoas e veículos

Os endpoints de CRUD desta seção exigem `ADMIN`; `Policy: nenhuma`.

### GET /api/pessoa
Lista pessoas com paginação. **Perfis permitidos:** `ADMIN` | **Respostas:** `200`, `400`, `401`, `403`.

### GET /api/pessoa/{id}
Retorna uma pessoa. **Perfis permitidos:** `ADMIN` | **Respostas:** `200`, `401`, `403`, `404`.

### POST /api/pessoa
Cria uma pessoa.

#### Requisição
```json
{ "nome": "Maria Silva", "documento": "12345678900", "telefone": "11999999999", "logradouro": "Rua A", "numero": "10", "complemento": null, "bairro": "Centro", "cidade": "São Paulo", "estado": "SP", "cep": "01000000", "usuarioId": "550e8400-e29b-41d4-a716-446655440000" }
```

**Perfis permitidos:** `ADMIN` | **Respostas:** `201`, `400`, `401`, `403`.

### PUT /api/pessoa
Atualiza uma pessoa.

#### Requisição
```json
{ "id": "550e8400-e29b-41d4-a716-446655440000", "nome": "Maria Silva", "telefone": "11988888888" }
```

**Perfis permitidos:** `ADMIN` | **Respostas:** `200`, `400`, `401`, `403`, `404`.

### DELETE /api/pessoa/{id}
Remove logicamente uma pessoa. **Perfis permitidos:** `ADMIN` | **Respostas:** `200`, `401`, `403`, `404`.

### GET /api/veiculos/pessoa/{pessoaId}
Lista veículos de uma pessoa. **Perfis permitidos:** `ADMIN` | **Respostas:** `200`, `400`, `401`, `403`.

### GET /api/veiculos
Lista veículos com paginação. **Perfis permitidos:** `ADMIN` | **Respostas:** `200`, `400`, `401`, `403`.

### GET /api/veiculos/{id}
Retorna um veículo. **Perfis permitidos:** `ADMIN` | **Respostas:** `200`, `401`, `403`, `404`.

### POST /api/veiculos
Cria um veículo.

#### Requisição
```json
{ "pessoaId": "550e8400-e29b-41d4-a716-446655440000", "placa": "ABC1D23", "marca": "Toyota", "modelo": "Corolla", "anoFabricacao": 2022, "cor": "Prata", "hodometro": 35000 }
```

**Perfis permitidos:** `ADMIN` | **Respostas:** `201`, `400`, `401`, `403`, `404`.

### PUT /api/veiculos
Atualiza um veículo. **Perfis permitidos:** `ADMIN` | **Respostas:** `200`, `400`, `401`, `403`, `404`.

### DELETE /api/veiculos/{id}
Remove logicamente um veículo. **Perfis permitidos:** `ADMIN` | **Respostas:** `200`, `401`, `403`, `404`.

## 🧰 Serviços, peças e itens de serviço

Os endpoints desta seção exigem `ADMIN` e não usam policy nomeada.

Observação sobre peças:
- Nos itens de orçamento, `pecaId` é opcional.
- Nos fluxos de item de serviço da ordem de serviço, a peça continua obrigatória.

### GET /api/servicos
Lista serviços com paginação. **Perfis permitidos:** `ADMIN` | **Respostas:** `200`, `400`, `401`, `403`.

### GET /api/servicos/{id}
Retorna um serviço. **Perfis permitidos:** `ADMIN` | **Respostas:** `200`, `401`, `403`, `404`.

### POST /api/servicos
Cria um serviço.

#### Requisição
```json
{ "nome": "Troca de óleo", "descricao": "Óleo e filtro", "valor": 250.00 }
```

**Perfis permitidos:** `ADMIN` | **Respostas:** `200`, `400`, `401`, `403`.

### PUT /api/servicos
Atualiza um serviço. **Perfis permitidos:** `ADMIN` | **Respostas:** `200`, `400`, `401`, `403`, `404`.

### DELETE /api/servicos/{id}
Remove logicamente um serviço. **Perfis permitidos:** `ADMIN` | **Respostas:** `200`, `401`, `403`, `404`.

### GET /api/peca
Lista peças com paginação. **Perfis permitidos:** `ADMIN` | **Respostas:** `200`, `400`, `401`, `403`.

### GET /api/peca/{id}
Retorna uma peça. **Perfis permitidos:** `ADMIN` | **Respostas:** `200`, `401`, `403`, `404`.

### POST /api/peca
Cria uma peça.

#### Requisição
```json
{ "nome": "Filtro de óleo", "descricao": "Filtro compatível", "codigo": "FO-001", "valor": 45.90, "quantidadeEstoque": 20 }
```

**Perfis permitidos:** `ADMIN` | **Respostas:** `200`, `400`, `401`, `403`.

### PUT /api/peca
Atualiza uma peça. **Perfis permitidos:** `ADMIN` | **Respostas:** `200`, `400`, `401`, `403`, `404`.

### DELETE /api/peca/{id}
Remove logicamente uma peça. **Perfis permitidos:** `ADMIN` | **Respostas:** `200`, `401`, `403`, `404`.

### POST /api/servicos-pecas
Vincula uma peça a um serviço.

#### Requisição
```json
{ "servicoId": "550e8400-e29b-41d4-a716-446655440000", "pecaId": "660e8400-e29b-41d4-a716-446655440000", "quantidade": 2 }
```

**Perfis permitidos:** `ADMIN` | **Respostas:** `200`, `400`, `401`, `403`, `404`, `409`.

### DELETE /api/servicos-pecas/{servicoId}/pecas/{pecaId}
Desativa uma peça do serviço. **Perfis permitidos:** `ADMIN` | **Respostas:** `200`, `400`, `401`, `403`, `404`, `409`.

### DELETE /api/servicos-pecas/{servicoId}/pecas
Desativa todas as peças do serviço. **Perfis permitidos:** `ADMIN` | **Respostas:** `200`, `400`, `401`, `403`, `404`, `409`.

### GET /api/item-servico/buscar-por/{ordemServicoId}
Lista itens de serviço de uma ordem. **Perfis permitidos:** `ADMIN` | **Respostas:** `200`, `401`, `403`, `404`.

### GET /api/item-servico/buscar-por/{ordemServicoId}/{itemServicoId}
Retorna um item de serviço de uma ordem. **Perfis permitidos:** `ADMIN` | **Respostas:** `200`, `401`, `403`, `404`.

### GET /api/item-servico/buscar-por-orcamento/{orcamentoId}
Lista itens de serviço de um orçamento. **Perfis permitidos:** `ADMIN` | **Respostas:** `200`, `401`, `403`, `404`.

### GET /api/item-servico/buscar-por-orcamento/{orcamentoId}/{itemServicoId}
Retorna um item de serviço de um orçamento. **Perfis permitidos:** `ADMIN` | **Respostas:** `200`, `401`, `403`, `404`.

### POST /api/item-servico/adicionar
Cria item de serviço para a ordem de serviço.

#### Requisição
```json
{ "ordemServicoId": "550e8400-e29b-41d4-a716-446655440000", "servicoId": "660e8400-e29b-41d4-a716-446655440000", "pecaId": "770e8400-e29b-41d4-a716-446655440000", "quantidade": 1 }
```

**Perfis permitidos:** `ADMIN` | **Respostas:** `201`, `400`, `401`, `403`, `404`.

### POST /api/item-servico/adicionar/para-orcamento
Cria item de serviço para o orçamento.

#### Requisição
```json
{ "orcamentoId": "550e8400-e29b-41d4-a716-446655440000", "servicoId": "660e8400-e29b-41d4-a716-446655440000", "pecaId": null, "quantidade": 1 }
```

**Perfis permitidos:** `ADMIN` | **Respostas:** `200`, `400`, `401`, `403`, `404`.

### POST /api/item-servico/adicionar-servico/para-orcamento
Cria item de serviço somente-serviço para o orçamento.

#### Requisição
```json
{ "orcamentoId": "550e8400-e29b-41d4-a716-446655440000", "servicoId": "660e8400-e29b-41d4-a716-446655440000" }
```

**Perfis permitidos:** `ADMIN` | **Respostas:** `200`, `400`, `401`, `403`, `404`.

### PUT /api/item-servico/atualizar
Atualiza item de serviço da ordem.

#### Requisição
```json
{ "itemServicoId": "550e8400-e29b-41d4-a716-446655440000", "ordemServicoId": "660e8400-e29b-41d4-a716-446655440000", "servicoId": "770e8400-e29b-41d4-a716-446655440000", "pecaId": "880e8400-e29b-41d4-a716-446655440000", "quantidade": 1 }
```

**Perfis permitidos:** `ADMIN` | **Respostas:** `200`, `400`, `401`, `403`, `404`, `409`.

### PUT /api/item-servico/atualizar/para-orcamento
Atualiza item de serviço do orçamento.

#### Requisição
```json
{ "itemServicoId": "550e8400-e29b-41d4-a716-446655440000", "orcamentoId": "660e8400-e29b-41d4-a716-446655440000", "servicoId": "770e8400-e29b-41d4-a716-446655440000", "pecaId": null, "quantidade": 1 }
```

**Perfis permitidos:** `ADMIN` | **Respostas:** `200`, `400`, `401`, `403`, `404`, `409`.

### PUT /api/item-servico/atualizar-servico/para-orcamento
Atualiza item de serviço somente-serviço do orçamento.

#### Requisição
```json
{ "itemServicoId": "550e8400-e29b-41d4-a716-446655440000", "orcamentoId": "660e8400-e29b-41d4-a716-446655440000", "servicoId": "770e8400-e29b-41d4-a716-446655440000" }
```

**Perfis permitidos:** `ADMIN` | **Respostas:** `200`, `400`, `401`, `403`, `404`, `409`.

### POST /api/item-servico/adicionar-servico/para-ordem-servico
Cria item de serviço somente-serviço para a ordem de serviço.

#### Requisição
```json
{ "ordemServicoId": "550e8400-e29b-41d4-a716-446655440000", "servicoId": "660e8400-e29b-41d4-a716-446655440000" }
```

**Perfis permitidos:** `ADMIN` | **Respostas:** `200`, `400`, `401`, `403`, `404`.

### PUT /api/item-servico/atualizar-servico/para-ordem-servico
Atualiza item de serviço somente-serviço da ordem de serviço.

#### Requisição
```json
{ "itemServicoId": "550e8400-e29b-41d4-a716-446655440000", "ordemServicoId": "660e8400-e29b-41d4-a716-446655440000", "servicoId": "770e8400-e29b-41d4-a716-446655440000" }
```

**Perfis permitidos:** `ADMIN` | **Respostas:** `200`, `400`, `401`, `403`, `404`, `409`.

### DELETE /api/item-servico
Remove um item de serviço.

#### Requisição
```json
{ "id": "550e8400-e29b-41d4-a716-446655440000", "ordemServicoId": "660e8400-e29b-41d4-a716-446655440000" }
```

**Perfis permitidos:** `ADMIN` | **Respostas:** `200`, `401`, `403`, `404`.

## 💰 Orçamentos

Todos os endpoints desta seção exigem `[Authorize(Roles = "ADMIN")]`.

### GET /api/orcamento
Lista orçamentos com paginação.

#### Resposta 200
```json
{ "success": true, "data": { "items": [{ "id": "550e8400-e29b-41d4-a716-446655440000", "cliente": "João da Silva", "responsavel": "Maria Souza", "mecanicoDiagnostico": "Carlos Lima", "status": "EmDiagnostico", "dataCriacao": "2026-08-01T12:00:00Z", "dataValidade": "2026-08-10T12:00:00Z", "desconto": 10, "valorTotal": 1120.00 }], "pageNumber": 1, "pageSize": 10, "totalCount": 1 } }
```

**Respostas:** `200`, `400`, `401`, `403`.

### GET /api/orcamento/detalhar/{id}
Retorna o orçamento detalhado com checklist e serviços previstos, incluindo `valorTotal` bruto, `valorTotalDesconto` com desconto aplicado, `valorDesconto` e `descontoEmDinheiro`.

#### Resposta 200
```json
{ "success": true, "data": { "id": "550e8400-e29b-41d4-a716-446655440000", "pessoaId": "...", "veiculoId": "...", "mecanicoId": "...", "consultorId": "...", "dataValidade": "2026-08-10T12:00:00Z", "desconto": 10, "valorTotal": 1120.00, "observacoes": "Avaliar ruído", "status": "EmDiagnostico", "checklist": { "id": "...", "orcamentoId": "...", "hodometroEntrada": 35000, "itensVerificados": "Pneus, freios", "observacoes": "Sem vazamentos" }, "itensServico": [{ "orcamentoId": "...", "servicos": [{ "servicoId": "...", "descricao": "Troca de óleo", "valorServico": 120, "valorTotal": 180, "pecas": [{ "pecaId": "...", "descricao": "Filtro de óleo", "quantidade": 1, "valorUnitario": 60, "valorTotal": 60 }] }] }] } }
```

**Respostas:** `200`, `401`, `403`, `404`.

### POST /api/orcamento/adicionar
Cria um orçamento com dados básicos. Os itens são adicionados em seguida no endpoint `POST /api/orcamento/{id}/itens`.

#### Requisição
```json
{ "pessoaId": "550e8400-e29b-41d4-a716-446655440000", "veiculoId": "660e8400-e29b-41d4-a716-446655440000", "agendamentoId": "990e8400-e29b-41d4-a716-446655440000", "consultorId": "770e8400-e29b-41d4-a716-446655440000", "mecanicoId": "880e8400-e29b-41d4-a716-446655440000", "dataValidade": "2026-08-10T12:00:00Z", "observacoes": "Avaliar ruído" }
```

**Respostas:** `200`, `400`, `401`, `403`, `404`.

### POST /api/orcamento/{id}/itens
Adiciona um ou mais itens ao orçamento.

#### Requisição
```json
{ "orcamentoId": "550e8400-e29b-41d4-a716-446655440000", "itens": [{ "servicoId": "...", "pecaId": null, "quantidade": 2 }, { "servicoId": "...", "pecaId": "...", "quantidade": 1 }] }
```

**Observações:** `pecaId` é opcional nesta rota e `orcamentoId` deve ser igual ao identificador da rota.

**Respostas:** `200`, `400`, `401`, `403`, `404`.

### PUT /api/orcamento/atualizar
Atualiza apenas os dados de cabeçalho de um orçamento.

#### Requisição
```json
{ "orcamentoId": "550e8400-e29b-41d4-a716-446655440000", "pessoaId": "...", "veiculoId": "...", "consultorId": "...", "mecanicoId": "...", "dataValidade": "2026-08-10T12:00:00Z", "observacoes": "Atualizado", "itensServico": [{ "servicoId": "...", "pecaId": null, "quantidade": 1 }] }
```

**Respostas:** `200`, `400`, `401`, `403`, `404`.

### POST /api/orcamento/iniciar-diagnostico
Inicia o diagnóstico do orçamento, alterando o status de `Criado` para `EmDiagnostico`. O orçamento precisa ter ao menos um item ativo. **Respostas:** `200`, `400`, `401`, `403`, `404`.

### POST /api/orcamento/finalizar
Finaliza o orçamento após o diagnóstico, alterando o status para `AguardandoEnvio`. **Respostas:** `200`, `400`, `401`, `403`, `404`.

### POST /api/orcamento/enviar
Marca o orçamento como enviado para o cliente, alterando o status para `AguardandoAprovacao` e registrando o histórico de status. O orçamento precisa estar finalizado antes desse passo. **Respostas:** `200`, `400`, `401`, `403`, `404`.

### POST /api/orcamento/aprova
Aprova o orçamento, busca automaticamente o hodômetro do agendamento vinculado ao orçamento, cria a ordem de serviço com status inicial `CRIADO` e vincula os itens do orçamento à nova OS. A execução passa a ser iniciada manualmente em seguida. **Respostas:** `200`, `400`, `401`, `403`, `404`.

#### Requisição
```json
{ "orcamentoId": "550e8400-e29b-41d4-a716-446655440000" }
```

### POST /api/orcamento/reprovar
Reprova o orçamento com motivo opcional, persistindo `MotivoRecusaOrcamento` e histórico de status. **Respostas:** `200`, `401`, `403`, `404`.

### POST /api/orcamento/reenviar-para-diagnostico
Reenvia o orçamento após reprovação, retornando o fluxo para diagnóstico e registrando histórico de status. **Respostas:** `200`, `401`, `403`, `404`.

## 🧾 Ordens de serviço

### GET /api/ordem-servico
Lista ordens paginadas com o contrato simplificado `OrdemServicoSimplesResponse`, incluindo `problemaRelatado`, status, datas e valor total formatado. **Policy:** nenhuma | **Perfis permitidos:** `ADMIN` | **Respostas:** `200`, `400`, `401`, `403`.

#### Resposta 200
```json
{ "success": true, "data": { "items": [{ "ordemServicoId": "660e8400-e29b-41d4-a716-446655440000", "cliente": "João da Silva", "consultor": "Maria Souza", "problemaRelatado": "Barulhos durante a aceleração", "status": "CRIADO", "dataAbetura": "16/08/2026", "dataFinalizacao": "", "observacao": "carro de dev", "valorTotal": "R$ 1.120,00" }], "pageNumber": 1, "pageSize": 10, "totalCount": 1 } }
```

### GET /api/ordem-servico/{id}
Retorna uma ordem detalhada com `OrdemServicoResponse`, expondo `problemaRelatado`, datas, valor total e os serviços vinculados. **Policy:** nenhuma | **Perfis permitidos:** `ADMIN` | **Respostas:** `200`, `401`, `403`, `404`.

#### Resposta 200
```json
{ "success": true, "data": { "ordemServicoId": "660e8400-e29b-41d4-a716-446655440000", "pessoaId": "550e8400-e29b-41d4-a716-446655440000", "veiculoId": "770e8400-e29b-41d4-a716-446655440000", "consultorId": "880e8400-e29b-41d4-a716-446655440000", "mecanicoId": "990e8400-e29b-41d4-a716-446655440000", "hodometro": 78123, "problemaRelatado": "Barulhos durante a aceleração", "status": "CRIADO", "dataAbertura": "2026-08-16T12:00:00Z", "dataFinalizacao": null, "observacao": "carro de dev", "valorTotal": 1120.00, "servicos": [{ "ordemServicoId": "660e8400-e29b-41d4-a716-446655440000", "servicos": [{ "servicoId": "11111111-2222-3333-4444-555555555555", "descricao": "Troca de óleo", "valorServico": 120, "valorTotal": 180, "pecas": [{ "pecaId": "aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee", "descricao": "Filtro de óleo", "quantidade": 1, "valorUnitario": 60, "valorTotal": 60 }] }] }, "createdAt": "2026-08-16T12:00:00Z", "updatedAt": null, "deletedAt": null } }
```

### PUT /api/ordem-servico/{id}/execucao
Inicia a execução da OS. **Policy:** nenhuma | **Perfis permitidos:** `ADMIN` | **Respostas:** `200`, `400`, `401`, `403`, `404`.

### PUT /api/ordem-servico/{id}/finalizar
Finaliza a OS. **Policy:** nenhuma | **Perfis permitidos:** `ADMIN` | **Respostas:** `200`, `400`, `401`, `403`, `404`.

### PUT /api/ordem-servico/{id}/entregar
Marca a OS como entregue. **Policy:** nenhuma | **Perfis permitidos:** `ADMIN` | **Respostas:** `200`, `400`, `401`, `403`, `404`.

### PUT /api/ordem-servico/{id}/cancelar
Cancela a OS. **Policy:** nenhuma | **Perfis permitidos:** `ADMIN` | **Respostas:** `200`, `400`, `401`, `403`, `404`.

### DELETE /api/ordem-servico/{id}
Remove logicamente uma OS. **Policy:** nenhuma | **Perfis permitidos:** `ADMIN` | **Respostas:** `200`, `401`, `403`, `404`.

#### Resposta de transição 200
```json
{ "success": true, "message": "Ordem de serviço atualizada com sucesso.", "errors": [], "data": null }
```

### Contratos relacionados ao fluxo de aprovação do orçamento

Ao aprovar um orçamento, a API cria uma OS com status inicial `CRIADO`, preserva o `problemaRelatado` a partir do orçamento/observações e vincula os itens do orçamento à nova OS. Esse fluxo deve manter coerência entre:

- `POST /api/orcamento/aprova`
- `GET /api/ordem-servico`
- `GET /api/ordem-servico/{id}`

Os testes automatizados devem validar:

1. o contrato detalhado expõe `problemaRelatado`;
2. o contrato simplificado expõe `problemaRelatado` e as demais informações da listagem;
3. a OS gerada a partir da aprovação do orçamento mantém o vínculo com os itens e o status inicial esperado.

## 📊 Códigos de resposta

- `200 OK`: operação concluída com sucesso.
- `201 Created`: recurso criado com sucesso.
- `400 Bad Request`: validação falhou ou regra de negócio rejeitou a operação.
- `401 Unauthorized`: token ausente, inválido ou credenciais inválidas.
- `403 Forbidden`: usuário autenticado sem a role exigida.
- `404 Not Found`: recurso não encontrado.
- `409 Conflict`: conflito de vínculo ou regra de integridade.
- `500 Internal Server Error`: erro não tratado, convertido pelo `ApiExceptionMiddleware`.

---

**Última atualização:** 2026  
**Versão:** 2.0  
**Status:** ✅ Referência sincronizada com os controllers e contratos atuais
