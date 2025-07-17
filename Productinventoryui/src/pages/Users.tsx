import { useEffect, useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { getAllUsers, deleteUser } from '../api/usersApi'
import type { User } from '../models/User'
import Toast from '../components/toast/Toast'
import Table from '../components/table/Table'
import type { Row } from '@tanstack/react-table'

const Users = () => {
  const [users, setUsers] = useState<User[]>([])
  const navigate = useNavigate()
  const [loading, setLoading] = useState(true)
  const [toast, setToast] = useState<{ type: 'success' | 'error', message: string } | null>(null)
  const [confirmId, setConfirmId] = useState<string | null>(null)

  const fetchUsers = () => {
    setLoading(true)
    getAllUsers().then(setUsers).catch(() => setToast({ type: 'error', message: 'Failed to fetch users' })).finally(() => setLoading(false))
  }

  useEffect(() => {
    fetchUsers()
  }, [])

  const handleDelete = async (id: string) => {
    try {
      const status = await deleteUser(id)
      if (status === 204) {
        setToast({ type: 'success', message: 'User deleted!' })
        fetchUsers()
      } else {
        setToast({ type: 'error', message: 'Failed to delete user' })
      }
    } catch {
      setToast({ type: 'error', message: 'Failed to delete user' })
    }
    setConfirmId(null)
  }

  const columns = [
    { header: 'User Name', accessorKey: 'userName' },
    { header: 'Email', accessorKey: 'email' },
    {
      header: 'Actions',
      id: 'actions',
      cell: ({ row }: { row: Row<User> }) => (
        <button onClick={() => setConfirmId(row.original.id)} style={{ background: '#ff4d4f', color: '#fff', border: 'none', borderRadius: 6, padding: '6px 14px', fontWeight: 600, cursor: 'pointer' }}>Delete</button>
      ),
    },
  ]

  return (
    <div style={{ minHeight: '100vh', background: '#f5f6fa', padding: 32 }}>
         <div style={{ display: 'flex', justifyContent: 'flex-end', alignItems: 'center', margin: '24px 0 8px 0', paddingRight: 32 }}>
        <button onClick={() => navigate(-1)} style={{ background: '#2d8cff', color: '#fff', border: 'none', borderRadius: 6, padding: '8px 18px', fontWeight: 600, cursor: 'pointer' }}>Back</button>
      </div>
      <h2 style={{ textAlign: 'center', marginBottom: 24 }}>Users</h2>
      {loading ? <div style={{ textAlign: 'center' }}>Loading...</div> : (
        <Table columns={columns} data={users} />
      )}
      {confirmId && (
        <div style={{ position: 'fixed', top: 0, left: 0, width: '100vw', height: '100vh', background: 'rgba(0,0,0,0.2)', display: 'flex', alignItems: 'center', justifyContent: 'center', zIndex: 1000 }}>
          <div style={{ background: '#fff', padding: 32, borderRadius: 12, minWidth: 320, boxShadow: '0 2px 16px rgba(0,0,0,0.08)', display: 'flex', flexDirection: 'column', gap: 16 }}>
            <h3>Delete User?</h3>
            <p>Are you sure you want to delete this user?</p>
            <div style={{ display: 'flex', gap: 12, marginTop: 12 }}>
              <button onClick={() => handleDelete(confirmId)} style={{ background: '#ff4d4f', color: '#fff', border: 'none', borderRadius: 6, padding: '10px 18px', fontWeight: 600, cursor: 'pointer' }}>Delete</button>
              <button onClick={() => setConfirmId(null)} style={{ background: '#2d8cff', color: '#fff', border: 'none', borderRadius: 6, padding: '10px 18px', fontWeight: 600, cursor: 'pointer' }}>Cancel</button>
            </div>
          </div>
        </div>
      )}
      {toast && <Toast type={toast.type} message={toast.message} onClose={() => setToast(null)} />}
    </div>
  )
}

export default Users 