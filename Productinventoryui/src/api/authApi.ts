import axios from 'axios'
import BASE_URL from './baseUrl'

export const api = axios.create({
  baseURL: BASE_URL,
  headers: {
    'accept': '*/*',
    'Content-Type': 'application/json',
  },
  withCredentials: false,
})

export async function login(email: string, password: string) {
  const res = await api.post('/User/login', { email, password })
  console.log(res)
  return res.data
}

export async function registerUser(userName: string, email: string, password: string) {
  const res = await api.post('/User/register', { userName, email, password })
  return res.data
}