import axios from 'axios'
import { useAuthStore } from '../store/authStore'
import BASE_URL from './baseUrl'

const api = axios.create({
  baseURL: BASE_URL,
  headers: { 'accept': '*/*', 'Content-Type': 'application/json' },
})

export async function getAllUsers() {
  const token = useAuthStore.getState().token
  const res = await api.get('/User/all', {
    headers: { Authorization: `Bearer ${token}` },
  })
  return res.data
}

export async function deleteUser(id: string) {
  const token = useAuthStore.getState().token
  const res = await api.delete(`/User/${id}`, {
    headers: { Authorization: `Bearer ${token}` },
  })
  return res.status
} 