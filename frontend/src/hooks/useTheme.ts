import { useEffect, useState } from 'react'
import {
  type Theme,
  applyTheme,
  getStoredTheme,
  getSystemTheme,
  setStoredTheme,
} from '@/lib/theme'

export function useTheme() {
  const [theme, setThemeState] = useState<Theme>(getStoredTheme)

  // Apply theme class to <html> whenever it changes
  useEffect(() => {
    applyTheme(theme)
  }, [theme])

  // Re-apply when the OS preference changes and theme is set to 'system'
  useEffect(() => {
    if (theme !== 'system') return
    const media = window.matchMedia('(prefers-color-scheme: dark)')
    const handler = () => applyTheme('system')
    media.addEventListener('change', handler)
    return () => media.removeEventListener('change', handler)
  }, [theme])

  function setTheme(next: Theme): void {
    setStoredTheme(next)
    setThemeState(next)
  }

  function toggleTheme(): void {
    const resolved = theme === 'system' ? getSystemTheme() : theme
    setTheme(resolved === 'dark' ? 'light' : 'dark')
  }

  return {
    theme,
    resolvedTheme: theme === 'system' ? getSystemTheme() : theme,
    setTheme,
    toggleTheme,
  }
}
