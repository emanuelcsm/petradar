import api from '@/services/api'
import type { ApiResponse, LoginResponseDto } from '@/types/api.types'

export const authService = {
  async login(email: string, password: string): Promise<ApiResponse<LoginResponseDto>> {
    const { data } = await api.post<ApiResponse<LoginResponseDto>>('/api/auth/login', {
      email,
      password,
    })
    return data
  },

  async register(name: string, email: string, password: string): Promise<void> {
    await api.post('/api/auth/register', { name, email, password })
  },
}
