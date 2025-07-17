import { useAuthStore } from '../store/authStore'
import { Navigate } from 'react-router-dom'
import type { ReactNode } from 'react'

const PublicRoute = ({ children }: { children: ReactNode }) => {
  const token = useAuthStore(s => s.token)
  if (token) return <Navigate to="/products" replace />
  return <>{children}</>
}

export default PublicRoute 