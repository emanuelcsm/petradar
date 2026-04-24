import api from '@/services/api'
import type { ApiResponse, LoginResponseDto } from '@/types/api.types'

export const authService = {
  async login(_email: string, _password: string): Promise<ApiResponse<LoginResponseDto>> {
    const { data } = await api.post<ApiResponse<LoginResponseDto>>('/api/auth/login', {
      email: _email,
      password: _password,
    })
    return data
  },

  async register(_email: string, _name: string, _password: string): Promise<void> {
    await api.post('/api/auth/register', {
      email: _email,
      name: _name,
      password: _password,
    })
  },
}
