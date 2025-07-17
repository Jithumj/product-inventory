import { useState } from 'react'
import { useAuthStore } from '../store/authStore'
import { login, registerUser } from '../api/authApi'
import { useNavigate } from 'react-router-dom'
import Toast from '../components/toast/Toast'

const Login = () => {
  // State for login form
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  // State for toast notifications
  const [toast, setToast] = useState<{ type: 'success' | 'error', message: string } | null>(null)
  // State for registration dialog
  const [showRegister, setShowRegister] = useState(false)
  const [regForm, setRegForm] = useState({ userName: '', email: '', password: '' })
  const [regLoading, setRegLoading] = useState(false)
  const setAuth = useAuthStore((s) => s.setAuth)
  const navigate = useNavigate()

  // Handle login form submit
  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    setToast(null)
    if (password.length < 6) {
      setToast({ type: 'error', message: 'Password must be at least 6 characters' })
      return
    }
    try {
      const data = await login(email, password)
      setAuth(data)
      setToast({ type: 'success', message: 'Login successful!' })
      setTimeout(() => navigate('/products'), 500)
    } catch (err: any) {
      setToast({ type: 'error', message: 'Invalid credentials' })
    }
  }

  // Handle registration form submit
  const handleRegister = async (e: React.FormEvent) => {
    e.preventDefault()
    setRegLoading(true)
    if (regForm.password.length < 6) {
      setToast({ type: 'error', message: 'Password must be at least 6 characters' })
      setRegLoading(false)
      return
    }
    try {
      await registerUser(regForm.userName, regForm.email, regForm.password)
      setToast({ type: 'success', message: 'User registered successfully!' })
      setShowRegister(false)
      setRegForm({ userName: '', email: '', password: '' })
    } catch {
      setToast({ type: 'error', message: 'Failed to register user' })
    } finally {
      setRegLoading(false)
    }
  }

  return (
    // Login form UI
    <div style={{ minHeight: '100vh', display: 'flex', alignItems: 'center', justifyContent: 'center', background: '#f5f6fa' }}>
      <form onSubmit={handleSubmit} style={{ background: '#fff', padding: 32, borderRadius: 12, boxShadow: '0 2px 16px rgba(0,0,0,0.08)', minWidth: 320, display: 'flex', flexDirection: 'column', gap: 18 }}>
        <h2 style={{ textAlign: 'center', marginBottom: 8 }}>Login</h2>
        <input type="email" placeholder="Email" value={email} onChange={e => setEmail(e.target.value)} required style={{ padding: 10, borderRadius: 6, border: '1px solid #ddd', fontSize: 16 }} />
        <input type="password" placeholder="Password" value={password} onChange={e => setPassword(e.target.value)} required style={{ padding: 10, borderRadius: 6, border: '1px solid #ddd', fontSize: 16 }} />
        <button type="submit" style={{ padding: 12, borderRadius: 6, background: '#2d8cff', color: '#fff', fontWeight: 600, fontSize: 16, border: 'none', cursor: 'pointer', marginTop: 8 }}>Login</button>
        <span style={{ textAlign: 'center', marginTop: 8, textDecoration: 'underline', color: '#2d8cff', cursor: 'pointer' }} onClick={() => setShowRegister(true)}>
          Register User
        </span>
      </form>
      {/* Registration dialog */}
      {showRegister && (
        <div style={{ position: 'fixed', top: 0, left: 0, width: '100vw', height: '100vh', background: 'rgba(0,0,0,0.2)', display: 'flex', alignItems: 'center', justifyContent: 'center', zIndex: 1000 }}>
          <form onSubmit={handleRegister} style={{ background: '#fff', padding: 32, borderRadius: 12, minWidth: 320, boxShadow: '0 2px 16px rgba(0,0,0,0.08)', display: 'flex', flexDirection: 'column', gap: 16 }}>
            <h3>Register User</h3>
            <input placeholder="User Name" value={regForm.userName} onChange={e => setRegForm(f => ({ ...f, userName: e.target.value }))} required style={{ padding: 10, borderRadius: 6, border: '1px solid #ddd', fontSize: 16 }} />
            <input type="email" placeholder="Email" value={regForm.email} onChange={e => setRegForm(f => ({ ...f, email: e.target.value }))} required style={{ padding: 10, borderRadius: 6, border: '1px solid #ddd', fontSize: 16 }} />
            <input type="password" placeholder="Password" value={regForm.password} onChange={e => setRegForm(f => ({ ...f, password: e.target.value }))} required style={{ padding: 10, borderRadius: 6, border: '1px solid #ddd', fontSize: 16 }} />
            <div style={{ display: 'flex', gap: 12, marginTop: 12 }}>
              <button type="submit" disabled={regLoading} style={{ background: '#2d8cff', color: '#fff', border: 'none', borderRadius: 6, padding: '10px 18px', fontWeight: 600, cursor: 'pointer' }}>{regLoading ? 'Registering...' : 'Register'}</button>
              <button type="button" onClick={() => setShowRegister(false)} style={{ background: '#ff4d4f', color: '#fff', border: 'none', borderRadius: 6, padding: '10px 18px', fontWeight: 600, cursor: 'pointer' }}>Cancel</button>
            </div>
          </form>
        </div>
      )}
      {/* Toast notification */}
      {toast && <Toast type={toast.type} message={toast.message} onClose={() => setToast(null)} />}
    </div>
  )
}

export default Login
