import axios from 'axios'

const api = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL,
  headers: { 'Content-Type': 'application/json' },
})

api.interceptors.request.use(config => {
  const token = localStorage.getItem('auth_token')
  if (token) {
    config.headers.Authorization = `Bearer ${token}`
  }
  // For FormData payloads the browser must set Content-Type with the multipart
  // boundary. Deleting the default here lets XMLHttpRequest do it automatically.
  if (config.data instanceof FormData) {
    delete config.headers['Content-Type']
  }
  return config
})

api.interceptors.response.use(
  response => response,
  async error => {
    if (error.response?.status === 401) {
      const { useAuthStore } = await import('@/modules/auth/stores/auth.store')
      useAuthStore().logout()
      window.location.href = '/login'
    }
    return Promise.reject(error)
  },
)

export default api
