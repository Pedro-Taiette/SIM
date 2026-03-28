# API Reference

Todos os endpoints requerem `Authorization: Bearer {jwt_token}`.

O JWT é obtido autenticando via Supabase Auth. Ver [authentication.md](authentication.md).

---

## Organizations

| Método | Rota | Role |
|--------|------|------|
| `POST` | `/api/organizations` | Admin |
| `GET` | `/api/organizations` | Admin |
| `GET` | `/api/organizations/{id}` | Admin, StockManager |

### POST /api/organizations

```json
{
  "name": "Rede SIM Norte",
  "cnpj": "00000000000191",
  "type": "Private"
}
```

**Tipos válidos:** `Public`, `Private`

**Resposta — 201 Created**
```json
{
  "id": "uuid",
  "name": "Rede SIM Norte",
  "cnpj": "00000000000191",
  "type": "Private",
  "createdAt": "2026-01-15T10:00:00Z",
  "isActive": true
}
```

---

## Users

| Método | Rota | Role |
|--------|------|------|
| `POST` | `/api/users` | Admin |
| `GET` | `/api/users/{id}` | Admin |

### POST /api/users

Provisiona um `UserProfile` para um usuário já criado no Supabase Auth.

```json
{
  "supabaseUserId": "a1b2c3d4-5e6f-7890-abcd-ef1234567890",
  "fullName": "User",
  "email": "user@sim.com",
  "role": "Pharmacist",
  "organizationId": "uuid-da-organizacao",
  "unitId": null
}
```

**Roles válidas:** `Admin`, `Pharmacist`, `StockManager`, `ReceivingOperator`

**Resposta — 201 Created**
```json
{
  "id": "a1b2c3d4-5e6f-7890-abcd-ef1234567890",
  "fullName": "User",
  "email": "user@sim.com",
  "role": "Pharmacist",
  "organizationId": "uuid-da-organizacao",
  "unitId": null,
  "createdAt": "2026-01-15T10:00:00Z",
  "isActive": true
}
```

---

## Products

| Método | Rota | Role |
|--------|------|------|
| `POST` | `/api/products` | Admin, StockManager |
| `GET` | `/api/products` | Admin, StockManager, Pharmacist, ReceivingOperator |
| `GET` | `/api/products/{id}` | Admin, StockManager, Pharmacist, ReceivingOperator |

### POST /api/products

```json
{
  "name": "Dipirona 500mg",
  "description": "Analgésico e antipirético"
}
```

**Resposta — 201 Created**
```json
{
  "id": "uuid",
  "name": "Dipirona 500mg",
  "description": "Analgésico e antipirético",
  "createdAt": "2026-01-15T10:00:00Z",
  "isActive": true
}
```

---

## Erros

| Status | Quando |
|--------|--------|
| `400 Bad Request` | Erros de lógica de negócio (`BusinessLogicException`) |
| `401 Unauthorized` | Token ausente ou inválido |
| `403 Forbidden` | Role sem permissão para o endpoint |
| `404 Not Found` | Recurso não encontrado |
| `422 Unprocessable Entity` | Violação de regra de domínio (`DomainValidationException`) |
| `500 Internal Server Error` | Erro inesperado (logado no servidor) |

Todos os erros retornam o formato padrão **ProblemDetails** (RFC 7807):

```json
{
  "type": "https://tools.ietf.org/html/rfc7807",
  "title": "Bad Request",
  "status": 400,
  "detail": "Organization with this CNPJ already exists."
}
```

---

## Testando com Postman

### 1. Obter o JWT

`POST https://SEU_REF.supabase.co/auth/v1/token`

Headers:
- `apikey`: Anon/Publishable Key
- `Content-Type`: `application/json`

Body:
```json
{
  "email": "user@sim.com",
  "password": "senha",
  "grant_type": "password"
}
```

Salve o `access_token` como variável `{{token}}` no Postman.

### 2. Usar nos requests

Header em todos os requests:
```
Authorization: Bearer {{token}}
```
