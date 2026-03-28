# Banco de Dados & Supabase

## Visão Geral

SIM usa **Supabase** como plataforma de banco de dados (PostgreSQL gerenciado) e autenticação. A conexão com o banco é feita via **Session Mode Pooler** (PgBouncer) usando um role dedicado `sim_api`.

---

## Setup do Projeto Supabase

> Cada desenvolvedor usa o **mesmo projeto Supabase** do time.
> Peça as credenciais ao Tech Lead e vá direto para [Configuração Local](configuration.md).
> Siga os passos abaixo apenas se estiver criando um projeto do zero.

### 1. Criar o projeto

Acesse [supabase.com](https://supabase.com), crie uma conta e um novo projeto. Anote a região (ex: `sa-east-1`).

### 2. Criar o role `sim_api`

No dashboard: **SQL Editor → New query**. Execute:

```sql
CREATE ROLE sim_api WITH LOGIN PASSWORD 'senha-forte-sem-caracteres-especiais';

GRANT CONNECT ON DATABASE postgres TO sim_api;
GRANT USAGE ON SCHEMA public TO sim_api;

GRANT SELECT, INSERT, UPDATE, DELETE ON ALL TABLES IN SCHEMA public TO sim_api;

ALTER DEFAULT PRIVILEGES IN SCHEMA public
    GRANT SELECT, INSERT, UPDATE, DELETE ON TABLES TO sim_api;
```

> Evite `;`, `=`, `@`, `%`, `&` na senha — eles quebram a connection string.

### 3. Coletar credenciais

| O que | Onde no dashboard |
|-------|------------------|
| **Project URL** | Settings → Data API → Project URL |
| **Anon/Publishable Key** | Connect → API Keys → Publishable Key |
| **Connection String** | Settings → Database → Connection Pooler → Session mode → .NET |

A connection string do pooler tem o formato:
```
User Id=postgres.[ref];Password=...;Server=aws-0-REGIAO.pooler.supabase.com;Port=5432;Database=postgres
```

Para o `sim_api`, substitua `postgres.[ref]` por `sim_api.[ref]`.

---

## Migrations

### Por que não `dotnet ef database update`?

O EF Core executa migrations dentro de transações DDL. O PgBouncer (pooler do Supabase) não suporta esse padrão e descarta a conexão antes da transação completar. A solução é gerar um script SQL puro e executá-lo diretamente no SQL Editor do Supabase.

### Aplicar migrations (primeira vez ou nova migration no repositório)

**Passo 1 — Gerar o script SQL**

Na raiz do repositório:

```bash
dotnet ef migrations script \
  --project SIM.Infrastructure \
  --startup-project SIM.WebApi \
  --output migration.sql \
  --idempotent
```

A flag `--idempotent` torna o script seguro para reexecutar.

**Passo 2 — Executar no Supabase**

1. Abra `migration.sql` gerado na raiz do projeto
2. Copie o conteúdo
3. No dashboard: **SQL Editor → New query**
4. Cole e clique em **Run**

> `migration.sql` está no `.gitignore` — não commitar. Ver abaixo.

### Criar uma nova migration (ao modificar entidades)

```bash
dotnet ef migrations add NomeDescritivo \
  --project SIM.Infrastructure \
  --startup-project SIM.WebApi
```

Depois repita os Passos 1 e 2 acima para aplicar no banco.

### O que commitar

| Arquivo | Git |
|---------|-----|
| `SIM.Infrastructure/Migrations/*.cs` | **Sim** — fonte da verdade |
| `migration.sql` | **Não** — artefato derivado, gerado on-demand |

---

## Notas sobre PgBouncer

O Supabase usa PgBouncer em **Session Mode** como pooler. Dois comportamentos importantes:

- **`No Reset On Close=true`** — obrigatório na connection string. O Npgsql tenta resetar a conexão ao devolvê-la ao pool; o PgBouncer descarta o objeto antes disso. Esse parâmetro desativa o reset.
- **DDL em transações** — não suportado pelo PgBouncer, por isso migrations são feitas via SQL script.
