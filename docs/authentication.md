# Autenticação & Autorização

## Visão Geral

SIM usa **dois sistemas separados** para identidade e perfil de negócio.

```
┌──────────────────────────────────┐     ┌──────────────────────────────────────┐
│  SUPABASE AUTH (auth.users)      │     │  SIM DATABASE (public.user_profiles) │
│                                  │     │                                      │
│  id:    "a1b2c3d4-..."  ◄────────┼─────┤  Id:             "a1b2c3d4-..."      │
│  email: "user@sim.com"           │     │  FullName:        "User"             │
│  (senha, tokens, sessões)        │     │  Role:            Admin              │
│                                  │     │  OrganizationId:  "org-uuid..."      │
└──────────────────────────────────┘     └──────────────────────────────────────┘
         Supabase gerencia                        SIM gerencia
```

O Supabase Auth gerencia **autenticação** (quem você é). A API SIM gerencia **autorização e dados de negócio** (o que você pode fazer e a qual organização pertence). Os dois sistemas se conectam pelo UUID do usuário (`auth.users.id` = `UserProfile.Id`).

---

## JWT RS256 / OIDC

Projetos Supabase usam assinatura assimétrica RS256. A API valida tokens via OIDC discovery:

```
Authority = {supabaseUrl}/auth/v1
```

O endpoint `.well-known/openid-configuration` expõe a chave pública. **Não há nenhum secret de JWT para configurar localmente.**

O `ValidAudience` esperado é `"authenticated"` — claim padrão dos tokens Supabase.

---

## Fluxo de Adição de Usuário

Para adicionar um novo membro ao SIM são necessárias duas etapas: criar a identidade no Supabase e provisionar o perfil na API.

### Etapa 1 — Criar a identidade (Admin, no dashboard Supabase)

Acesse: **Authentication → Users → Add user → Create new user**

- Informe email e senha do usuário
- Copie o **User UID** gerado (ex: `a1b2c3d4-5e6f-...`)

### Etapa 2 — Provisionar o UserProfile (Admin, via API)

```http
POST /api/users
Authorization: Bearer {token_do_admin}
Content-Type: application/json

{
  "supabaseUserId": "a1b2c3d4-5e6f-...",
  "fullName":       "User",
  "email":          "user@sim.com",
  "role":           "Pharmacist",
  "organizationId": "uuid-da-organizacao",
  "unitId":         null
}
```

### Etapa 3 — Login normal

O usuário autentica via Supabase (frontend ou Postman). O JWT retornado contém o `sub` com o UUID. A API valida o JWT, carrega o `UserProfile` pelo UUID e aplica as permissões automaticamente.

---

## Claims Transformation

A cada requisição autenticada:

1. JWT Bearer valida a assinatura via OIDC
2. `SupabaseClaimsTransformation` lê o `sub` do JWT (= `UserProfile.Id`)
3. Carrega o `UserProfile` do banco e injeta o `Role` como `ClaimTypes.Role`
4. `[Authorize(Roles = "Admin")]` passa a funcionar nativamente

---

## Roles e Permissões

| Role | Descrição | Acesso |
|------|-----------|--------|
| `Admin` | Gestor geral da rede | Total |
| `StockManager` | Responsável logístico | Leitura de orgs, criação/leitura de produtos |
| `Pharmacist` | Operador de balcão | Leitura de produtos |
| `ReceivingOperator` | Operador de recebimento | Leitura de produtos |

As roles são definidas como constantes em `SIM.WebApi/Auth/Roles.cs` e aplicadas via `[Authorize(Roles = Roles.X)]` nos controllers.
