import { useEffect, useState } from 'react'
import { useParams, useNavigate } from 'react-router-dom'
import { getProductById, addVariantStock, removeVariantStock } from '../api/productsApi'
import type { Product, VariantCombination } from '../models/Product'
import Table from '../components/table/Table'
import Toast from '../components/toast/Toast'
import type { Row } from '@tanstack/react-table'

const ProductVariants = () => {
  const { id } = useParams()
  const navigate = useNavigate()
  const [data, setData] = useState<VariantCombination[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')
  const [productName, setProductName] = useState('')
  const [toast, setToast] = useState<{ type: 'success' | 'error', message: string } | null>(null)

  const fetchData = () => {
    if (!id) return
    setLoading(true)
    getProductById(id).then((res: Product) => {
      setData(res.variantCombinations)
      setProductName(res.productName)
      setLoading(false)
    }).catch(() => {
      setError('Failed to load variants')
      setLoading(false)
    })
  }

  useEffect(() => {
    fetchData()
    // eslint-disable-next-line
  }, [id])

  const handleAddStock = async (combinationId: string) => {
    try {
      await addVariantStock(combinationId, 1)
      setToast({ type: 'success', message: 'Stock added!' })
      fetchData()
    } catch {
      setToast({ type: 'error', message: 'Failed to add stock' })
    }
  }
  const handleRemoveStock = async (combinationId: string) => {
    try {
      await removeVariantStock(combinationId, 1)
      setToast({ type: 'success', message: 'Stock removed!' })
      fetchData()
    } catch {
      setToast({ type: 'error', message: 'Failed to remove stock' })
    }
  }

  const columns = [
    { header: 'Combination Code', accessorKey: 'combinationCode' },
    { header: 'Stock', accessorKey: 'stock' },
    {
      header: 'Options',
      accessorKey: 'options',
      cell: ({ row }: { row: Row<VariantCombination> }) => row.original.options.map(o => `${o.variant}: ${o.option}`).join(', '),
    },
    {
      header: 'Actions',
      id: 'actions',
      cell: ({ row }: { row: Row<VariantCombination> }) => (
        <div style={{ display: 'flex', gap: 8 }}>
          <button onClick={() => handleAddStock(row.original.id)} style={{ background: '#2d8cff', color: '#fff', border: 'none', borderRadius: 6, padding: '4px 10px', fontWeight: 600, cursor: 'pointer' }}>+</button>
          <button onClick={() => handleRemoveStock(row.original.id)} style={{ background: '#ff4d4f', color: '#fff', border: 'none', borderRadius: 6, padding: '4px 10px', fontWeight: 600, cursor: 'pointer' }}>-</button>
        </div>
      ),
    },
  ]

  return (
    <div style={{ minHeight: '100vh', background: '#f5f6fa' }}>
      <div style={{ display: 'flex', justifyContent: 'flex-end', alignItems: 'center', margin: '24px 0 8px 0', paddingRight: 32 }}>
        <button onClick={() => navigate(-1)} style={{ background: '#2d8cff', color: '#fff', border: 'none', borderRadius: 6, padding: '8px 18px', fontWeight: 600, cursor: 'pointer' }}>Back</button>
      </div>
      <h2 style={{ textAlign: 'center', marginTop: 0 }}>{productName} Variants Stock Mangement</h2>
      {loading ? <div style={{ textAlign: 'center' }}>Loading...</div> : error ? <div style={{ color: 'red', textAlign: 'center' }}>{error}</div> : (
        <Table columns={columns} data={data} />
      )}
      {toast && <Toast type={toast.type} message={toast.message} onClose={() => setToast(null)} />}
    </div>
  )
}

export default ProductVariants 