// src/services/authService.js
import axios from 'axios';

const API = 'http://localhost:5000/api'; // zamienić port jeśli jest inny

export const login = async (email, password) => {
  const res = await axios.post(`${API}/auth/login`, { email, password });
  const token = res.data.token;
  localStorage.setItem('token', token);
  return token;
};

export const register = async (email, password) => {
  return await axios.post(`${API}/auth/register`, { email, password });
};

export const changePassword = async (currentPassword, newPassword) => {
  const token = localStorage.getItem('token');
  return await axios.put(
    `${API}/users/password`,
    { currentPassword, newPassword },
    { headers: { Authorization: `Bearer ${token}` } }
  );
};
