# Configuração

## Variáveis de Ambiente

SIM usa `dotnet user-secrets` para todas as credenciais. **Nunca coloque segredos no `appsettings.json`.**

### Secrets obrigatórios

| Chave | Descrição |
|-------|-----------|
| `Supabase:Url` | URL do projeto Supabase (ex: `https://ref.supabase.co`) |
| `ConnectionStrings:DefaultConnection` | Connection string com `No Reset On Close=true` |

### Configurar os secrets

```bash
cd SIM.WebApi

dotnet user-secrets set "Supabase:Url" "https://SEU_REF.supabase.co"

dotnet user-secrets set "ConnectionStrings:DefaultConnection" \
  "User Id=sim_api.SEU_REF;Password=SENHA_SIM_API;Server=aws-0-REGIAO.pooler.supabase.com;Port=5432;Database=postgres;No Reset On Close=true"
```

Confirme que foram salvos:

```bash
dotnet user-secrets list
```

---

## `appsettings.json`

Contém apenas configurações não-sensíveis. Os secrets sobrescrevem os valores vazios em runtime.

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "Supabase": {
    "Url": ""
  },
  "ConnectionStrings": {
    "DefaultConnection": ""
  }
}
```

---

## Rodando a API

```bash
cd SIM.WebApi
dotnet run
```

A porta aparece no terminal:
```
Now listening on: https://localhost:49170
```

**Scalar UI** (documentação interativa): `https://localhost:PORTA/scalar`

---

## Credenciais e Segurança

- Secrets são armazenados pelo `dotnet user-secrets` fora do repositório (AppData do usuário no Windows).
- A chave JWT RS256 do Supabase é obtida automaticamente via OIDC discovery — não há nenhum secret de JWT para configurar.
- Para alterar a senha do `sim_api` no banco: **SQL Editor do Supabase** → `ALTER ROLE sim_api WITH PASSWORD 'nova-senha';`
- Após alterar a senha, atualize o secret localmente: `dotnet user-secrets set "ConnectionStrings:DefaultConnection" "..."`.
