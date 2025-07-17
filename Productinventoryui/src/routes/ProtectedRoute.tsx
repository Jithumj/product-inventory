import { useAuthStore } from '../store/authStore'
import { Navigate } from 'react-router-dom'
import type { ReactNode } from 'react'

const ProtectedRoute = ({ children }: { children: ReactNode }) => {
  const token = useAuthStore(s => s.token)
  if (!token) return <Navigate to="/login" replace />
  return <>{children}</>
}

export default ProtectedRoute 