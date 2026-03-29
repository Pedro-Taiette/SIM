import { Button } from '@/components/ui/button'
import { useAuth } from '@/hooks/useAuth'

// Placeholder — replace with real dashboard content.
export default function DashboardPage() {
  const { user, signOut } = useAuth()

  return (
    <div className="min-h-screen flex flex-col items-center justify-center gap-4">
      <p className="text-slate-600">Signed in as <span className="font-medium">{user?.email}</span></p>
      <Button variant="outline" onClick={signOut}>Sign out</Button>
    </div>
  )
}
