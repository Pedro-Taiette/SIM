# Frontend Standards

## Stack

| Tecnologia | Uso |
|---|---|
| **React 19** + **TypeScript** | Framework e tipagem |
| **Vite** | Build tool |
| **React Router v7** | Roteamento (library mode) |
| **Zustand** | Estado global |
| **Axios** | HTTP client |
| **React Hook Form** + **Zod** | Formulários e validação |
| **TailwindCSS v4** | Estilização |
| **shadcn/ui** | Componentes de UI (Radix + Tailwind) |
| **lucide-react** | Ícones |

**Regra fundamental:** o backend é a única source of truth. O frontend nunca se comunica diretamente com o Supabase ou qualquer serviço externo. Toda operação passa pela `SIM.WebApi`.

---

## Estrutura de Pastas

```
src/
  pages/              ← componentes de rota
    login/
      LoginPage.tsx   ← só JSX
      useLoginForm.ts ← toda a lógica
  components/
    ui/               ← shadcn/ui (não editar manualmente)
    layout/           ← componentes estruturais (PrivateRoute, etc.)
  hooks/              ← hooks globais reutilizáveis
  store/              ← Zustand stores
  lib/                ← utilitários (api.ts, tokenStorage.ts, theme.ts, utils.ts)
  types/              ← interfaces e types TypeScript compartilhados
  router.tsx          ← definição centralizada de rotas
  App.tsx             ← RouterProvider, nada mais
  main.tsx            ← entry point
```

---

## O Padrão de Página — A Lei

Toda feature nova segue esta estrutura sem exceção:

```
src/pages/<feature>/
  <Feature>Page.tsx   → renderiza JSX usando componentes e o hook
  use<Feature>.ts     → toda a lógica: form, validação, API, navegação
```

### Page component — só renderiza

```tsx
// src/pages/users/UsersPage.tsx
import { useUsers } from './useUsers'
import { Button } from '@/components/ui/button'

export default function UsersPage() {
  const { users, isLoading, onCreate } = useUsers()

  if (isLoading) return <p>Carregando...</p>

  return (
    <div>
      {users.map(u => <p key={u.id}>{u.fullName}</p>)}
      <Button onClick={onCreate}>Novo usuário</Button>
    </div>
  )
}
```

### Page hook — toda a lógica

```ts
// src/pages/users/useUsers.ts
import { useState, useEffect } from 'react'
import { api } from '@/lib/api'
import { useNavigate } from 'react-router'

export function useUsers() {
  const [users, setUsers] = useState([])
  const [isLoading, setIsLoading] = useState(true)
  const navigate = useNavigate()

  useEffect(() => {
    api.get('/api/users').then(({ data }) => {
      setUsers(data)
      setIsLoading(false)
    })
  }, [])

  function onCreate() {
    navigate('/users/new')
  }

  return { users, isLoading, onCreate }
}
```

**Regras:**
- `Page.tsx` nunca importa `api`, `useAuthStore` ou faz chamadas HTTP diretamente
- `usePage.ts` nunca retorna JSX
- Lógica compartilhada entre páginas vai em `src/hooks/`, não duplicada em cada `usePage.ts`

---

## Roteamento

Rotas definidas em `src/router.tsx`. Sempre usar `createBrowserRouter`.

```tsx
// src/router.tsx
import { createBrowserRouter } from 'react-router'
import PrivateRoute from '@/components/layout/PrivateRoute'
import LoginPage from '@/pages/login/LoginPage'
import DashboardPage from '@/pages/dashboard/DashboardPage'

export const router = createBrowserRouter([
  { path: '/login', element: <LoginPage /> },
  {
    element: <PrivateRoute />,        // protege todas as rotas filhas
    children: [
      { path: '/', element: <DashboardPage /> },
      { path: '/users', element: <UsersPage /> },
    ],
  },
])
```

**Rotas protegidas:** `PrivateRoute` redireciona para `/login` automaticamente se não autenticado. Nunca checar `isAuthenticated` manualmente dentro das páginas.

---

## Estado Global — Zustand

Usar Zustand para estado compartilhado entre múltiplas páginas. Estado local de uma única página fica no hook da página via `useState`.

```ts
// src/store/exampleStore.ts
import { create } from 'zustand'

interface ExampleState {
  count: number
  increment: () => void
}

export const useExampleStore = create<ExampleState>((set) => ({
  count: 0,
  increment: () => set((state) => ({ count: state.count + 1 })),
}))
```

**Regra de separação de responsabilidades nos stores:**
- Store = estado puro + ações simples
- Nunca importar `api` dentro de um store
- Chamadas HTTP ficam nos page hooks (`usePage.ts`) ou em hooks globais (`src/hooks/`)

O `authStore` é a exceção documentada — expõe `setUser` e `signOut`, mas o `signIn` (que usa `api`) fica no `useAuth`.

---

## Autenticação

O fluxo completo está implementado e não deve ser alterado sem alinhamento:

```
signIn(email, password)
  → api.post('/api/auth/login')
  → tokenStorage.save(accessToken, refreshToken, expiresIn, email)
  → authStore.setUser({ email })

Toda request autenticada:
  → interceptor lê tokenStorage.getAccessToken()
  → injeta Authorization: Bearer <token>

Token expirado (401):
  → interceptor tenta api.post('/api/auth/refresh')
  → se falhar → tokenStorage.clear() + authStore.signOut()
  → usuário redirecionado para /login automaticamente
```

Para usar autenticação em qualquer componente ou hook:

```ts
import { useAuth } from '@/hooks/useAuth'

const { user, isAuthenticated, signIn, signOut } = useAuth()
```

---

## Chamadas HTTP

Sempre usar a instância `api` de `src/lib/api.ts`. Nunca criar instâncias axios avulsas.

```ts
import { api } from '@/lib/api'

// GET
const { data } = await api.get('/api/organizations')

// POST
const { data } = await api.post('/api/users', { email, fullName, role, organizationId })

// PUT
const { data } = await api.put(`/api/users/${id}`, payload)

// DELETE
await api.delete(`/api/users/${id}`)
```

O token de autenticação é injetado automaticamente. Não adicionar headers manualmente.

---

## Formulários

Todos os formulários usam **React Hook Form** + **Zod** + componentes shadcn `Form`.

### Estrutura padrão

```ts
// use<Feature>.ts
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'

const schema = z.object({
  name: z.string().min(1, 'Nome é obrigatório.').max(200),
  email: z.string().email('Email inválido.'),
})

type FormValues = z.infer<typeof schema>

export function useCreateUserForm() {
  const form = useForm<FormValues>({
    resolver: zodResolver(schema),
    defaultValues: { name: '', email: '' },
  })

  async function onSubmit(values: FormValues): Promise<void> {
    await api.post('/api/users', values)
    // navigate ou reset
  }

  return {
    form,
    onSubmit: form.handleSubmit(onSubmit),
    isSubmitting: form.formState.isSubmitting,
  }
}
```

```tsx
// <Feature>Page.tsx
import { Form, FormControl, FormField, FormItem, FormLabel, FormMessage } from '@/components/ui/form'
import { Input } from '@/components/ui/input'
import { Button } from '@/components/ui/button'

export default function CreateUserPage() {
  const { form, onSubmit, isSubmitting } = useCreateUserForm()

  return (
    <Form {...form}>
      <form onSubmit={onSubmit} className="space-y-4">
        <FormField
          control={form.control}
          name="name"
          render={({ field }) => (
            <FormItem>
              <FormLabel>Nome</FormLabel>
              <FormControl>
                <Input placeholder="Nome completo" {...field} />
              </FormControl>
              <FormMessage />
            </FormItem>
          )}
        />
        <Button type="submit" disabled={isSubmitting}>
          {isSubmitting ? 'Salvando...' : 'Salvar'}
        </Button>
      </form>
    </Form>
  )
}
```

**Regras:**
- Schema Zod sempre junto ao hook, nunca no Page
- `form.handleSubmit(onSubmit)` exposto pelo hook — Page nunca chama `onSubmit` diretamente
- Mensagens de erro em português

---

## Componentes UI — shadcn/ui

Para adicionar um novo componente shadcn:

```bash
npx shadcn@latest add <component>
```

Exemplos: `table`, `dialog`, `select`, `toast`, `badge`, `separator`.

**Nunca editar os arquivos em `src/components/ui/` manualmente.** São gerados pelo CLI e podem ser sobrescritos.

Componentes customizados (não shadcn) ficam em `src/components/layout/` ou em subpastas temáticas como `src/components/shared/`.

---

## Estilização

- **Tailwind classes diretamente no JSX** — sem arquivos `.css` por componente
- **`cn()` helper** para classes condicionais:

```tsx
import { cn } from '@/lib/utils'

<div className={cn('p-4 rounded', isActive && 'bg-primary text-primary-foreground')} />
```

- **`src/index.css`** é o único arquivo CSS global — contém as CSS variables do tema e o `@import "tailwindcss"`
- Nunca usar `style={{}}` inline para valores que o Tailwind cobre

---

## Dark / Light Mode

O sistema de tema está implementado em `src/lib/theme.ts` e `src/hooks/useTheme.ts`.

Para adicionar o toggle em qualquer layout:

```tsx
import { ThemeToggle } from '@/components/ui/ThemeToggle'

<ThemeToggle />
```

Para reagir ao tema atual em um hook ou componente:

```ts
import { useTheme } from '@/hooks/useTheme'

const { resolvedTheme, toggleTheme, setTheme } = useTheme()
// resolvedTheme: 'light' | 'dark'
// setTheme('light' | 'dark' | 'system')
```

**A preferência é persistida no `localStorage` (`sim_theme`) e aplicada sem flash** via inline script no `index.html`.

---

## Path Alias

Sempre usar `@/` para imports internos. Nunca usar caminhos relativos com `../../`.

```ts
// ✅ correto
import { api } from '@/lib/api'
import { useAuth } from '@/hooks/useAuth'
import { Button } from '@/components/ui/button'

// ❌ errado
import { api } from '../../lib/api'
```

---

## Adicionando uma Nova Feature — Checklist

```
1. Criar src/pages/<feature>/
   ├── <Feature>Page.tsx
   └── use<Feature>.ts

2. Implementar o schema Zod e o form hook (se houver formulário)

3. Implementar as chamadas HTTP no hook via api.get/post/put/delete

4. Adicionar componentes shadcn se necessário:
   npx shadcn@latest add <component>

5. Registrar a rota em src/router.tsx (dentro de PrivateRoute se autenticada)

6. Exportar o Page como default export
```

---

## Padrões Proibidos

| Proibido | Use em vez disso |
|---|---|
| Lógica de negócio no Page | `usePage.ts` |
| `fetch()` ou `axios.create()` avulso | `api` de `@/lib/api` |
| Chamar Supabase diretamente | Endpoints da `SIM.WebApi` |
| `useContext` para estado global | Zustand store |
| Importar `api` dentro de um store | Hook da página ou `src/hooks/` |
| Editar `src/components/ui/` manualmente | `npx shadcn@latest add` |
| Caminhos relativos `../../` | Alias `@/` |
| `style={{}}` inline para layout | Classes Tailwind |
| Validação manual no submit | Schema Zod + `zodResolver` |
| Múltiplos `useState` para um form | `useForm` do React Hook Form |
