import { useEffect } from 'react'

interface ToastProps {
  type: 'success' | 'error'
  message: string
  onClose: () => void
}

const Toast = ({ type, message, onClose }: ToastProps) => {
  useEffect(() => {
    const timer = setTimeout(onClose, 2500)
    return () => clearTimeout(timer)
  }, [onClose])

  return (
    <div style={{
      position: 'fixed',
      left: '50%',
      bottom: 40,
      transform: 'translateX(-50%)',
      background: type === 'success' ? '#4caf50' : '#ff4d4f',
      color: '#fff',
      padding: '14px 32px',
      borderRadius: 10,
      fontWeight: 600,
      fontSize: 16,
      zIndex: 10000,
      boxShadow: '0 2px 16px rgba(0,0,0,0.12)',
      minWidth: 220,
      textAlign: 'center',
    }}>
      {message}
    </div>
  )
}

export default Toast 