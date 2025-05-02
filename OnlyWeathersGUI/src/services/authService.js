// Importujemy bibliotekę axios – służy do wysyłania zapytań HTTP
import axios from 'axios'

// Bazowy adres do naszego backendu API
const API = 'http://localhost:5255/api'

// ------------------------
// Funkcja logowania
// ------------------------
import { token, email } from '@/services/authState'

export const login = (emailInput, password) =>
  axios.post(`${API}/auth/login`, { email: emailInput, password }).then(res => {
    const t = res.data.token
    localStorage.setItem('token', t)
    token.value = t

    const jwt = JSON.parse(atob(t.split('.')[1]))
    email.value = jwt['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name']
  })

// ------------------------
// Funkcja rejestracji
// ------------------------

export const register = (email, password) =>
  // Wysyłamy POST na /users/register z e-mailem i hasłem
  axios.post(`${API}/users/register`, { email, password })
// Uwaga: ta funkcja nie zapisuje tokena – tylko rejestruje użytkownika

// ------------------------
// Funkcja zmiany hasła
// ------------------------

export const changePassword = (currentPassword, newPassword) => {
  // Pobieramy zapisany token z localStorage
  const token = localStorage.getItem('token')

  // Wysyłamy PUT na /users/password z obecnym i nowym hasłem
  return axios.put(`${API}/users/password`,
    { currentPassword, newPassword },
    {
      headers: {
        // Dodajemy token do nagłówka Authorization w formacie Bearer
        Authorization: `Bearer ${token}`
      }
    }
  )
}
