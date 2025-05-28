import api from '@/services/api'
import { token, email } from '@/services/authState'

// Logowanie – wysyła dane i zapisuje token + email z JWT
export const login = (emailInput, password) =>
  api.post('/api/auth/login', { email: emailInput, password }).then(res => {
    const t = res.data.token
    localStorage.setItem('token', t)
    token.value = t

    const jwt = JSON.parse(atob(t.split('.')[1]))
    email.value = jwt['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name']
  })

// Rejestracja – tylko wysyła dane, bez zapisu tokena
export const register = (email, password) =>
  api.post('/api/users/register', { email, password })

// Zmiana hasła – wymaga tokena w nagłówku
export const changePassword = (currentPassword, newPassword) => {
  const t = localStorage.getItem('token')

  return api.put('/api/users/password',
    { currentPassword, newPassword },
    {
      headers: {
        Authorization: `Bearer ${t}`
      }
    }
  )
}
