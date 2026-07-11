# Referência da API - Ofichinna

## Autenticação

Todos os endpoints protegidos exigem o cabeçalho:

```http
Authorization: Bearer <token>
```

O formato padrão de resposta segue `ApiResponse` e `ApiResponse<T>`:

```json
{
  "success": true,
  "message": "Mensagem opcional",
  "errors": [],
  "data": { }
}
```

## Endpoints de autenticação

### POST /api/auth/login

Autentica um usuário com e-mail e senha.

#### Requisição

```json
{
  "email": "admin@ofichinna.com",
  "senha": "Senha@123"
}
```

#### Resposta 200

```json
{
  "success": true,
  "message": "Autenticação realizada com sucesso.",
  "data": {
	"accessToken": "eyJhbGciOi...",
	"expiraEm": "2026-07-11T12:00:00Z",
	"usuarioId": "550e8400-e29b-41d4-a716-446655440000",
	"email": "admin@ofichinna.com",
	"perfis": ["ADMIN"]
  }
}
```

### POST /api/auth/register

Cria um novo usuário autenticável.

#### Requisição

```json
{
  "email": "novo.usuario@ofichinna.com",
  "senha": "Senha@123"
}
```

#### Resposta 201

```json
{
  "success": true,
  "message": "Cadastro realizado com sucesso.",
  "data": {
	"accessToken": "eyJhbGciOi...",
	"expiraEm": "2026-07-11T12:00:00Z",
	"usuarioId": "550e8400-e29b-41d4-a716-446655440000",
	"email": "novo.usuario@ofichinna.com",
	"perfis": []
  }
}
```

## Endpoints de perfil

### GET /api/perfil

Lista todos os perfis cadastrados.

- Policy: `UserPolicyEnum.Ler`
- Perfis permitidos: `USUARIO`, `ADMIN`

### GET /api/perfil/{id}

Retorna um perfil pelo identificador.

#### Resposta 200

```json
{
  "success": true,
  "data": {
	"id": "550e8400-e29b-41d4-a716-446655440000",
	"nome": "ADMIN",
	"descricao": "Perfil administrativo",
	"createdAt": "2026-07-11T12:00:00Z",
	"updatedAt": null,
	"deletedAt": null
  }
}
```

### POST /api/perfil

Cria um novo perfil.

- Policy: `UserPolicyEnum.Escrever`
- Perfil permitido: `ADMIN`

#### Requisição

```json
{
  "nomePerfil": "OPERADOR",
  "descricao": "Perfil operacional"
}
```

### PUT /api/perfil

Atualiza um perfil existente.

- Policy: `UserPolicyEnum.Atualizar`
- Perfil permitido: `ADMIN`

#### Requisição

```json
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "nomePerfil": "OPERADOR",
  "descricao": "Perfil operacional atualizado"
}
```

### DELETE /api/perfil/{id}

Desativa um perfil existente.

- Policy: `UserPolicyEnum.Deletar`
- Perfil permitido: `ADMIN`

## Códigos de resposta

- `200 OK`: operação concluída com sucesso.
- `201 Created`: recurso criado com sucesso.
- `400 Bad Request`: validação falhou.
- `401 Unauthorized`: token ausente ou inválido.
- `403 Forbidden`: usuário autenticado sem permissão.
- `404 Not Found`: recurso não encontrado.
