# SIM
Plataforma de gestão de inventário e logística de medicamentos para redes de farmácias públicas e privadas.

---

## Stack

- .NET 10 / C# 13
- Supabase: Auth (JWT RS256) + PostgreSQL
- Entity Framework Core 10 + Npgsql 10
- FluentValidation 12

## Pré-requisitos

| Ferramenta | Versão mínima |
|------------|--------------|
| .NET SDK | 10.0 |
| EF Core CLI | 10.0 |
| Git | qualquer |

```bash
dotnet tool install --global dotnet-ef
dotnet --version    # 10.x.x
dotnet ef --version # 10.x.x
```

## Quick Start

```bash
git clone <url-do-repositorio>
cd SIM/SIM.WebApi

dotnet user-secrets set "Supabase:Url" "https://SEU_REF.supabase.co"
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "User Id=sim_api.SEU_REF;Password=SENHA;Server=aws-0-REGIAO.pooler.supabase.com;Port=5432;Database=postgres;No Reset On Close=true"

cd ..
dotnet run --project SIM.WebApi
```

Scalar UI (documentação interativa): `https://localhost:PORTA/scalar`

---

## Documentação

- [Arquitetura](docs/architecture.md)
- [Banco de Dados & Supabase](docs/database.md)
- [Configuração](docs/configuration.md)
- [Autenticação & Autorização](docs/authentication.md)
- [Code Standards](docs/code-standards.md)
- [API Reference](docs/api-reference.md)
