import axios from 'axios'

const API = 'http://localhost:5255/api'

export const login = (email, password) =>
  axios.post(`${API}/auth/login`, { email, password }).then(res => {
    localStorage.setItem('token', res.data.token)
  })

export const register = (email, password) =>
  axios.post(`${API}/auth/register`, { email, password })

export const changePassword = (currentPassword, newPassword) => {
  const token = localStorage.getItem('token')
  return axios.put(`${API}/users/password`, { currentPassword, newPassword }, {
    headers: { Authorization: `Bearer ${token}` }
  })
}
