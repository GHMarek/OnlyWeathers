// Helper do pobierania portu api z pliku .env

import axios from 'axios'

const api = axios.create({
  baseURL: import.meta.env.VITE_API_URL 
})

export default api