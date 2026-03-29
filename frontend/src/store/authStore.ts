import { create } from 'zustand'
import { tokenStorage } from '../lib/tokenStorage'

interface AuthUser {
  email: string
}

interface AuthState {
  user: AuthUser | null
  isAuthenticated: boolean
  setUser: (user: AuthUser | null) => void
  signOut: () => void
}

function resolveInitialUser(): AuthUser | null {
  if (!tokenStorage.hasSession() || tokenStorage.isExpired()) return null
  const email = tokenStorage.getEmail()
  return email ? { email } : null
}

export const useAuthStore = create<AuthState>((set) => ({
  user: resolveInitialUser(),
  isAuthenticated: resolveInitialUser() !== null,

  setUser: (user) => set({ user, isAuthenticated: user !== null }),

  signOut: () => {
    tokenStorage.clear()
    set({ user: null, isAuthenticated: false })
  },
}))
