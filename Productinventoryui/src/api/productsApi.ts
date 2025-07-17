import axios from 'axios'
import { useAuthStore } from '../store/authStore'
import type { AddProduct } from '../models/AddProduct'
import BASE_URL from './baseUrl'

const api = axios.create({
  baseURL: BASE_URL,
  headers: { 'accept': '*/*', 'Content-Type': 'application/json' },
})

export async function getProducts(page = 1, pageSize = 20) {
  const token = useAuthStore.getState().token
  const res = await api.get(`/Products?page=${page}&pageSize=${pageSize}`, {
    headers: { Authorization: `Bearer ${token}` },
  })
  return res.data
}

export async function getProductById(id: string) {
  const token = useAuthStore.getState().token
  const res = await api.get(`/Products/${id}`, {
    headers: { Authorization: `Bearer ${token}` },
  })
  return res.data
}

export async function addProduct(product: AddProduct) {
  const token = useAuthStore.getState().token
  const res = await api.post('/Products', product, {
    headers: { Authorization: `Bearer ${token}` },
  })
  return res.data
}

export async function addVariantStock(combinationId: string, quantity: number) {
  const token = useAuthStore.getState().token
  const res = await api.post('/Products/add-stock', { combinationId, quantity }, {
    headers: { Authorization: `Bearer ${token}` },
  })
  return res.data
}

export async function removeVariantStock(combinationId: string, quantity: number) {
  const token = useAuthStore.getState().token
  const res = await api.post('/Products/remove-stock', { combinationId, quantity }, {
    headers: { Authorization: `Bearer ${token}` },
  })
  return res.data
} 