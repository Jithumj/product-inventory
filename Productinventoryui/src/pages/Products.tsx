import { useAuthStore } from '../store/authStore'
import { useNavigate } from 'react-router-dom'
import Table from '../components/table/Table'
import { useEffect, useState } from 'react'
import { getProducts, addProduct } from '../api/productsApi'
import type { Product as ProductBase } from '../models/Product'
import type { CellContext, Row } from '@tanstack/react-table'
import type { AddProduct, AddProductVariant } from '../models/AddProduct'
import Toast from '../components/toast/Toast'

// Extend Product type for table row
interface Product extends ProductBase {
  onShowVariants: (id: string) => void
}

const columns = [
  { header: 'Product Name', accessorKey: 'productName' },
  { header: 'Created Date', accessorKey: 'createdDate', cell: (info: CellContext<Product, any>) => new Date(info.getValue()).toLocaleString() },
  { header: 'Product Code', accessorKey: 'productCode' },
  { header: 'Stock', accessorKey: 'totalStock' },
  {
    header: 'Actions',
    id: 'actions',
    cell: ({ row }: { row: Row<Product> }) => <a href="#" onClick={e => { e.preventDefault(); row.original.onShowVariants(row.original.id) }}>Show Variants</a>,
  },
]

const Products = () => {
  const logout = useAuthStore(s => s.logout)
  const navigate = useNavigate()
  const [data, setData] = useState<Product[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')
  const userId = useAuthStore(s => s.id)
  const [showModal, setShowModal] = useState(false)
  const [form, setForm] = useState<AddProduct>({
    productCode: '',
    productName: '',
    createdUser: userId || '',
    variants: [{ name: '', options: [''] }],
  })
  const [toast, setToast] = useState<{ type: 'success' | 'error', message: string } | null>(null)

  useEffect(() => {
    setLoading(true)
    getProducts().then(res => {
      setData(res.map((p: ProductBase) => ({ ...p, onShowVariants: (id: string) => navigate(`/variants/${id}`) })))
      setLoading(false)
    }).catch(() => {
      setError('Failed to load products')
      setLoading(false)
    })
  }, [navigate])

  const handleLogout = () => {
    logout()
    navigate('/login')
  }
  const handleAdd = () => setShowModal(true)
  const handleFormChange = (field: keyof AddProduct, value: any) => setForm(f => ({ ...f, [field]: value }))
  const handleVariantChange = (idx: number, field: keyof AddProductVariant, value: any) => setForm(f => ({ ...f, variants: f.variants.map((v, i) => i === idx ? { ...v, [field]: value } : v) }))
  const handleOptionChange = (vIdx: number, oIdx: number, value: string) => setForm(f => ({ ...f, variants: f.variants.map((v, i) => i === vIdx ? { ...v, options: v.options.map((o, j) => j === oIdx ? value : o) } : v) }))
  const addVariant = () => setForm(f => ({ ...f, variants: [...f.variants, { name: '', options: [''] }] }))
  const removeVariant = (idx: number) => setForm(f => ({ ...f, variants: f.variants.filter((_, i) => i !== idx) }))
  const addOption = (vIdx: number) => setForm(f => ({ ...f, variants: f.variants.map((v, i) => i === vIdx ? { ...v, options: [...v.options, ''] } : v) }))
  const removeOption = (vIdx: number, oIdx: number) => setForm(f => ({ ...f, variants: f.variants.map((v, i) => i === vIdx ? { ...v, options: v.options.filter((_, j) => j !== oIdx) } : v) }))

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    try {
      await addProduct({ ...form, createdUser: userId || '' })
      setToast({ type: 'success', message: 'Product added successfully!' })
      setShowModal(false)
      setForm({ productCode: '', productName: '', createdUser: userId || '', variants: [{ name: '', options: [''] }] })
      // Refresh product list
      setLoading(true)
      getProducts().then(res => {
        setData(res.map((p: ProductBase) => ({ ...p, onShowVariants: (id: string) => navigate(`/variants/${id}`) })))
        setLoading(false)
      })
    } catch {
      setToast({ type: 'error', message: 'Failed to add product' })
    }
  }

  const handleUsers = () => navigate('/users')

  return (
    <div style={{ minHeight: '100vh', background: '#f5f6fa' }}>
      <div style={{ display: 'flex', justifyContent: 'flex-end', alignItems: 'center', padding: 24, gap: 12 }}>
        <button onClick={handleUsers} style={{ background: '#2d8cff', color: '#fff', border: 'none', borderRadius: 6, padding: '8px 16px', fontWeight: 600, cursor: 'pointer' }}>Users</button>
        <button onClick={handleLogout} style={{ background: '#ff4d4f', color: '#fff', border: 'none', borderRadius: 6, padding: '8px 16px', fontWeight: 600, cursor: 'pointer' }}>Logout</button>
      </div>
      <h2 style={{ textAlign: 'center', marginTop: 0 }}>Products</h2>
      {loading ? <div style={{ textAlign: 'center' }}>Loading...</div> : error ? <div style={{ color: 'red', textAlign: 'center' }}>{error}</div> : (
        <Table columns={columns} data={data} onAdd={handleAdd} addLabel="Add Product" />
      )}
      {toast && <Toast type={toast.type} message={toast.message} onClose={() => setToast(null)} />}
      {showModal && (
        <div style={{ position: 'fixed', top: 0, left: 0, width: '100vw', height: '100vh', background: 'rgba(0,0,0,0.2)', display: 'flex', alignItems: 'center', justifyContent: 'center', zIndex: 1000 }}>
          <form onSubmit={handleSubmit} style={{ background: '#fff', padding: 32, borderRadius: 12, minWidth: 340, boxShadow: '0 2px 16px rgba(0,0,0,0.08)', display: 'flex', flexDirection: 'column', gap: 16 }}>
            <h3>Add Product</h3>
            <input placeholder="Product Code" value={form.productCode} onChange={e => handleFormChange('productCode', e.target.value)} required style={{ padding: 10, borderRadius: 6, border: '1px solid #ddd', fontSize: 16 }} />
            <input placeholder="Product Name" value={form.productName} onChange={e => handleFormChange('productName', e.target.value)} required style={{ padding: 10, borderRadius: 6, border: '1px solid #ddd', fontSize: 16 }} />
            <div style={{ display: 'flex', flexDirection: 'column', gap: 8 }}>
              <label style={{ fontWeight: 600 }}>Variants</label>
              {form.variants.map((variant, vIdx) => (
                <div key={vIdx} style={{ border: '1px solid #eee', borderRadius: 6, padding: 8, marginBottom: 8 }}>
                  <input placeholder="Variant Name (e.g. size)" value={variant.name} onChange={e => handleVariantChange(vIdx, 'name', e.target.value)} required style={{ padding: 8, borderRadius: 4, border: '1px solid #ddd', fontSize: 15, marginRight: 8 }} />
                  <button type="button" onClick={() => removeVariant(vIdx)} style={{ background: '#ff4d4f', color: '#fff', border: 'none', borderRadius: 4, padding: '4px 10px', fontWeight: 600, cursor: 'pointer', marginLeft: 4 }}>Remove</button>
                  <div style={{ marginTop: 8, display: 'flex', flexDirection: 'column', gap: 4 }}>
                    {variant.options.map((option, oIdx) => (
                      <div key={oIdx} style={{ display: 'flex', alignItems: 'center', gap: 4 }}>
                        <input placeholder="Option (e.g. M, L, S)" value={option} onChange={e => handleOptionChange(vIdx, oIdx, e.target.value)} required style={{ padding: 6, borderRadius: 4, border: '1px solid #ddd', fontSize: 15 }} />
                        <button type="button" onClick={() => removeOption(vIdx, oIdx)} style={{ background: '#ff9800', color: '#fff', border: 'none', borderRadius: 4, padding: '2px 8px', fontWeight: 600, cursor: 'pointer' }}>Remove</button>
                      </div>
                    ))}
                    <button type="button" onClick={() => addOption(vIdx)} style={{ background: '#2d8cff', color: '#fff', border: 'none', borderRadius: 4, padding: '4px 10px', fontWeight: 600, cursor: 'pointer', marginTop: 4 }}>Add Option</button>
                  </div>
                </div>
              ))}
              <button type="button" onClick={addVariant} style={{ background: '#2d8cff', color: '#fff', border: 'none', borderRadius: 4, padding: '6px 12px', fontWeight: 600, cursor: 'pointer', marginTop: 8 }}>Add Variant</button>
            </div>
            <div style={{ display: 'flex', gap: 12, marginTop: 12 }}>
              <button type="submit" style={{ background: '#4caf50', color: '#fff', border: 'none', borderRadius: 6, padding: '10px 18px', fontWeight: 600, cursor: 'pointer' }}>Add Product</button>
              <button type="button" onClick={() => setShowModal(false)} style={{ background: '#ff4d4f', color: '#fff', border: 'none', borderRadius: 6, padding: '10px 18px', fontWeight: 600, cursor: 'pointer' }}>Cancel</button>
            </div>
          </form>
        </div>
      )}
    </div>
  )
}

export default Products
