import { create } from 'zustand'

interface AuthState {
  token: string | null
  id: string | null
  userName: string | null
  email: string | null
  setAuth: (data: { token: string, id: string, userName: string, email: string }) => void
  logout: () => void
}

export const useAuthStore = create<AuthState>((set) => ({
  token: null,
  id: null,
  userName: null,
  email: null,
  setAuth: (data) => set({ ...data }),
  logout: () => set({ token: null, id: null, userName: null, email: null })
})) 