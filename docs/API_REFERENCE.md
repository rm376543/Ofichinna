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

### GET /api/item-servico?ordemServicoId={ordemServicoId}
Lista itens de serviço de uma ordem. **Perfis permitidos:** `ADMIN` | **Respostas:** `200`, `401`, `403`, `404`.

### GET /api/item-servico/{id}?ordemServicoId={ordemServicoId}
Retorna um item de serviço. **Perfis permitidos:** `ADMIN` | **Respostas:** `200`, `401`, `403`, `404`.

### POST /api/item-servico
Cria item de serviço e suas peças.

#### Requisição
```json
{ "ordemServicoId": "550e8400-e29b-41d4-a716-446655440000", "pecas": [{ "servicoPecaId": "660e8400-e29b-41d4-a716-446655440000", "quantidade": 1 }] }
```

**Perfis permitidos:** `ADMIN` | **Respostas:** `201`, `400`, `401`, `403`, `404`.

### PUT /api/item-servico
Atualiza o serviço de um item.

#### Requisição
```json
{ "id": "550e8400-e29b-41d4-a716-446655440000", "ordemServicoId": "660e8400-e29b-41d4-a716-446655440000", "servicoPecaId": "770e8400-e29b-41d4-a716-446655440000" }
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

### GET /api/orcamentos
Lista orçamentos com paginação.

#### Resposta 200
```json
{ "success": true, "data": { "items": [{ "id": "550e8400-e29b-41d4-a716-446655440000", "cliente": "João da Silva", "responsavel": "Maria Souza", "mecanicoDiagnostico": "Carlos Lima", "status": "EmDiagnostico", "dataCriacao": "2026-08-01T12:00:00Z", "dataValidade": "2026-08-10T12:00:00Z", "desconto": 10, "valorTotal": "R$ 1.250,00" }], "pageNumber": 1, "pageSize": 10, "totalCount": 1 } }
```

**Respostas:** `200`, `400`, `401`, `403`.

### GET /api/orcamentos/{id}
Retorna o orçamento detalhado com checklist e serviços previstos, incluindo peças e valores calculados.

#### Resposta 200
```json
{ "success": true, "data": { "id": "550e8400-e29b-41d4-a716-446655440000", "pessoaId": "...", "veiculoId": "...", "mecanicoDiagnosticoId": "...", "responsavelId": "...", "dataValidade": "2026-08-10T12:00:00Z", "desconto": 10, "observacoes": "Avaliar ruído", "status": "EmDiagnostico", "checklist": { "id": "...", "orcamentoId": "...", "hodometroEntrada": 35000, "itensVerificados": "Pneus, freios", "observacoes": "Sem vazamentos" }, "servicos": [{ "id": "...", "orcamentoId": "...", "servicoId": "...", "descricao": "Troca de óleo", "valorServico": 120, "valorTotal": 180, "pecas": [{ "pecaId": "...", "descricao": "Filtro de óleo", "quantidade": 1, "valorUnitario": 60, "valorTotal": 60 }] }] } }
```

**Respostas:** `200`, `401`, `403`, `404`.

### POST /api/orcamentos
Cria um orçamento.

#### Requisição
```json
{ "pessoaId": "550e8400-e29b-41d4-a716-446655440000", "veiculoId": "660e8400-e29b-41d4-a716-446655440000", "checklistId": "990e8400-e29b-41d4-a716-446655440000", "responsavelId": "770e8400-e29b-41d4-a716-446655440000", "mecanicoDiagnosticoId": "880e8400-e29b-41d4-a716-446655440000", "dataValidade": "2026-08-10T12:00:00Z", "observacoes": "Avaliar ruído", "desconto": 10, "itensServico": [{ "servicoId": "...", "pecaId": "...", "quantidade": 1 }] }
```

**Respostas:** `200`, `400`, `401`, `403`, `404`.

### PUT /api/orcamentos
Atualiza um orçamento.

#### Requisição
```json
{ "id": "550e8400-e29b-41d4-a716-446655440000", "pessoaId": "...", "veiculoId": "...", "responsavelId": "...", "mecanicoDiagnosticoId": "...", "dataValidade": "2026-08-10T12:00:00Z", "observacoes": "Atualizado", "desconto": 12, "itensServico": [{ "servicoId": "...", "pecaId": "...", "quantidade": 1 }] }
```

**Respostas:** `200`, `400`, `401`, `403`, `404`.

### PUT /api/orcamentos/{id}/iniciar-diagnostico
Inicia o diagnóstico do orçamento, alterando o status de `Recebida` para `EmDiagnostico`. **Respostas:** `200`, `400`, `401`, `403`, `404`.

### PUT /api/orcamentos/{id}/finalizar
Finaliza o orçamento após o diagnóstico, alterando o status para `AguardandoAprovacao`. **Respostas:** `200`, `400`, `401`, `403`, `404`.

### PUT /api/orcamentos/{id}/enviar
Marca o orçamento como enviado para o cliente e registra o histórico de status. O orçamento precisa estar finalizado antes desse passo. **Respostas:** `200`, `400`, `401`, `403`, `404`.

### PUT /api/orcamentos/{id}/aprovar
Aprova o orçamento, seleciona automaticamente o mecânico de reparo quando disponível e gera a ordem de serviço vinculada. **Respostas:** `200`, `400`, `401`, `403`, `404`.

### PUT /api/orcamentos/{id}/reprovar
Reprova o orçamento com motivo opcional, persistindo `MotivoRecusaOrcamento` e histórico de status. **Respostas:** `200`, `401`, `403`, `404`.

### PUT /api/orcamentos/{id}/reenviar
Reenvia o orçamento após reprovação, retornando o fluxo para diagnóstico e registrando histórico de status. **Respostas:** `200`, `401`, `403`, `404`.

## 🧾 Ordens de serviço

### GET /api/ordem-servico
Lista ordens paginadas. **Policy:** nenhuma | **Perfis permitidos:** `ADMIN` | **Respostas:** `200`, `400`, `401`, `403`.

### GET /api/ordem-servico/{id}
Retorna uma ordem detalhada. **Policy:** nenhuma | **Perfis permitidos:** `ADMIN` | **Respostas:** `200`, `401`, `403`, `404`.

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
