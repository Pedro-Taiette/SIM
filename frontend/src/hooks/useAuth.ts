import { api } from '../lib/api'
import { tokenStorage } from '../lib/tokenStorage'
import { useAuthStore } from '../store/authStore'

export function useAuth() {
  const { user, isAuthenticated, setUser, signOut } = useAuthStore()

  async function signIn(email: string, password: string): Promise<void> {
    const { data } = await api.post('/api/auth/login', { email, password })
    tokenStorage.save(data.accessToken, data.refreshToken, data.expiresIn, email)
    setUser({ email })
  }

  return { user, isAuthenticated, signIn, signOut }
}
